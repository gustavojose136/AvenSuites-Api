# 🔧 Troubleshooting: Erro 403 no Guest Portal

## ❌ Problema

Ao acessar endpoints do `GuestPortalController` (ex: `/api/GuestPortal/bookings`), você recebe:

```
403 Forbidden
```

---

## 🔍 Causas Possíveis

### 1. **Role "Guest" não existe no banco de dados**
### 2. **Usuário não fez login como Guest**
### 3. **Token JWT não contém o role "Guest"**
### 4. **Migration não foi executada**

---

## ✅ Soluções

### Solução 1: Verificar se a Migration foi executada

```bash
# Executar migrations
dotnet ef database update --project src/AvenSuites-Api.Infrastructure --startup-project src/AvenSuites-Api
```

Isso irá criar/atualizar o banco de dados com os roles, incluindo o "Guest".

---

### Solução 2: Verificar se o Role "Guest" existe

Execute esta query no banco de dados:

```sql
SELECT * FROM roles WHERE Name = 'Guest';
```

**Resultado esperado:**
```
Id: b2c3d4e5-f6a7-8901-bcde-f12345678901
Name: Guest
Description: Guest role for customers who can make reservations
```

Se não retornar nada, rode a migration novamente ou insira manualmente:

```sql
INSERT INTO roles (Id, Name, Description, CreatedAt, IsActive)
VALUES 
('b2c3d4e5-f6a7-8901-bcde-f12345678901', 'Guest', 'Guest role for customers who can make reservations', UTC_TIMESTAMP(), 1);
```

---

### Solução 3: Registrar um novo hóspede

Use o endpoint de registro de hóspede:

**Endpoint:** `POST /api/Auth/register-guest`

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao.teste@email.com",
  "password": "Senha123!",
  "phone": "+55 11 99999-9999",
  "documentType": "CPF",
  "document": "123.456.789-00",
  "birthDate": "1990-01-01T00:00:00Z",
  "addressLine1": "Rua Teste, 123",
  "city": "São Paulo",
  "state": "SP",
  "postalCode": "01234-567",
  "countryCode": "BR",
  "marketingConsent": true,
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000"
}
```

**Response esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-01T12:00:00Z",
  "user": {
    "id": "guid-do-usuario",
    "name": "João Silva",
    "email": "joao.teste@email.com",
    "roles": ["Guest"]  // ← DEVE conter "Guest"!
  }
}
```

---

### Solução 4: Verificar o Token JWT

Copie o token recebido e cole em https://jwt.io

**Verifique se contém:**
```json
{
  "nameid": "user-id",
  "name": "João Silva",
  "email": "joao.teste@email.com",
  "GuestId": "guest-id",
  "role": "Guest",  // ← DEVE ter isso!
  "exp": 1730462400,
  "iss": "AvenSuites-Api",
  "aud": "AvenSuites-Client"
}
```

**Se não tiver `"role": "Guest"`**, o problema está no registro. Refaça o registro.

---

### Solução 5: Fazer Login novamente

Se você já registrou, faça login:

**Endpoint:** `POST /api/Auth/login`

**Body:**
```json
{
  "email": "joao.teste@email.com",
  "password": "Senha123!"
}
```

---

### Solução 6: Verificar se o UserRole foi criado

Execute esta query:

```sql
SELECT ur.*, r.Name as RoleName, u.Email
FROM userroles ur
JOIN users u ON ur.UserId = u.Id
JOIN roles r ON ur.RoleId = r.Id
WHERE u.Email = 'joao.teste@email.com';
```

**Resultado esperado:**
```
UserId: guid-do-usuario
RoleId: b2c3d4e5-f6a7-8901-bcde-f12345678901
RoleName: Guest
Email: joao.teste@email.com
```

---

## 🧪 Teste Completo

### Passo 1: Registrar
```bash
curl -X POST http://localhost:7000/api/Auth/register-guest \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teste Guest",
    "email": "teste@guest.com",
    "password": "Senha123!",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "addressLine1": "Rua Teste, 123",
    "city": "São Paulo",
    "state": "SP",
    "postalCode": "01234-567",
    "countryCode": "BR"
  }'
```

### Passo 2: Salvar o token
```bash
TOKEN="cole-o-token-aqui"
```

### Passo 3: Testar o endpoint
```bash
curl -X GET http://localhost:7000/api/GuestPortal/bookings \
  -H "Authorization: Bearer $TOKEN"
```

**Resultado esperado:** `200 OK` com array de reservas (pode estar vazio `[]`)

---

## ❌ Erros Comuns

### Erro 1: "Role 'Guest' não encontrado"
**Causa:** Migration não foi executada  
**Solução:** Execute `dotnet ef database update`

### Erro 2: Token não contém role "Guest"
**Causa:** Usuário fez login com email/senha de Admin ou Hotel-Admin  
**Solução:** Registre-se como Guest usando `/api/Auth/register-guest`

### Erro 3: 401 Unauthorized
**Causa:** Token inválido ou expirado  
**Solução:** Faça login novamente para obter novo token

### Erro 4: 403 Forbidden
**Causa:** Token válido mas não tem role "Guest"  
**Solução:** Verifique o token em jwt.io e certifique-se de que tem `"role": "Guest"`

---

## 🔍 Debug Avançado

### Ver logs da aplicação

```bash
dotnet run --project src/AvenSuites-Api
```

Procure por erros relacionados a:
- JWT validation
- Authorization
- Role not found

### Habilitar logs detalhados

No `appsettings.Development.json`, adicione:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

---

## ✅ Checklist

- [ ] Migration executada (`dotnet ef database update`)
- [ ] Role "Guest" existe no banco de dados
- [ ] Usuário registrado via `/api/Auth/register-guest`
- [ ] Token JWT contém `"role": "Guest"`
- [ ] Token não expirou (válido por 1 hora)
- [ ] Header `Authorization: Bearer {token}` está correto
- [ ] Endpoint correto: `/api/GuestPortal/bookings`

---

## 📞 Ainda com problema?

Se depois de seguir todos os passos ainda tiver 403:

1. Limpe o banco de dados e rode as migrations do zero
2. Verifique se há algum middleware de autorização customizado
3. Verifique se o `Program.cs` tem `app.UseAuthentication()` ANTES de `app.UseAuthorization()`

---

**Data:** 31/10/2025  
**Status:** Solução testada e funcional

