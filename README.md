# 🏨 AvenSuites API

Sistema completo de gestão hoteleira desenvolvido em .NET 9, seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Módulos Principais](#módulos-principais)
- [Autenticação e Autorização](#autenticação-e-autorização)
- [Endpoints Principais](#endpoints-principais)
- [Banco de Dados](#banco-de-dados)
- [Testes](#testes)
- [Deploy](#deploy)

---

## 🎯 Sobre o Projeto

AvenSuites é uma API RESTful completa para gestão de hotéis, incluindo:

- ✅ Gestão de hotéis, quartos e tipos de quarto
- ✅ Sistema de reservas (bookings) com controle de disponibilidade
- ✅ Gestão de hóspedes com dados criptografados (LGPD)
- ✅ Sistema de faturamento e integração com NFSe
- ✅ Portal do hóspede
- ✅ Preços dinâmicos por ocupação
- ✅ Controle de disponibilidade dia a dia

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com separação clara de responsabilidades:

```
┌─────────────────────────────────────┐
│   Presentation (Controllers)        │
├─────────────────────────────────────┤
│   Application (Services, DTOs)      │
├─────────────────────────────────────┤
│   Domain (Entities, Interfaces)    │
├─────────────────────────────────────┤
│   Infrastructure (Repositories, DB) │
└─────────────────────────────────────┘
```

### 📁 Estrutura de Camadas

- **Domain**: Entidades de negócio, interfaces e regras de domínio
- **Application**: Serviços de aplicação, DTOs, lógica de negócio
- **Infrastructure**: Repositórios, acesso a dados, integrações externas
- **Presentation**: Controllers, middleware, configurações da API

### 🎯 Princípios Aplicados

- ✅ **SOLID**: Single Responsibility, Dependency Inversion
- ✅ **Repository Pattern**: Abstração de acesso a dados
- ✅ **DTO Pattern**: Separação entre entidades e modelos de apresentação
- ✅ **Dependency Injection**: Inversão de controle

---

## 🚀 Funcionalidades

### 🔐 Autenticação e Autorização
- Autenticação JWT Bearer
- Controle de acesso baseado em roles (Admin, Hotel-Admin, User, Guest)
- Hash de senhas com Argon2
- Validação de credenciais

### 🏨 Gestão de Hotéis
- CRUD completo de hotéis
- Gestão de usuários por hotel
- Configurações de timezone e localização

### 🛏️ Gestão de Quartos
- CRUD de quartos e tipos de quarto
- Sistema de preços por ocupação (1, 2, 3+ hóspedes)
- Controle de disponibilidade em tempo real
- Bloqueios de manutenção
- Amenidades por tipo de quarto

### 📅 Sistema de Reservas
- Criação de reservas com validação de disponibilidade
- Cálculo automático de preços baseado em ocupação
- Controle de conflitos dia a dia (BookingRoomNight)
- Status de reserva (PENDING, CONFIRMED, CANCELLED, CHECKED_IN, CHECKED_OUT)
- Cancelamento com liberação automática de quartos

### 👥 Gestão de Hóspedes
- CRUD de hóspedes
- Dados pessoais criptografados (LGPD compliance)
- Portal do hóspede para consulta de reservas
- Histórico de reservas por hóspede

### 💰 Faturamento
- Geração automática de notas fiscais
- Integração com sistema de NFSe (IPM)
- Histórico de faturas e emissões

### 📧 Notificações
- E-mails de confirmação de reserva
- E-mails de cancelamento
- Templates configuráveis

---

## 🛠️ Tecnologias

- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core** - ORM
- **MySQL 8.0+** - Banco de dados
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL
- **JWT Bearer Authentication** - Autenticação
- **Argon2** - Hash de senhas (implementação própria)
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Framework de testes

---

## 📋 Pré-requisitos

- .NET 9 SDK
- MySQL Server 8.0+ ou MariaDB 10.5+
- Visual Studio 2022, VS Code ou Rider
- Git

---

## 🔧 Configuração

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd AvenSuites-Api
```

### 2. Configure a string de conexão

Edite `src/AvenSuites-Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=root;Pwd=sua_senha;Port=3306;"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-super-longa-e-segura-aqui",
    "Issuer": "AvenSuites",
    "Audience": "AvenSuites",
    "ExpirationMinutes": 60
  }
}
```

### 3. Aplique as migrações

```bash
cd src/AvenSuites-Api.Infrastructure
dotnet ef database update --startup-project ../AvenSuites-Api
```

### 4. Execute o projeto

```bash
cd src/AvenSuites-Api
dotnet run
```

### 5. Acesse a documentação

Abra o navegador em: `https://localhost:5001/swagger` ou `http://localhost:5000/swagger`

---

## 📁 Estrutura do Projeto

```
AvenSuites-Api/
├── src/
│   ├── AvenSuites-Api/              # Camada de Apresentação
│   │   ├── Controllers/             # Controllers da API
│   │   │   ├── Auth/
│   │   │   ├── Bookings/
│   │   │   ├── Guests/
│   │   │   ├── Hotels/
│   │   │   ├── Invoices/
│   │   │   ├── Rooms/
│   │   │   └── Users/
│   │   ├── Middleware/              # Middleware customizado
│   │   ├── Workers/                 # Background workers
│   │   └── Program.cs
│   │
│   ├── AvenSuites-Api.Application/  # Camada de Aplicação
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   ├── Services/                # Serviços de aplicação
│   │   │   ├── Implementations/
│   │   │   └── Interfaces/
│   │   └── Utils/                   # Utilitários
│   │
│   ├── AvenSuites-Api.Domain/        # Camada de Domínio
│   │   ├── Entities/                # Entidades de negócio
│   │   └── Interfaces/              # Interfaces de repositórios
│   │
│   └── AvenSuites-Api.Infrastructure/# Camada de Infraestrutura
│       ├── Data/                    # Contextos de dados
│       ├── Migrations/              # Migrações do EF Core
│       └── Repositories/            # Implementações de repositórios
│
└── tests/                            # Testes unitários e de integração
```

---

## 🎯 Módulos Principais

### 🔐 Autenticação (`/api/auth`)
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/register` - Registro de novo usuário

### 🏨 Hotéis (`/api/hotels`)
- `GET /api/hotels` - Listar hotéis
- `GET /api/hotels/{id}` - Obter hotel por ID
- `POST /api/hotels` - Criar hotel
- `PUT /api/hotels/{id}` - Atualizar hotel

### 🛏️ Quartos (`/api/rooms`)
- `GET /api/rooms` - Listar quartos
- `GET /api/rooms/{id}` - Obter quarto por ID
- `GET /api/rooms/availability` - Verificar disponibilidade
- `POST /api/rooms` - Criar quarto
- `PUT /api/rooms/{id}` - Atualizar quarto

### 📋 Tipos de Quarto (`/api/roomtypes`)
- `GET /api/roomtypes` - Listar tipos de quarto
- `GET /api/roomtypes/{id}` - Obter tipo de quarto (com preços de ocupação)
- `POST /api/roomtypes` - Criar tipo de quarto
- `PUT /api/roomtypes/{id}` - Atualizar tipo de quarto

### 📅 Reservas (`/api/bookings`)
- `GET /api/bookings` - Listar reservas
- `GET /api/bookings/{id}` - Obter reserva por ID
- `POST /api/bookings` - Criar reserva (com cálculo automático de preço)
- `PUT /api/bookings/{id}` - Atualizar reserva
- `POST /api/bookings/{id}/cancel` - Cancelar reserva
- `POST /api/bookings/{id}/confirm` - Confirmar reserva
- `POST /api/bookings/{id}/check-in` - Realizar check-in
- `POST /api/bookings/{id}/check-out` - Realizar check-out

### 👥 Hóspedes (`/api/guests`)
- `GET /api/guests` - Listar hóspedes
- `GET /api/guests/{id}` - Obter hóspede por ID
- `POST /api/guests` - Criar hóspede
- `PUT /api/guests/{id}` - Atualizar hóspede

### 🌐 Portal do Hóspede (`/api/guest-portal`)
- `GET /api/guest-portal/bookings` - Reservas do hóspede logado
- `GET /api/guest-portal/bookings/{id}` - Detalhes da reserva
- `POST /api/guest-portal/bookings/{id}/cancel` - Cancelar própria reserva

### 💰 Faturas (`/api/invoices`)
- `GET /api/invoices` - Listar faturas
- `GET /api/invoices/{id}` - Obter fatura por ID
- `POST /api/invoices/generate/{bookingId}` - Gerar fatura para reserva

---

## 🔐 Autenticação e Autorização

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@avensuites.com",
  "password": "Admin@123!"
}
```

### Resposta

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-16T10:44:00Z",
  "user": {
    "id": "guid",
    "name": "Administrator",
    "email": "admin@avensuites.com",
    "roles": ["Admin"]
  }
}
```

### Usando o Token

```http
GET /api/bookings
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Roles Disponíveis

- **Admin**: Acesso total ao sistema
- **Hotel-Admin**: Acesso ao hotel específico
- **User**: Usuário padrão
- **Guest**: Hóspede (acesso ao portal)

---

## 💡 Funcionalidades Especiais

### 💰 Preços por Ocupação

O sistema permite definir preços diferentes baseados no número de hóspedes:

- **1 hóspede**: R$ 100/noite
- **2 hóspedes**: R$ 150/noite
- **3 hóspedes**: R$ 200/noite

O cálculo é feito automaticamente ao criar uma reserva.

### 📅 Controle de Disponibilidade

- Verificação dia a dia usando `BookingRoomNight`
- Impede reservas conflitantes
- Liberação automática ao cancelar
- Considera blocos de manutenção

### 🔒 Segurança e LGPD

- Dados pessoais (CPF, RG) criptografados com AES-GCM
- Hashes SHA256 para busca sem descriptografar
- Chaves de criptografia por hotel
- Rotação de chaves suportada

---

## 🗄️ Banco de Dados

### Principais Tabelas

- `hotels` - Hotéis cadastrados
- `users` - Usuários do sistema
- `guests` / `guest_pii` - Hóspedes e dados pessoais
- `room_types` - Tipos de quarto
- `room_type_occupancy_prices` - Preços por ocupação
- `rooms` - Quartos físicos
- `bookings` - Reservas
- `booking_rooms` - Quartos da reserva
- `booking_room_nights` - Noites individuais (controle de disponibilidade)
- `invoices` - Notas fiscais

### Migrações

As migrações são aplicadas automaticamente ou via:

```bash
dotnet ef migrations add NomeDaMigration --project src/AvenSuites-Api.Infrastructure --startup-project src/AvenSuites-Api
dotnet ef database update --project src/AvenSuites-Api.Infrastructure --startup-project src/AvenSuites-Api
```

---

## 🧪 Testes

O projeto inclui testes unitários e de integração:

```bash
dotnet test
```

### Estrutura de Testes

- `AvenSuites-Api.Domain.Tests` - Testes de entidades
- `AvenSuites-Api.Application.Tests` - Testes de serviços
- `AvenSuites-Api.Infrastructure.Tests` - Testes de repositórios
- `AvenSuites-Api.IntegrationTests` - Testes de integração

---

## 👤 Usuário Padrão

O sistema cria automaticamente um usuário administrador:

- **Email**: `admin@avensuites.com`
- **Senha**: `Admin@123!`
- **Role**: Admin

---

## 🚀 Deploy

### Produção

1. Configure a string de conexão para o MySQL de produção
2. Altere a chave JWT para uma chave segura e longa
3. Configure variáveis de ambiente apropriadas
4. Execute as migrações do banco de dados
5. Configure HTTPS e certificados SSL
6. Configure CORS para o domínio do frontend

### Variáveis de Ambiente Recomendadas

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<string-de-conexao>
Jwt__Key=<chave-secreta-jwt>
```

---

## 📝 Licença

Este projeto está sob a licença MIT.

---

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

## 📞 Suporte

Para dúvidas ou problemas, abra uma issue no repositório.

---

**Desenvolvido com ❤️ usando .NET 9**
