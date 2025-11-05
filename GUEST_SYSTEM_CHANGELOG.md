# 🔄 Sistema de Portal do Hóspede - Changelog

## 📅 Data: 31/10/2025

---

## ✨ Novas Funcionalidades

### 1. Sistema de Autenticação para Hóspedes

Os hóspedes agora podem criar contas próprias e acessar um portal exclusivo.

**Antes:**
- Apenas Admin e Hotel-Admin podiam acessar o sistema
- Hóspedes eram gerenciados apenas pelos administradores

**Depois:**
- Hóspedes podem se auto-registrar
- Acesso a um portal exclusivo com suas informações
- Controle total sobre seu perfil e reservas

---

## 🗃️ Mudanças no Banco de Dados

### Entidade `Guest`
**Adicionado:**
```csharp
public Guid? UserId { get; set; }           // Link para User
public virtual User? User { get; set; }      // Navegação
```

### Entidade `User`
**Adicionado:**
```csharp
public virtual Guest? Guest { get; set; }    // Navegação reversa
```

### Tabela `Roles`
**Novo registro:**
```sql
INSERT INTO Roles (Id, Name, Description, IsActive)
VALUES (
  'b2c3d4e5-f6a7-8901-bcde-f12345678901',
  'Guest',
  'Guest role for customers who can make reservations',
  1
);
```

---

## 🔐 Mudanças no JWT Token

### Claims Adicionadas

Quando um usuário com role "Guest" faz login, o token agora inclui:

```json
{
  "GuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441"
}
```

### Arquivo Modificado
- `src/AvenSuites-Api.Application/Services/Implementations/Auth/JwtService.cs`

```csharp
// Adicionar GuestId claim se user é guest
if (user.Guest != null)
{
    claims.Add(new Claim("GuestId", user.Guest.Id.ToString()));
}
```

---

## 🛠️ Novos Serviços

### 1. `IGuestRegistrationService` / `GuestRegistrationService`

**Localização:** `src/AvenSuites-Api.Application/Services/`

**Métodos:**
- `RegisterAsync(GuestRegisterRequest)` - Registra novo hóspede
- `GetProfileAsync(Guid guestId)` - Busca perfil do hóspede
- `UpdateProfileAsync(Guid guestId, GuestRegisterRequest)` - Atualiza perfil

**Funcionalidades:**
- Cria User + Guest + GuestPii em uma única transação
- Gera hash SHA256 para indexação de dados sensíveis
- Atribui automaticamente o role "Guest"
- Retorna token JWT já pronto para uso

---

### 2. Atualização no `ICurrentUserService`

**Novos Métodos:**
```csharp
Guid? GetUserGuestId();                    // Pega GuestId do token
bool IsGuest();                            // Verifica se é Guest
bool HasAccessToGuest(Guid guestId);       // Verifica acesso ao guest
```

**Localização:** `src/AvenSuites-Api.Application/Services/Implementations/CurrentUserService.cs`

---

## 📡 Novos Endpoints

### Controller: `AuthController`

#### `POST /api/Auth/register-guest`
- **Autenticação:** Público
- **Descrição:** Registro de novo hóspede
- **Request:** `GuestRegisterRequest`
- **Response:** `LoginResponse` (com token)

---

### Novo Controller: `GuestPortalController`

**Localização:** `src/AvenSuites-Api/Controllers/GuestPortal/GuestPortalController.cs`

**Autorização:** `[Authorize(Roles = "Guest")]`

#### Endpoints:

1. `GET /api/GuestPortal/profile`
   - Retorna perfil do hóspede logado

2. `PUT /api/GuestPortal/profile`
   - Atualiza perfil do hóspede logado

3. `GET /api/GuestPortal/bookings`
   - Lista todas as reservas do hóspede

4. `GET /api/GuestPortal/bookings/{id}`
   - Detalhes de uma reserva específica
   - Valida se a reserva pertence ao hóspede

5. `POST /api/GuestPortal/bookings/{id}/cancel`
   - Cancela uma reserva
   - Valida se a reserva pertence ao hóspede

---

## 📄 Novos DTOs

### Localização: `src/AvenSuites-Api.Application/DTOs/Guest/`

1. **`GuestRegisterRequest`**
   - Campos completos para registro de hóspede
   - Inclui dados pessoais, endereço e documento

2. **`GuestProfileResponse`**
   - Resposta com perfil completo do hóspede
   - Inclui nome do hotel

3. **`GuestBookingRequest`**
   - Request simplificado para criação de reserva por hóspede
   - (Em desenvolvimento)

---

## 🔧 Configurações

### DependencyInjection.cs

**Adicionado:**
```csharp
services.AddScoped<IGuestRegistrationService, GuestRegistrationService>();
```

### ApplicationDbContext.cs

**Configurações adicionadas:**

```csharp
// Guest configuration
modelBuilder.Entity<Guest>(entity =>
{
    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.SetNull);
});
```

**Seed data adicionado:**
- Role "Guest"

---

## 🔄 Migration Necessária

### Comando:
```powershell
Add-Migration AddGuestUserRelationship
Update-Database
```

### Alterações no Schema:

```sql
-- Adicionar coluna UserId na tabela Guests
ALTER TABLE Guests ADD UserId char(36) NULL;

-- Criar índice
CREATE INDEX IX_Guests_UserId ON Guests(UserId);

-- Adicionar constraint de foreign key
ALTER TABLE Guests 
ADD CONSTRAINT FK_Guests_Users_UserId 
FOREIGN KEY (UserId) 
REFERENCES Users(Id) 
ON DELETE SET NULL;

-- Inserir role Guest
INSERT INTO Roles (Id, Name, Description, CreatedAt, IsActive)
VALUES (
    'b2c3d4e5-f6a7-8901-bcde-f12345678901',
    'Guest',
    'Guest role for customers who can make reservations',
    '2024-01-01 00:00:00',
    1
);
```

---

## 📊 Fluxo de Registro de Hóspede

```
┌─────────────┐
│   Frontend  │
└──────┬──────┘
       │ POST /api/Auth/register-guest
       ▼
┌─────────────────────────────┐
│  GuestRegistrationService   │
├─────────────────────────────┤
│ 1. Validar email único      │
│ 2. Validar hotel existe     │
│ 3. Hash da senha (Argon2)   │
│ 4. Criar User               │
│ 5. Atribuir role "Guest"    │
│ 6. Criar Guest              │
│ 7. Criar GuestPii           │
│ 8. Gerar hash SHA256        │
│ 9. Gerar token JWT          │
└──────┬──────────────────────┘
       │
       ▼
┌─────────────────────────────┐
│   LoginResponse             │
├─────────────────────────────┤
│ - token (JWT)               │
│ - expiresAt                 │
│ - user (com role Guest)     │
└─────────────────────────────┘
```

---

## 🔐 Segurança Implementada

### 1. Hashing de Senha
- Utiliza **Argon2** para hash de senha
- Salt automático por usuário

### 2. Indexação de Dados Sensíveis
- Email, Telefone e Documento são hasheados com **SHA256**
- Permite busca sem expor dados originais

### 3. Validação de Propriedade
- Hóspedes só podem acessar suas próprias reservas
- Verificação feita via `GuestId` no token

### 4. Isolamento por Role
- Endpoint `GuestPortal` requer role "Guest"
- Admin e Hotel-Admin não acessam este portal

---

## 🧪 Testes Recomendados

### 1. Teste de Registro
```bash
POST /api/Auth/register-guest
# Verificar:
- ✅ Criação de User
- ✅ Criação de Guest
- ✅ Criação de GuestPii
- ✅ Atribuição de role Guest
- ✅ Token retornado com GuestId
```

### 2. Teste de Login
```bash
POST /api/Auth/login
# Verificar:
- ✅ Token contém GuestId
- ✅ Token contém role Guest
```

### 3. Teste de Acesso ao Portal
```bash
GET /api/GuestPortal/profile
# Verificar:
- ✅ 200 OK para Guest
- ✅ 403 Forbidden para Admin/Hotel-Admin
- ✅ 401 Unauthorized sem token
```

### 4. Teste de Isolamento de Dados
```bash
GET /api/GuestPortal/bookings/{outraReservaId}
# Verificar:
- ✅ 403 Forbidden se não for reserva do guest logado
```

---

## 📈 Melhorias Futuras (Sugestões)

### Fase 2 (Opcional)
1. **Criação de Reserva pelo Hóspede**
   - Endpoint para guest criar própria reserva
   - Seleção de quarto disponível
   - Cálculo automático de preço

2. **Reset de Senha**
   - Endpoint para solicitar reset
   - Email com link de reset

3. **Histórico de Atividades**
   - Log de ações do hóspede
   - Histórico de reservas antigas

4. **Preferências do Hóspede**
   - Salvar preferências (tipo de quarto, andar, etc)
   - Sugestões personalizadas

5. **Sistema de Notificações**
   - Email de boas-vindas
   - Lembrete de check-in
   - Confirmação de reserva

---

## 📝 Arquivos Modificados

### Domain
- ✅ `src/AvenSuites-Api.Domain/Entities/Guest.cs`
- ✅ `src/AvenSuites-Api.Domain/Entities/User.cs`

### Application
- ✅ `src/AvenSuites-Api.Application/DTOs/Guest/GuestRegisterRequest.cs` (NEW)
- ✅ `src/AvenSuites-Api.Application/DTOs/Guest/GuestProfileResponse.cs` (NEW)
- ✅ `src/AvenSuites-Api.Application/DTOs/Guest/GuestBookingRequest.cs` (NEW)
- ✅ `src/AvenSuites-Api.Application/Services/Interfaces/IGuestRegistrationService.cs` (NEW)
- ✅ `src/AvenSuites-Api.Application/Services/Implementations/Guest/GuestRegistrationService.cs` (NEW)
- ✅ `src/AvenSuites-Api.Application/Services/Interfaces/ICurrentUserService.cs`
- ✅ `src/AvenSuites-Api.Application/Services/Implementations/CurrentUserService.cs`
- ✅ `src/AvenSuites-Api.Application/Services/Implementations/Auth/JwtService.cs`
- ✅ `src/AvenSuites-Api.Application/DependencyInjection.cs`

### Infrastructure
- ✅ `src/AvenSuites-Api.Infrastructure/Data/Contexts/ApplicationDbContext.cs`
- ✅ `src/AvenSuites-Api.Infrastructure/Repositories/Implementations/UserRepository.cs`

### API
- ✅ `src/AvenSuites-Api/Controllers/Auth/AuthController.cs`
- ✅ `src/AvenSuites-Api/Controllers/GuestPortal/GuestPortalController.cs` (NEW)

---

## ✅ Checklist de Deploy

Antes de fazer deploy para produção:

- [ ] Criar e aplicar migration `AddGuestUserRelationship`
- [ ] Verificar se o role "Guest" foi criado
- [ ] Testar fluxo completo de registro
- [ ] Testar login de hóspede
- [ ] Testar acesso ao portal
- [ ] Testar isolamento de dados entre hóspedes
- [ ] Validar que Admin/Hotel-Admin não acessam GuestPortal
- [ ] Atualizar documentação da API (Swagger)
- [ ] Informar equipe de frontend sobre novos endpoints

---

## 🎯 Resumo

**Impacto:** BAIXO (não afeta funcionalidades existentes)  
**Breaking Changes:** NENHUM  
**Novas Tabelas:** 0  
**Colunas Adicionadas:** 1 (Guests.UserId)  
**Novos Endpoints:** 6  
**Novos Roles:** 1 (Guest)

---

**Versão:** 1.0  
**Autor:** Backend Team  
**Data:** 31/10/2025

