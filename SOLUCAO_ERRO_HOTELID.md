# 🔧 Solução do Erro: Unknown column 'u.HotelId1'

## ❌ Erro

```
MySqlConnector.MySqlException: 'Unknown column 'u.HotelId1' in 'field list'
```

**Onde ocorre:**
```csharp
public async Task<User?> GetByEmailAsync(string email)
{
    return await _context.Users
        .Include(u => u.Hotel)  // ← Tentando incluir Hotel aqui
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email == email);
}
```

---

## 🔍 Causa do Erro

O erro ocorre porque:

1. ✅ Adicionamos o campo `HotelId` na entidade `User` (código C#)
2. ✅ Configuramos o relacionamento no `ApplicationDbContext`
3. ❌ **MAS a coluna ainda não existe no banco de dados MySQL**

Quando o EF Core tenta fazer `.Include(u => u.Hotel)`, ele procura pela coluna `HotelId` na tabela `Users`, mas ela não existe, então ele tenta criar uma shadow property `HotelId1`, o que também falha.

---

## ✅ Solução: Criar e Aplicar Migration

### Opção 1: Via Package Manager Console (Visual Studio)

```powershell
# 1. Criar a migration
Add-Migration AddHotelIdToUser -Project src/AvenSuites-Api.Infrastructure -StartupProject src/AvenSuites-Api

# 2. Aplicar no banco
Update-Database -Project src/AvenSuites-Api.Infrastructure -StartupProject src/AvenSuites-Api
```

### Opção 2: Via dotnet CLI (Terminal)

```bash
# 1. Criar a migration
dotnet ef migrations add AddHotelIdToUser --project src/AvenSuites-Api.Infrastructure/AvenSuites-Api.Infrastructure.csproj --startup-project src/AvenSuites-Api/AvenSuites-Api.csproj

# 2. Aplicar no banco
dotnet ef database update --project src/AvenSuites-Api.Infrastructure/AvenSuites-Api.Infrastructure.csproj --startup-project src/AvenSuites-Api/AvenSuites-Api.csproj
```

---

## 📝 O que a Migration vai fazer

A migration vai gerar SQL para:

```sql
-- 1. Adicionar a coluna HotelId na tabela Users
ALTER TABLE `Users` ADD `HotelId` char(36) NULL;

-- 2. Criar o índice da foreign key
CREATE INDEX `IX_Users_HotelId` ON `Users` (`HotelId`);

-- 3. Adicionar a constraint da foreign key
ALTER TABLE `Users` 
ADD CONSTRAINT `FK_Users_Hotels_HotelId` 
FOREIGN KEY (`HotelId`) 
REFERENCES `Hotels` (`Id`) 
ON DELETE SET NULL;

-- 4. Atualizar o usuário Gustavo com o HotelId
UPDATE `Users` 
SET `HotelId` = '7a326969-3bf6-40d9-96dc-1aecef585000' 
WHERE `Id` = 'f36d8acd-1822-4019-ac76-a6ea959d5193';
```

---

## ⚠️ Importante: Sobre os Roles

Você também criou o novo role **Hotel-Admin** no seed. A migration também vai incluir isso:

```sql
-- Inserir novo role Hotel-Admin
INSERT INTO `Roles` (`Id`, `Name`, `Description`, `CreatedAt`, `IsActive`)
VALUES (
    'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    'Hotel-Admin',
    'Hotel administrator role with access to specific hotel only',
    '2024-01-01 00:00:00',
    1
);

-- Atualizar UserRole do Gustavo para Hotel-Admin
UPDATE `UserRoles`
SET `RoleId` = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'
WHERE `UserId` = 'f36d8acd-1822-4019-ac76-a6ea959d5193';
```

---

## 🧪 Verificação Pós-Migration

Após aplicar a migration, verifique se funcionou:

### 1. No MySQL
```sql
-- Verificar se a coluna foi criada
DESCRIBE Users;

-- Verificar se o Gustavo tem HotelId
SELECT Id, Name, Email, HotelId FROM Users 
WHERE Email = 'gjose2980@gmail.com';
```

**Resultado esperado:**
```
Id: f36d8acd-1822-4019-ac76-a6ea959d5193
Name: Gustavo
Email: gjose2980@gmail.com
HotelId: 7a326969-3bf6-40d9-96dc-1aecef585000
```

### 2. Testar o Login

```bash
POST /api/Auth/login
{
  "email": "gjose2980@gmail.com",
  "password": "Admin123!"
}
```

**Token deve conter:**
```json
{
  "nameid": "f36d8acd-1822-4019-ac76-a6ea959d5193",
  "name": "Gustavo",
  "email": "gjose2980@gmail.com",
  "HotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "role": "Hotel-Admin"
}
```

---

## 🚨 Se der erro na Migration

### Erro: "Foreign key constraint fails"

Se você já tem dados na tabela `Users` (além do seed), pode dar erro ao criar a constraint.

**Solução:**
```sql
-- Atualizar users sem hotel para NULL
UPDATE Users SET HotelId = NULL WHERE HotelId = '';

-- Ou associar users existentes a algum hotel
UPDATE Users SET HotelId = '7a326969-3bf6-40d9-96dc-1aecef585000' 
WHERE Email = 'algum-email@example.com';
```

### Erro: "Duplicate entry"

Se o role Hotel-Admin já existe no banco:

**Solução:**
```sql
-- Ver roles existentes
SELECT * FROM Roles;

-- Se necessário, deletar duplicata
DELETE FROM Roles WHERE Name = 'Hotel-Admin' AND Id != 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';
```

---

## ✅ Checklist Final

Depois de aplicar a migration:

- [ ] Coluna `HotelId` existe na tabela `Users`
- [ ] Foreign key `FK_Users_Hotels_HotelId` foi criada
- [ ] Gustavo tem `HotelId = 7a326969-3bf6-40d9-96dc-1aecef585000`
- [ ] Role `Hotel-Admin` foi criado
- [ ] Gustavo tem role `Hotel-Admin` (não mais `Admin`)
- [ ] Login funciona e token contém `HotelId`
- [ ] Endpoint `GET /api/Hotel` retorna apenas Hotel Avenida para Gustavo

---

## 🔄 Caso queira reverter

Se algo der errado e você quiser voltar:

```powershell
# Reverter última migration
Update-Database -Migration NomeDaMigrationAnterior

# Ou via CLI
dotnet ef database update NomeDaMigrationAnterior --project src/AvenSuites-Api.Infrastructure --startup-project src/AvenSuites-Api
```

---

**Resolução**: Execute os comandos de migration acima e o erro será resolvido! 🎉

