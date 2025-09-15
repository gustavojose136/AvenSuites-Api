# AvenSuites API - .NET 9 Web API

Uma API RESTful desenvolvida em .NET 9 seguindo os padrões DDD (Domain-Driven Design) e MVC, com autenticação JWT e banco de dados MySQL.

## 🏗️ Arquitetura

O projeto segue a arquitetura DDD com organização clara e separação de responsabilidades:

### 📁 **Estrutura Organizada**
- **Domain**: Entidades, interfaces e regras de negócio
- **Application**: Serviços organizados por funcionalidade (Auth, Users), DTOs e utilitários
- **Infrastructure**: Repositórios implementados, contextos de dados e configurações
- **Presentation**: Controllers organizados por módulo (Auth, Users) e configurações

### 🎯 **Benefícios da Organização**
- ✅ Separação clara de responsabilidades
- ✅ Estrutura escalável e manutenível
- ✅ Facilita localização de arquivos
- ✅ Padrões DDD aplicados corretamente

## 🚀 Funcionalidades

- ✅ Autenticação JWT
- ✅ Registro de usuários
- ✅ Login com validação de credenciais
- ✅ Controle de acesso baseado em roles (Admin/User)
- ✅ CORS configurado para frontend externo
- ✅ Swagger/OpenAPI para documentação
- ✅ Entity Framework Core com SQL Server
- ✅ Hash de senhas com Argon2

## 🛠️ Tecnologias

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- JWT Bearer Authentication
- Argon2 para hash de senhas (implementação própria)
- Swagger/OpenAPI

## 📋 Pré-requisitos

- .NET 9 SDK
- MySQL Server 8.0+
- Visual Studio 2022 ou VS Code

## 🔧 Configuração

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd teste-api
   ```

2. **Configure a string de conexão**
   
   Edite o arquivo `src/AvenSuites-Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=root;Pwd=;Port=3306;"
     }
   }
   ```

3. **Execute o projeto**
   ```bash
   cd src/AvenSuites-Api
   dotnet run
   ```

4. **Acesse a documentação**
   
   Abra o navegador em: `/`

## 📁 Estrutura Detalhada do Projeto

Para uma visão completa da organização do projeto, consulte o arquivo [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) que contém:

- 📋 Estrutura detalhada de todas as pastas
- 🔄 Fluxo de dependências entre camadas
- 📝 Benefícios da organização implementada
- 🚀 Guia para adicionar novas funcionalidades

## 🔐 Autenticação

### Registro de Usuário
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "MinhaSenh@123",
  "confirmPassword": "MinhaSenh@123"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@email.com",
  "password": "MinhaSenh@123"
}
```

### Resposta do Login
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-16T10:44:00Z",
  "user": {
    "id": "guid-do-usuario",
    "name": "João Silva",
    "email": "joao@email.com",
    "roles": ["User"]
  }
}
```

### Usando o Token
```http
GET /api/users/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📚 Endpoints Disponíveis

### Autenticação
- `POST /api/auth/login` - Login do usuário
- `POST /api/auth/register` - Registro de novo usuário
- `POST /api/auth/validate` - Validar credenciais

### Usuários
- `GET /api/users` - Listar usuários (Admin apenas)
- `GET /api/users/{id}` - Obter usuário por ID
- `GET /api/users/profile` - Obter perfil do usuário logado

## 👤 Usuário Padrão

O sistema cria automaticamente um usuário administrador:

- **Email**: admin@avensuites.com
- **Senha**: Admin123!
- **Role**: Admin

## 🔒 Segurança

- Senhas são hasheadas com Argon2 (algoritmo mais seguro que BCrypt) - implementação própria
- Tokens JWT com expiração configurável
- CORS configurado para frontend externo
- Validação de entrada com Data Annotations
- Controle de acesso baseado em roles

## 🗄️ Banco de Dados

O Entity Framework Core cria automaticamente o banco de dados e as tabelas na primeira execução. As tabelas criadas são:

- `Users` - Usuários do sistema
- `Roles` - Roles/perfis de acesso
- `UserRoles` - Relacionamento usuário-role

## 🚀 Deploy

Para fazer deploy em produção:

1. Configure a string de conexão para o MySQL de produção
2. Altere a chave JWT para uma chave segura
3. Configure as variáveis de ambiente apropriadas
4. Execute as migrações do banco de dados

## 📝 Licença

Este projeto está sob a licença MIT.
