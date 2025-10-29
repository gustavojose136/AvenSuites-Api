# 🔐 Como Guardar Credenciais IPM com Segurança

## 📋 Visão Geral

As credenciais IPM (username e password) são **criptografadas automaticamente** antes de serem salvas no banco de dados usando **AES-256**. Quando necessário para chamar o webservice, a senha é **descriptografada automaticamente** em memória.

---

## ✅ Implementação Automática

### **1. Sistema de Criptografia**

Criamos um serviço `ISecureEncryptionService` que:
- ✅ Criptografa a senha **antes de salvar** no banco
- ✅ Descriptografa a senha **ao buscar** para uso
- ✅ Usa **AES-256-CBC** com IV aleatório
- ✅ Armazena no banco em formato **Base64**

### **2. Serviço de Credenciais IPM**

O `IIpmCredentialsService` faz a abstração completa:
- ✅ **Salvar**: Você passa a senha em texto plano, ela é criptografada automaticamente
- ✅ **Buscar**: A senha vem descriptografada automaticamente
- ✅ **Atualizar**: Detecta se precisa criptografar ou não

---

## 🚀 Como Usar

### **Salvar Credenciais (Exemplo via API ou Seed)**

```csharp
var credentials = new IpmCredentials
{
    Id = Guid.NewGuid(),
    HotelId = hotelId,
    Username = "usuario_ipm",
    Password = "senha_em_texto_plano", // 👈 Você passa em texto plano
    CpfCnpj = "12.345.678/0001-90",
    CityCode = "1234",
    SerieNfse = "1",
    Active = true
};

// O serviço criptografa automaticamente antes de salvar
await _ipmCredentialsService.AddAsync(credentials);
```

### **Buscar Credenciais (Para usar no webservice)**

```csharp
// A senha vem descriptografada automaticamente
var credentials = await _ipmCredentialsService.GetDecryptedByHotelIdAsync(hotelId);

if (credentials != null)
{
    // credentials.Password está em texto plano, pronto para usar
    await _httpClient.PostAsync(endpoint, xml, credentials.Username, credentials.Password);
}
```

### **Atualizar Credenciais**

```csharp
var credentials = await _ipmCredentialsService.GetDecryptedByHotelIdAsync(hotelId);
credentials.Password = "nova_senha_texto_plano"; // Nova senha em texto plano

// O serviço detecta e criptografa automaticamente
await _ipmCredentialsService.UpdateAsync(credentials);
```

---

## ⚙️ Configuração

### **1. Configurar Chave de Criptografia**

No `appsettings.json`:

```json
{
  "Security": {
    "EncryptionKey": "SuaChaveComExatamente32Caracteres!!"
  }
}
```

**⚠️ IMPORTANTE:**
- A chave deve ter **exatamente 32 caracteres** (256 bits)
- **NUNCA** commite a chave real no Git
- Use variáveis de ambiente em produção
- Gere uma chave forte aleatória:

```csharp
// Gerar chave aleatória de 32 bytes (Base64)
var keyBytes = new byte[32];
RandomNumberGenerator.Fill(keyBytes);
var base64Key = Convert.ToBase64String(keyBytes);
// Copie o resultado e use no appsettings
```

### **2. Em Produção (Segurança Máxima)**

#### **Opção 1: Variável de Ambiente**
```bash
export Security__EncryptionKey="SuaChaveComExatamente32Caracteres!!"
```

#### **Opção 2: Azure Key Vault / AWS Secrets Manager**
```csharp
// No Program.cs ou Startup.cs
var keyVaultUri = builder.Configuration["KeyVault:Uri"];
var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
var encryptionKey = await client.GetSecretAsync("IpmEncryptionKey");
```

#### **Opção 3: Docker Secrets**
```yaml
services:
  api:
    secrets:
      - ipm_encryption_key
secrets:
  ipm_encryption_key:
    external: true
```

---

## 🔒 Segurança Implementada

### **✅ O que está protegido:**
- ✅ Senha criptografada no banco de dados
- ✅ IV (Initialization Vector) aleatório para cada criptografia
- ✅ AES-256 (padrão militar)
- ✅ A senha nunca é logada ou exposta

### **⚠️ Boas Práticas:**
1. **NUNCA** logue `credentials.Password` em texto plano
2. **NUNCA** retorne a senha em APIs públicas
3. **SEMPRE** use HTTPS em produção
4. **ROTACIONE** a chave de criptografia periodicamente
5. **BACKUP** seguro da chave de criptografia (sem ela, dados são irrecuperáveis)

---

## 📊 Como Funciona Internamente

### **1. Ao Salvar:**
```
Senha em Texto Plano → AES-256 Encrypt → Base64 → Banco de Dados
```

### **2. Ao Buscar:**
```
Banco de Dados → Base64 Decode → AES-256 Decrypt → Senha em Texto Plano (em memória)
```

### **3. Estrutura no Banco:**
```sql
IpmCredentials:
├── Id: Guid
├── HotelId: Guid
├── Username: "usuario_ipm" (texto plano - OK)
├── Password: "aGVsbG8gd29ybGQ..." (Base64 criptografado) ✅
├── CpfCnpj: "12.345.678/0001-90"
├── CityCode: "1234"
└── Active: true
```

---

## 🧪 Testando

### **Teste Manual:**

1. **Salvar credenciais:**
```csharp
var service = serviceProvider.GetRequiredService<IIpmCredentialsService>();
var creds = new IpmCredentials { /* ... */, Password = "MinhaSenha123" };
await service.AddAsync(creds);
```

2. **Verificar no banco:**
```sql
SELECT Password FROM IpmCredentials WHERE HotelId = '...';
-- Deve mostrar algo como: "aGVsbG8gd29ybGQhIS..."
-- NÃO deve mostrar "MinhaSenha123"
```

3. **Buscar e usar:**
```csharp
var creds = await service.GetDecryptedByHotelIdAsync(hotelId);
// creds.Password deve ser "MinhaSenha123" novamente
```

---

## 📝 Exemplo Completo de Uso

### **Controller para Gerenciar Credenciais:**

```csharp
[ApiController]
[Route("api/hotels/{hotelId}/ipm-credentials")]
[Authorize]
public class IpmCredentialsController : ControllerBase
{
    private readonly IIpmCredentialsService _credentialsService;

    [HttpPost]
    public async Task<IActionResult> CreateCredentials(
        Guid hotelId,
        [FromBody] CreateIpmCredentialsRequest request)
    {
        var credentials = new IpmCredentials
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Username = request.Username,
            Password = request.Password, // Texto plano - será criptografado
            CpfCnpj = request.CpfCnpj,
            CityCode = request.CityCode,
            SerieNfse = request.SerieNfse ?? "1",
            Active = true
        };

        var created = await _credentialsService.AddAsync(credentials);
        
        // IMPORTANTE: Não retornar a senha!
        return Ok(new { Id = created.Id, Username = created.Username });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCredentials(
        Guid id,
        [FromBody] UpdateIpmCredentialsRequest request)
    {
        var credentials = await _credentialsService.GetDecryptedByHotelIdAsync(
            request.HotelId);
        
        if (credentials == null)
            return NotFound();

        if (!string.IsNullOrEmpty(request.Password))
            credentials.Password = request.Password; // Será criptografado

        await _credentialsService.UpdateAsync(credentials);
        return NoContent();
    }
}
```

---

## ✅ Resumo

### **O que você precisa fazer:**
1. ✅ Configure a chave de 32 caracteres no `appsettings.json`
2. ✅ Use `IIpmCredentialsService` para salvar/buscar credenciais
3. ✅ Passe senhas em **texto plano** - a criptografia é automática
4. ✅ Nunca logue ou retorne senhas em APIs públicas

### **O que está implementado:**
- ✅ Criptografia AES-256 automática
- ✅ Descriptografia automática ao buscar
- ✅ IV aleatório para cada criptografia
- ✅ Serviço abstraindo toda a complexidade

**🎉 Pronto! As credenciais estão seguras!**

