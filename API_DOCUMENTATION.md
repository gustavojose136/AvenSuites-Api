# 📚 Documentação da API - AvenSuites

## 🌐 Informações Gerais

**Base URL**: `https://api.avensuites.com` (ou `http://localhost:5000` em desenvolvimento)  
**Versão**: 2.0.0  
**Autenticação**: JWT Bearer Token

---

## 🔐 Autenticação

Todos os endpoints (exceto `/Auth/login` e `/Auth/register`) requerem autenticação via JWT Token.

### Header de Autenticação
```http
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json
```

---

## 📋 Índice

1. [Autenticação](#1-autenticação)
2. [Hotéis](#2-hotéis)
3. [Quartos](#3-quartos)
4. [Hóspedes](#4-hóspedes)
5. [Reservas](#5-reservas)
6. [Notas Fiscais](#6-notas-fiscais)
7. [Tipos de Quarto](#7-tipos-de-quarto)
8. [Usuários](#8-usuários)

---

## 1. Autenticação

### 1.1. Login

**Endpoint**: `POST /api/Auth/login`  
**Autenticação**: Não requerida  
**Roles**: Público

**Request Body**:
```json
{
  "email": "gjose2980@gmail.com",
  "password": "SenhaForte@123"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-10-30T10:30:00Z"
}
```

**Token JWT contém as seguintes claims**:
```json
{
  "nameid": "f36d8acd-1822-4019-ac76-a6ea959d5193",
  "name": "Gustavo",
  "email": "gjose2980@gmail.com",
  "HotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "role": "Hotel-Admin",
  "exp": 1730284200
}
```

**Observações**:
- `HotelId` está presente apenas para usuários com role `Hotel-Admin`
- Usuários com role `Admin` não possuem `HotelId` (têm acesso a todos os hotéis)

**Response (401 Unauthorized)**:
```json
{
  "message": "Email ou senha inválidos"
}
```

---

### 1.2. Registro

**Endpoint**: `POST /api/Auth/register`  
**Autenticação**: Não requerida  
**Roles**: Público

**Request Body**:
```json
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "SenhaForte@123"
}
```

**Response (200 OK)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "João Silva",
  "email": "joao@example.com"
}
```

**Response (400 Bad Request)**:
```json
{
  "message": "Email já está em uso"
}
```

---

## 2. Hotéis

### 2.1. Listar Hotéis

**Endpoint**: `GET /api/Hotel`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**: Nenhum

**Response (200 OK)** - Admin vê todos os hotéis:
```json
[
  {
    "id": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "name": "Hotel Avenida",
    "legalName": "Hotel Avenida LTDA",
    "cnpj": "83.630.657/0001-60",
    "stateRegistration": "123456789",
    "municipalRegistration": "987654321",
    "address": "Rua Principal, 123",
    "city": "Joinville",
    "state": "SC",
    "postalCode": "89230-000",
    "country": "Brasil",
    "phone": "+55 47 3433-2211",
    "email": "contato@hotelavenida.com.br",
    "website": "https://hotelavenida.com.br",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

**Response (200 OK)** - Hotel-Admin vê apenas o próprio hotel:
```json
[
  {
    "id": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "name": "Hotel Avenida",
    // ... mesma estrutura acima
  }
]
```

---

### 2.2. Buscar Hotel por ID

**Endpoint**: `GET /api/Hotel/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do hotel

**Response (200 OK)**:
```json
{
  "id": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "name": "Hotel Avenida",
  "legalName": "Hotel Avenida LTDA",
  "cnpj": "83.630.657/0001-60",
  "stateRegistration": "123456789",
  "municipalRegistration": "987654321",
  "address": "Rua Principal, 123",
  "city": "Joinville",
  "state": "SC",
  "postalCode": "89230-000",
  "country": "Brasil",
  "phone": "+55 47 3433-2211",
  "email": "contato@hotelavenida.com.br",
  "website": "https://hotelavenida.com.br",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

**Response (404 Not Found)**:
```json
{
  "message": "Hotel não encontrado"
}
```

**Response (403 Forbidden)**:
```json
{
  "message": "Acesso negado"
}
```

---

### 2.3. Buscar Hotel por CNPJ

**Endpoint**: `GET /api/Hotel/cnpj/{cnpj}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `cnpj` (string): CNPJ do hotel (com ou sem máscara)

**Response**: Mesma estrutura do endpoint 2.2

---

### 2.4. Criar Hotel

**Endpoint**: `POST /api/Hotel`  
**Autenticação**: Obrigatória  
**Roles**: Admin (apenas)

**Request Body**:
```json
{
  "name": "Grand Hotel",
  "legalName": "Grand Hotel S/A",
  "cnpj": "12.345.678/0001-90",
  "stateRegistration": "123456789",
  "municipalRegistration": "987654321",
  "address": "Av. Principal, 456",
  "city": "São Paulo",
  "state": "SP",
  "postalCode": "01234-567",
  "country": "Brasil",
  "phone": "+55 11 3333-4444",
  "email": "contato@grandhotel.com",
  "website": "https://grandhotel.com"
}
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Grand Hotel",
  // ... mesma estrutura com todos os campos
}
```

---

### 2.5. Atualizar Hotel

**Endpoint**: `PUT /api/Hotel/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do hotel

**Request Body**: Mesma estrutura do POST (2.4)

**Response (200 OK)**: Mesma estrutura do GET (2.2)

---

### 2.6. Deletar Hotel

**Endpoint**: `DELETE /api/Hotel/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin (apenas)

**Path Parameters**:
- `id` (UUID): ID do hotel

**Response (204 No Content)**: Sem corpo

**Response (404 Not Found)**:
```json
{
  "message": "Hotel não encontrado"
}
```

---

## 3. Quartos

### 3.1. Listar Quartos

**Endpoint**: `GET /api/Room`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, opcional): Filtrar por hotel
- `status` (string, opcional): Filtrar por status (ACTIVE, INACTIVE, MAINTENANCE, OCCUPIED, CLEANING)

**Response (200 OK)**:
```json
[
  {
    "id": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "roomNumber": "101",
    "floor": 1,
    "status": "ACTIVE",
    "maxOccupancy": 2,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  {
    "id": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "roomNumber": "102",
    "floor": 1,
    "status": "ACTIVE",
    "maxOccupancy": 3,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 3.2. Buscar Quarto por ID

**Endpoint**: `GET /api/Room/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do quarto

**Response (200 OK)**:
```json
{
  "id": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "roomNumber": "101",
  "floor": 1,
  "status": "ACTIVE",
  "maxOccupancy": 2,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

---

### 3.3. Listar Quartos por Hotel

**Endpoint**: `GET /api/Room/hotel/{hotelId}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel

**Query Parameters**:
- `status` (string, opcional): Filtrar por status

**Response (200 OK)**: Array de quartos (mesma estrutura do 3.1)

---

### 3.4. Verificar Disponibilidade

**Endpoint**: `GET /api/Room/availability`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, obrigatório): ID do hotel
- `checkInDate` (datetime, obrigatório): Data de check-in
- `checkOutDate` (datetime, obrigatório): Data de check-out
- `guests` (int, opcional): Número de hóspedes

**Response (200 OK)**:
```json
[
  {
    "roomId": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
    "roomNumber": "101",
    "floor": 1,
    "maxOccupancy": 2,
    "isAvailable": true,
    "pricePerNight": 250.00
  },
  {
    "roomId": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
    "roomNumber": "102",
    "floor": 1,
    "maxOccupancy": 3,
    "isAvailable": false,
    "reason": "Já reservado"
  }
]
```

---

### 3.5. Criar Quarto

**Endpoint**: `POST /api/Room`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Request Body**:
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "roomNumber": "201",
  "floor": 2,
  "status": "ACTIVE",
  "maxOccupancy": 2
}
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "roomNumber": "201",
  "floor": 2,
  "status": "ACTIVE",
  "maxOccupancy": 2,
  "createdAt": "2025-10-29T12:00:00Z",
  "updatedAt": "2025-10-29T12:00:00Z"
}
```

---

### 3.6. Atualizar Quarto

**Endpoint**: `PUT /api/Room/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do quarto

**Request Body**:
```json
{
  "roomNumber": "201-A",
  "floor": 2,
  "status": "MAINTENANCE",
  "maxOccupancy": 3
}
```

**Response (200 OK)**: Mesma estrutura do GET (3.2)

---

### 3.7. Deletar Quarto

**Endpoint**: `DELETE /api/Room/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do quarto

**Response (204 No Content)**: Sem corpo

---

## 4. Hóspedes

### 4.1. Listar Hóspedes

**Endpoint**: `GET /api/Guest`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, opcional): Filtrar por hotel

**Response (200 OK)**:
```json
[
  {
    "id": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "fullName": "Joni Cardoso",
    "email": "joni@example.com",
    "phone": "+55 47 99999-8888",
    "documentNumber": "12345678900",
    "birthDate": "1990-05-15T00:00:00Z",
    "address": "MONSENHOR GERCINO, S/N",
    "city": "Joinville",
    "state": "SC",
    "neighborhood": "JARIVATUBA",
    "postalCode": "89230-290",
    "country": "BR",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 4.2. Buscar Hóspede por ID

**Endpoint**: `GET /api/Guest/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do hóspede

**Response (200 OK)**: Mesma estrutura do 4.1 (objeto único)

---

### 4.3. Listar Hóspedes por Hotel

**Endpoint**: `GET /api/Guest/hotel/{hotelId}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel

**Response (200 OK)**: Array de hóspedes (mesma estrutura do 4.1)

---

### 4.4. Criar Hóspede

**Endpoint**: `POST /api/Guest`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Request Body**:
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "fullName": "Maria Santos",
  "email": "maria@example.com",
  "phone": "+55 47 98888-7777",
  "documentNumber": "98765432100",
  "birthDate": "1985-08-20",
  "address": "Rua das Flores, 456",
  "city": "Florianópolis",
  "state": "SC",
  "neighborhood": "Centro",
  "postalCode": "88000-000",
  "country": "BR"
}
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "fullName": "Maria Santos",
  // ... todos os campos
}
```

---

### 4.5. Atualizar Hóspede

**Endpoint**: `PUT /api/Guest/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do hóspede

**Request Body**: Mesma estrutura do POST (4.4)

**Response (200 OK)**: Mesma estrutura do GET (4.2)

---

### 4.6. Deletar Hóspede

**Endpoint**: `DELETE /api/Guest/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID do hóspede

**Response (204 No Content)**: Sem corpo

---

## 5. Reservas

### 5.1. Listar Reservas

**Endpoint**: `GET /api/Booking`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, opcional): Filtrar por hotel
- `guestId` (UUID, opcional): Filtrar por hóspede
- `startDate` (datetime, opcional): Data inicial
- `endDate` (datetime, opcional): Data final

**Response (200 OK)**:
```json
[
  {
    "id": "012e3456-e89b-12d3-a456-426614174003",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "code": "BKG-2024-00123",
    "status": "CONFIRMED",
    "source": "DIRECT",
    "checkInDate": "2024-02-01T14:00:00Z",
    "checkOutDate": "2024-02-05T12:00:00Z",
    "adults": 2,
    "children": 0,
    "currency": "BRL",
    "totalAmount": 1000.00,
    "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "channelRef": null,
    "notes": "Check-in antecipado solicitado",
    "createdAt": "2024-01-15T10:00:00Z",
    "updatedAt": "2024-01-15T10:30:00Z",
    "mainGuest": {
      "id": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
      "fullName": "Joni Cardoso",
      "email": "joni@example.com",
      "phone": "+55 47 99999-8888"
    },
    "bookingRooms": [
      {
        "id": "room-booking-1",
        "roomId": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
        "roomNumber": "101",
        "roomTypeName": "Standard",
        "priceTotal": 1000.00,
        "notes": null
      }
    ],
    "payments": [
      {
        "id": "payment-1",
        "method": "CREDIT_CARD",
        "status": "PAID",
        "amount": 1000.00,
        "currency": "BRL",
        "paidAt": "2024-01-16T10:00:00Z"
      }
    ]
  }
]
```

---

### 5.2. Buscar Reserva por ID

**Endpoint**: `GET /api/Booking/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Response (200 OK)**: Mesma estrutura do 5.1 (objeto único)

---

### 5.3. Buscar Reserva por Código

**Endpoint**: `GET /api/Booking/code/{code}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, obrigatório): ID do hotel

**Path Parameters**:
- `code` (string): Código da reserva

**Response (200 OK)**: Mesma estrutura do 5.1 (objeto único)

---

### 5.4. Listar Reservas por Hotel

**Endpoint**: `GET /api/Booking/hotel/{hotelId}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel

**Query Parameters**:
- `startDate` (datetime, opcional): Data inicial
- `endDate` (datetime, opcional): Data final

**Response (200 OK)**: Array de reservas (mesma estrutura do 5.1)

---

### 5.5. Listar Reservas por Hóspede

**Endpoint**: `GET /api/Booking/guest/{guestId}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `guestId` (UUID): ID do hóspede

**Response (200 OK)**: Array de reservas (mesma estrutura do 5.1)

---

### 5.6. Criar Reserva

**Endpoint**: `POST /api/Booking`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Request Body**:
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "code": "BKG-2024-00456",
  "source": "DIRECT",
  "checkInDate": "2024-03-10T14:00:00Z",
  "checkOutDate": "2024-03-15T12:00:00Z",
  "adults": 2,
  "children": 1,
  "currency": "BRL",
  "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "channelRef": null,
  "notes": "Aniversário de casamento",
  "bookingRooms": [
    {
      "roomId": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
      "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
      "ratePlanId": null,
      "priceTotal": 1250.00,
      "notes": "Vista para o mar"
    }
  ]
}
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "code": "BKG-2024-00456",
  "status": "PENDING",
  // ... todos os campos
}
```

---

### 5.7. Atualizar Reserva

**Endpoint**: `PUT /api/Booking/{id}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Request Body**:
```json
{
  "status": "CONFIRMED",
  "checkInDate": "2024-03-10T14:00:00Z",
  "checkOutDate": "2024-03-16T12:00:00Z",
  "adults": 3,
  "children": 0,
  "notes": "Hóspede adicional"
}
```

**Response (200 OK)**: Mesma estrutura do GET (5.2)

---

### 5.8. Cancelar Reserva

**Endpoint**: `POST /api/Booking/{id}/cancel`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Query Parameters**:
- `reason` (string, opcional): Motivo do cancelamento

**Response (200 OK)**:
```json
{
  "message": "Reserva cancelada com sucesso."
}
```

---

### 5.9. Confirmar Reserva

**Endpoint**: `POST /api/Booking/{id}/confirm`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Response (200 OK)**:
```json
{
  "message": "Reserva confirmada com sucesso."
}
```

---

### 5.10. Realizar Check-in

**Endpoint**: `POST /api/Booking/{id}/check-in`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Response (200 OK)**:
```json
{
  "message": "Check-in realizado com sucesso",
  "booking": {
    "id": "012e3456-e89b-12d3-a456-426614174003",
    "status": "CHECKED_IN",
    // ... todos os campos da reserva
  }
}
```

**Comportamento**:
- Altera status da reserva para `CHECKED_IN`
- Altera status dos quartos para `OCCUPIED`

---

### 5.11. Realizar Check-out

**Endpoint**: `POST /api/Booking/{id}/check-out`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `id` (UUID): ID da reserva

**Response (200 OK)**:
```json
{
  "message": "Check-out realizado com sucesso",
  "booking": {
    "id": "012e3456-e89b-12d3-a456-426614174003",
    "status": "CHECKED_OUT",
    // ... todos os campos da reserva
  }
}
```

**Comportamento**:
- Altera status da reserva para `CHECKED_OUT`
- Altera status dos quartos para `CLEANING`

---

## 6. Notas Fiscais

### 6.1. Criar NF-e Simplificada

**Endpoint**: `POST /api/Invoice/simple/{roomId}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `roomId` (UUID): ID do quarto

**Request Body**:
```json
{
  "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "checkInDate": "2024-02-01T14:00:00Z",
  "checkOutDate": "2024-02-05T12:00:00Z",
  "days": 4,
  "numberOfPeople": 2,
  "description": "Hospedagem - Quarto 101"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "NF-e gerada com sucesso",
  "invoiceId": "invoice-550e8400",
  "nfseNumber": "2281",
  "nfseSeries": "1",
  "verificationCode": "8319291025114036410836306572025107397702",
  "linkNfse": "https://saofranciscodosul.atende.net/autoatendimento/...",
  "issueDate": "2025-10-29T11:40:36Z",
  "xmlSent": "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>...",
  "xmlResponse": "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>..."
}
```

**Response (400 Bad Request)**:
```json
{
  "success": false,
  "message": "Erro ao gerar NF-e",
  "xmlResponse": "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>..."
}
```

---

### 6.2. Criar NF-e Completa

**Endpoint**: `POST /api/Invoice`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Query Parameters**:
- `hotelId` (UUID, obrigatório): ID do hotel

**Request Body**:
```json
{
  "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "bookingId": "012e3456-e89b-12d3-a456-426614174003",
  "fatoGeradorDate": "2024-02-05",
  "items": [
    {
      "tributaMunicipioPrestador": "S",
      "codigoLocalPrestacaoServico": "8319",
      "codigoItemListaServico": "09.01",
      "codigoAtividade": "6209100",
      "descritivo": "Hospedagem - 4 diárias",
      "aliquotaItemListaServico": 5.00,
      "situacaoTributaria": 0,
      "valorTributavel": 1000.00,
      "valorDeducao": 0.00,
      "valorDescontoIncondicional": 0.00,
      "valorIssrf": 50.00
    }
  ]
}
```

**Response (200 OK)**: Mesma estrutura do 6.1

---

### 6.3. Cancelar NF-e

**Endpoint**: `POST /api/Invoice/{hotelId}/cancel`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel

**Request Body**:
```json
{
  "nfseNumber": "2281",
  "nfseSeries": "1",
  "cancelReason": "Cancelamento a pedido do cliente"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "NF-e cancelada com sucesso",
  "xmlResponse": "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>..."
}
```

---

### 6.4. Buscar NF-e por Código de Verificação

**Endpoint**: `GET /api/Invoice/{hotelId}/verification/{verificationCode}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel
- `verificationCode` (string): Código de verificação da NF-e

**Response (200 OK)**:
```json
{
  "success": true,
  "nfseNumber": "2281",
  "nfseSeries": "1",
  "issueDate": "29/10/2025",
  "linkNfse": "https://saofranciscodosul.atende.net/...",
  "xmlResponse": "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>..."
}
```

---

### 6.5. Buscar NF-e por Número e Série

**Endpoint**: `GET /api/Invoice/{hotelId}/number/{nfseNumber}/serie/{serie}`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Path Parameters**:
- `hotelId` (UUID): ID do hotel
- `nfseNumber` (string): Número da NF-e
- `serie` (string): Série da NF-e

**Response (200 OK)**: Mesma estrutura do 6.4

---

## 7. Tipos de Quarto

### 7.1. Listar Tipos de Quarto

**Endpoint**: `GET /api/RoomType`  
**Autenticação**: Obrigatória  
**Roles**: Admin, Hotel-Admin

**Response (200 OK)**:
```json
[
  {
    "id": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "name": "Standard",
    "description": "Quarto standard com cama de casal",
    "maxOccupancy": 2,
    "basePrice": 250.00,
    "amenities": ["TV", "Ar-condicionado", "Wi-Fi"],
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

---

## 8. Usuários

### 8.1. Listar Usuários

**Endpoint**: `GET /api/Users`  
**Autenticação**: Obrigatória  
**Roles**: Admin

**Response (200 OK)**:
```json
[
  {
    "id": "f36d8acd-1822-4019-ac76-a6ea959d5193",
    "name": "Gustavo José",
    "email": "gjose2980@gmail.com",
    "isActive": true,
    "roles": ["Hotel-Admin"],
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

## 📊 Códigos de Status HTTP

| Código | Significado |
|--------|-------------|
| 200 | OK - Requisição bem-sucedida |
| 201 | Created - Recurso criado com sucesso |
| 204 | No Content - Sucesso sem corpo de resposta |
| 400 | Bad Request - Dados inválidos |
| 401 | Unauthorized - Token inválido ou ausente |
| 403 | Forbidden - Sem permissão para acessar o recurso |
| 404 | Not Found - Recurso não encontrado |
| 500 | Internal Server Error - Erro interno do servidor |
| 501 | Not Implemented - Funcionalidade não implementada |

---

## 🔑 Roles e Permissões

| Role | Descrição | Acesso |
|------|-----------|--------|
| **Admin** | Administrador global | Acesso total a todos os hotéis |
| **Hotel-Admin** | Administrador de hotel | Acesso apenas ao hotel associado |
| **User** | Usuário padrão | Acesso limitado (futuro) |

---

## 📝 Notas Importantes

### 1. Formatação de Datas
Todas as datas devem ser enviadas no formato ISO 8601:
```
2024-02-01T14:00:00Z
```

### 2. UUIDs
Todos os IDs são UUIDs no formato:
```
550e8400-e29b-41d4-a716-446655440000
```

### 3. Valores Monetários
Valores monetários são sempre `decimal` com 2 casas decimais:
```json
{
  "totalAmount": 1250.50
}
```

### 4. Status de Reservas
Possíveis valores:
- `PENDING` - Pendente
- `CONFIRMED` - Confirmada
- `CHECKED_IN` - Check-in realizado
- `CHECKED_OUT` - Check-out realizado
- `CANCELLED` - Cancelada
- `NO_SHOW` - Não compareceu

### 5. Status de Quartos
Possíveis valores:
- `ACTIVE` - Ativo
- `INACTIVE` - Inativo
- `MAINTENANCE` - Manutenção
- `OCCUPIED` - Ocupado
- `CLEANING` - Limpeza

### 6. Tratamento de Erros
Todos os erros seguem o formato padrão:
```json
{
  "message": "Mensagem descritiva do erro",
  "errors": {
    "campo1": ["Erro específico"],
    "campo2": ["Outro erro"]
  }
}
```

---

## 🧪 Exemplos de Uso (cURL)

### Login
```bash
curl -X POST https://api.avensuites.com/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "gjose2980@gmail.com",
    "password": "SenhaForte@123"
  }'
```

### Listar Hotéis
```bash
curl -X GET https://api.avensuites.com/api/Hotel \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

### Criar Reserva
```bash
curl -X POST https://api.avensuites.com/api/Booking \
  -H "Authorization: Bearer SEU_TOKEN_JWT" \
  -H "Content-Type: application/json" \
  -d '{
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "code": "BKG-2024-00789",
    "source": "DIRECT",
    "checkInDate": "2024-03-10T14:00:00Z",
    "checkOutDate": "2024-03-15T12:00:00Z",
    "adults": 2,
    "children": 0,
    "currency": "BRL",
    "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "bookingRooms": [{
      "roomId": "bd823cb6-d7a4-45ae-9853-66895ea593bb",
      "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
      "priceTotal": 1250.00
    }]
  }'
```

### Realizar Check-in
```bash
curl -X POST https://api.avensuites.com/api/Booking/{id}/check-in \
  -H "Authorization: Bearer SEU_TOKEN_JWT"
```

---

## 📞 Suporte

Para dúvidas ou problemas, entre em contato:
- **Email**: suporte@avensuites.com
- **GitHub**: https://github.com/avensuites/api

---

**Versão da Documentação**: 2.0.0  
**Última Atualização**: 29 de Outubro de 2025

