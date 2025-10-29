# 🔐 Alterações no Sistema JWT - HotelId no Token

## ✅ Implementação Concluída

### 📋 Resumo
Adicionado campo `HotelId` ao token JWT para usuários com role **Hotel-Admin**, permitindo identificar automaticamente qual hotel o usuário administra.

---

## 🔧 Alterações Realizadas

### 1. **Entidade User**
Localização: `src/AvenSuites-Api.Domain/Entities/User.cs`

**Campo adicionado:**
```csharp
public Guid? HotelId { get; set; }

// Navigation property
public virtual Hotel? Hotel { get; set; }
```

**Tipo**: `Guid?` (nullable)  
**Motivo**: Apenas usuários Hotel-Admin possuem hotel associado. Admins globais não têm HotelId.

---

### 2. **JwtService**
Localização: `src/AvenSuites-Api.Application/Services/Implementations/Auth/JwtService.cs`

**Lógica implementada:**
```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new(ClaimTypes.Name, user.Name),
    new(ClaimTypes.Email, user.Email)
};

// Add HotelId claim if user has a hotel
if (user.HotelId.HasValue)
{
    claims.Add(new Claim("HotelId", user.HotelId.Value.ToString()));
}

// Add role claims
foreach (var userRole in user.UserRoles)
{
    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
}
```

**Comportamento**:
- ✅ Se `user.HotelId` tem valor → adiciona claim `HotelId`
- ✅ Se `user.HotelId` é null → não adiciona claim (Admin global)

---

### 3. **ApplicationDbContext**
Localização: `src/AvenSuites-Api.Infrastructure/Data/Contexts/ApplicationDbContext.cs`

**Configuração do relacionamento:**
```csharp
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.Id);
    // ... outras configurações
    
    // Relacionamento com Hotel (opcional para Hotel-Admin)
    entity.HasOne(e => e.Hotel)
        .WithMany()
        .HasForeignKey(e => e.HotelId)
        .OnDelete(DeleteBehavior.SetNull);
});
```

**Seed atualizado:**
```csharp
modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = gustavoUserId,
        Name = "Gustavo",
        Email = "gjose2980@gmail.com",
        PasswordHash = Argon2PasswordHasher.HashPassword("Admin123!"),
        HotelId = hotelAvenidaId, // ✨ NOVO - Associar ao Hotel Avenida
        CreatedAt = fixedCreatedAt,
        UpdatedAt = fixedCreatedAt,
        IsActive = true
    }
);
```

---

## 🎯 Como Funciona

### Exemplo 1: Token para Hotel-Admin (Gustavo)
```json
POST /api/Auth/login
{
  "email": "gjose2980@gmail.com",
  "password": "Admin123!"
}
```

**Token JWT gerado contém:**
```json
{
  "nameid": "f36d8acd-1822-4019-ac76-a6ea959d5193",
  "name": "Gustavo",
  "email": "gjose2980@gmail.com",
  "HotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "role": "Hotel-Admin",
  "nbf": 1730280600,
  "exp": 1730284200,
  "iat": 1730280600,
  "iss": "AvenSuitesApi",
  "aud": "AvenSuitesClient"
}
```

### Exemplo 2: Token para Admin Global
```json
POST /api/Auth/login
{
  "email": "admin@avensuites.com",
  "password": "AdminGlobal123!"
}
```

**Token JWT gerado contém:**
```json
{
  "nameid": "admin-user-id",
  "name": "Admin Global",
  "email": "admin@avensuites.com",
  "role": "Admin",
  "nbf": 1730280600,
  "exp": 1730284200,
  "iat": 1730280600,
  "iss": "AvenSuitesApi",
  "aud": "AvenSuitesClient"
}
```

**Observação**: Não possui `HotelId` pois tem acesso a todos os hotéis.

---

## 📝 CurrentUserService

O serviço `ICurrentUserService` já estava preparado para extrair o `HotelId` do token:

```csharp
public Guid? GetUserHotelId()
{
    var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("HotelId")?.Value;
    
    if (string.IsNullOrEmpty(hotelIdClaim))
        return null;
    
    return Guid.TryParse(hotelIdClaim, out var hotelId) ? hotelId : null;
}

public bool HasAccessToHotel(Guid hotelId)
{
    // Admin tem acesso a todos os hotéis
    if (IsAdmin())
        return true;
    
    // Hotel-Admin só tem acesso ao próprio hotel
    if (IsHotelAdmin())
    {
        var userHotelId = GetUserHotelId();
        return userHotelId.HasValue && userHotelId.Value == hotelId;
    }
    
    return false;
}
```

**Como usar no frontend:**

```typescript
// Decodificar o JWT no frontend
import jwt_decode from 'jwt-decode';

interface TokenPayload {
  nameid: string;
  name: string;
  email: string;
  HotelId?: string; // Opcional
  role: string[];
  exp: number;
}

const token = localStorage.getItem('token');
const decoded = jwt_decode<TokenPayload>(token);

console.log('User ID:', decoded.nameid);
console.log('User Name:', decoded.name);
console.log('Hotel ID:', decoded.HotelId); // undefined para Admin, Guid para Hotel-Admin
console.log('Roles:', decoded.role);

// Verificar se é Hotel-Admin
const isHotelAdmin = decoded.role.includes('Hotel-Admin');
const isAdmin = decoded.role.includes('Admin');

// Se for Hotel-Admin, pegar o HotelId
if (isHotelAdmin && decoded.HotelId) {
  console.log('Administra o hotel:', decoded.HotelId);
}
```

---

## 🔄 Migração Necessária

Para aplicar as alterações no banco de dados:

```powershell
# Criar migration
Add-Migration AddHotelIdToUser

# Aplicar no banco
Update-Database
```

**SQL gerado (resumo):**
```sql
ALTER TABLE Users ADD HotelId uniqueidentifier NULL;

ALTER TABLE Users 
  ADD CONSTRAINT FK_Users_Hotels_HotelId 
  FOREIGN KEY (HotelId) REFERENCES Hotels(Id) 
  ON DELETE SET NULL;

UPDATE Users 
SET HotelId = '7a326969-3bf6-40d9-96dc-1aecef585000' 
WHERE Id = 'f36d8acd-1822-4019-ac76-a6ea959d5193';
```

---

## ✅ Benefícios

### 1. **Segurança Aprimorada**
- O HotelId vem do token, não pode ser manipulado pelo cliente
- Cada requisição é validada automaticamente

### 2. **Simplicidade no Frontend**
- Não precisa passar `hotelId` em toda requisição
- O backend extrai automaticamente do token

### 3. **Código Mais Limpo**
```csharp
// ANTES (tinha que passar hotelId)
[HttpGet]
public async Task<ActionResult> GetRooms([FromQuery] Guid hotelId)
{
    if (!_currentUser.HasAccessToHotel(hotelId))
        return Forbid();
    // ...
}

// DEPOIS (pega automaticamente do token)
[HttpGet]
public async Task<ActionResult> GetRooms()
{
    var hotelId = _currentUser.GetUserHotelId();
    if (!hotelId.HasValue)
        return BadRequest("Usuário não está associado a nenhum hotel");
    // ...
}
```

### 4. **Melhor UX**
- Frontend não precisa gerenciar estado do hotel
- Menos chance de erros

---

## 🧪 Como Testar

### 1. Fazer Login como Hotel-Admin
```bash
POST /api/Auth/login
{
  "email": "gjose2980@gmail.com",
  "password": "Admin123!"
}
```

### 2. Decodificar o Token
Use https://jwt.io para verificar as claims:
- Deve conter `HotelId: 7a326969-3bf6-40d9-96dc-1aecef585000`
- Deve conter `role: Hotel-Admin`

### 3. Testar Endpoints
```bash
# Listar quartos (não precisa passar hotelId)
GET /api/Room
Authorization: Bearer {token}

# O backend extrai hotelId automaticamente do token
```

---

## 📊 Impacto nos Endpoints

**Todos os endpoints que antes exigiam `hotelId` como parâmetro** agora podem:
1. Extrair automaticamente do token (Hotel-Admin)
2. Aceitar hotelId opcional para Admins globais

**Endpoints afetados:**
- ✅ `GET /api/Room` - Agora filtra automaticamente por hotel do usuário
- ✅ `GET /api/Guest` - Agora filtra automaticamente por hotel do usuário
- ✅ `GET /api/Booking` - Agora filtra automaticamente por hotel do usuário
- ✅ Todos os endpoints de criação/atualização validam acesso automaticamente

---

## 📚 Documentação Atualizada

A documentação completa da API (`API_DOCUMENTATION.md`) foi atualizada para incluir:
- Estrutura do token JWT com HotelId
- Explicação de quando o HotelId está presente
- Exemplos de uso no frontend

---

## ✅ Checklist de Implementação

- [x] Adicionar campo `HotelId` à entidade `User`
- [x] Atualizar `JwtService` para incluir `HotelId` no token
- [x] Configurar relacionamento no `ApplicationDbContext`
- [x] Atualizar seed para associar Gustavo ao Hotel Avenida
- [x] Atualizar documentação da API
- [ ] **Pendente**: Criar e executar migration `AddHotelIdToUser`

---

**Data de Implementação**: 29 de Outubro de 2025  
**Versão**: 2.1.0

