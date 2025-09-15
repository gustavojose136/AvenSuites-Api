# Estrutura do Projeto AvenSuites-Api

## 📁 Organização das Pastas

### 🏗️ **Estrutura Geral**
```
AvenSuites-Api/
├── src/
│   ├── AvenSuites-Api.Domain/           # Camada de Domínio
│   ├── AvenSuites-Api.Application/       # Camada de Aplicação
│   ├── AvenSuites-Api.Infrastructure/   # Camada de Infraestrutura
│   └── AvenSuites-Api/                  # Camada de Apresentação (API)
└── AvenSuites-Api.sln                   # Solution File
```

---

## 🎯 **AvenSuites-Api.Domain**
**Responsabilidade**: Entidades, interfaces e regras de negócio

```
AvenSuites-Api.Domain/
├── Entities/
│   ├── User.cs                          # Entidade Usuário
│   ├── Role.cs                          # Entidade Role/Perfil
│   └── UserRole.cs                      # Entidade de Relacionamento
└── Interfaces/
    ├── IUserRepository.cs               # Interface do Repositório de Usuários
    └── IRoleRepository.cs               # Interface do Repositório de Roles
```

---

## 🔧 **AvenSuites-Api.Application**
**Responsabilidade**: Casos de uso, serviços e DTOs

```
AvenSuites-Api.Application/
├── DTOs/
│   ├── LoginRequest.cs                  # DTO para Login
│   ├── LoginResponse.cs                 # DTO de Resposta do Login
│   └── RegisterRequest.cs               # DTO para Registro
├── Services/
│   ├── Interfaces/                      # Interfaces dos Serviços
│   │   ├── IAuthService.cs              # Interface do Serviço de Autenticação
│   │   └── IJwtService.cs               # Interface do Serviço JWT
│   └── Implementations/
│       └── Auth/                         # Implementações de Autenticação
│           ├── AuthService.cs            # Implementação do Serviço de Auth
│           └── JwtService.cs             # Implementação do Serviço JWT
├── Utils/
│   └── Argon2PasswordHasher.cs          # Utilitário para Hash de Senhas
└── DependencyInjection.cs               # Configuração de Dependências
```

---

## 🗄️ **AvenSuites-Api.Infrastructure**
**Responsabilidade**: Acesso a dados e implementações externas

```
AvenSuites-Api.Infrastructure/
├── Data/
│   ├── Contexts/
│   │   └── ApplicationDbContext.cs      # Contexto do Entity Framework
│   └── Configurations/                   # Configurações do EF (futuro)
├── Repositories/
│   └── Implementations/                  # Implementações dos Repositórios
│       ├── UserRepository.cs             # Repositório de Usuários
│       └── RoleRepository.cs             # Repositório de Roles
└── DependencyInjection.cs               # Configuração de Dependências
```

---

## 🌐 **AvenSuites-Api (Presentation)**
**Responsabilidade**: Controllers, configurações e startup

```
AvenSuites-Api/
├── Controllers/
│   ├── Auth/
│   │   └── AuthController.cs            # Controller de Autenticação
│   └── Users/
│       └── UsersController.cs           # Controller de Usuários
├── Properties/
│   └── launchSettings.json              # Configurações de Execução
├── appsettings.json                      # Configurações da Aplicação
├── appsettings.Development.json          # Configurações de Desenvolvimento
├── Program.cs                            # Ponto de Entrada da Aplicação
└── AvenSuites-Api.csproj                # Arquivo de Projeto
```

---

## 🔄 **Fluxo de Dependências**

```
Presentation Layer (API)
    ↓
Application Layer (Services, DTOs, Utils)
    ↓
Domain Layer (Entities, Interfaces)
    ↑
Infrastructure Layer (Repositories, Data Access)
```

---

## 📋 **Benefícios da Nova Estrutura**

### ✅ **Separação Clara de Responsabilidades**
- **Domain**: Apenas regras de negócio e entidades
- **Application**: Casos de uso e serviços
- **Infrastructure**: Acesso a dados e integrações
- **Presentation**: Controllers e configurações

### ✅ **Organização por Funcionalidade**
- **Auth/**: Tudo relacionado à autenticação
- **Users/**: Tudo relacionado aos usuários
- **Interfaces/**: Todas as interfaces separadas
- **Implementations/**: Implementações específicas

### ✅ **Facilita Manutenção**
- Fácil localização de arquivos
- Estrutura escalável
- Separação clara entre camadas
- Facilita testes unitários

### ✅ **Padrões DDD Aplicados**
- Domain-Driven Design
- Clean Architecture
- Dependency Inversion
- Separation of Concerns

---

## 🚀 **Como Usar**

1. **Adicionar nova funcionalidade**:
   - Domain: Criar entidade e interface
   - Application: Criar DTOs e serviços
   - Infrastructure: Implementar repositório
   - Presentation: Criar controller

2. **Estrutura de pastas**:
   - Sempre seguir a organização por funcionalidade
   - Separar interfaces das implementações
   - Utilitários em pasta específica

3. **Namespaces**:
   - Seguir a estrutura de pastas
   - Usar nomes descritivos
   - Manter consistência
