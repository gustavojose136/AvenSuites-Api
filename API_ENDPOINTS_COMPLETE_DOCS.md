# 📚 AvenSuites API - Documentação Completa de Endpoints

## 📋 Índice

1. [Autenticação](#-1-autenticação)
2. [Hotéis](#-2-hotéis)
3. [Quartos](#-3-quartos)
4. [Tipos de Quarto](#-4-tipos-de-quarto)
5. [Hóspedes](#-5-hóspedes)
6. [Reservas](#-6-reservas)
7. [Notas Fiscais](#-7-notas-fiscais)
8. [Usuários](#-8-usuários)
9. [Portal do Hóspede](#-9-portal-do-hóspede)

---

## 🔐 Roles e Permissões

| Role | Descrição | Acesso |
|------|-----------|--------|
| **Admin** | Administrador global | Acesso total a todos os hotéis e recursos |
| **Hotel-Admin** | Administrador de hotel | Acesso apenas ao hotel atribuído |
| **Guest** | Hóspede | Acesso apenas aos próprios dados e reservas |
| **User** | Usuário padrão | Permissões básicas |

---

## 🌐 Base URL

- **Produção:** `https://api.avensuites.com`
- **Desenvolvimento:** `https://localhost:7000`

---

## 🔑 1. Autenticação

### 1.1 Login

**Endpoint:** `POST /api/Auth/login`  
**Autenticação:** ❌ Público

**Request Body:**
```json
{
  "email": "usuario@email.com",
  "password": "senha123"
}
```

**Response 200 OK:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-01T12:00:00Z",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Nome do Usuário",
    "email": "usuario@email.com",
    "roles": ["Admin"]
  }
}
```

**Response 401 Unauthorized:**
```json
{
  "message": "Email ou senha inválidos"
}
```

---

### 1.2 Registrar Admin/Staff

**Endpoint:** `POST /api/Auth/register`  
**Autenticação:** ❌ Público

**Request Body:**
```json
{
  "name": "Nome Completo",
  "email": "novo@email.com",
  "password": "senha123",
  "roleId": "60ccaec1-6c42-4fb5-a104-2036b42585a3"
}
```

**Response 200 OK:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Nome Completo",
  "email": "novo@email.com",
  "roles": ["Admin"]
}
```

**Response 400 Bad Request:**
```json
{
  "message": "Email já está em uso"
}
```

---

### 1.3 Validar Credenciais

**Endpoint:** `POST /api/Auth/validate`  
**Autenticação:** ❌ Público

**Request Body:**
```json
{
  "email": "usuario@email.com",
  "password": "senha123"
}
```

**Response 200 OK:**
```json
{
  "message": "Credenciais válidas"
}
```

**Response 401 Unauthorized:**
```json
{
  "message": "Credenciais inválidas"
}
```

---

### 1.4 Registrar Hóspede (Self-Registration)

**Endpoint:** `POST /api/Auth/register-guest`  
**Autenticação:** ❌ Público

**Request Body:**
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "Senha123!",
  "phone": "+55 11 99999-9999",
  "documentType": "CPF",
  "document": "123.456.789-00",
  "birthDate": "1990-01-01T00:00:00",
  "addressLine1": "Rua Exemplo, 123",
  "addressLine2": "Apto 45",
  "city": "São Paulo",
  "neighborhood": "Centro",
  "state": "SP",
  "postalCode": "01234-567",
  "countryCode": "BR",
  "marketingConsent": true,
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000"
}
```

**Response 200 OK:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-01T12:00:00Z",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "João Silva",
    "email": "joao@email.com",
    "roles": ["Guest"]
  }
}
```

---

## 🏨 2. Hotéis

### 2.1 Listar Hotéis

**Endpoint:** `GET /api/Hotels`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Comportamento:**
- **Admin:** vê todos os hotéis
- **Hotel-Admin:** vê apenas o próprio hotel

**Response 200 OK:**
```json
[
  {
    "id": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "name": "Hotel Avenida",
    "brandName": "Avenida Hotels",
    "cnpj": "83.630.657/0001-60",
    "email": "contato@hotelavenida.com",
    "phone": "+55 47 3433-0000",
    "addressLine1": "Rua XV de Novembro, 777",
    "city": "Joinville",
    "state": "SC",
    "postalCode": "89201-600",
    "countryCode": "BR",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 2.2 Buscar Hotel por ID

**Endpoint:** `GET /api/Hotels/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (mesmo formato do item acima)

**Response 403 Forbidden:**
```json
{
  "message": "Acesso negado"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Hotel não encontrado"
}
```

---

### 2.3 Buscar Hotel por CNPJ

**Endpoint:** `GET /api/Hotels/cnpj/{cnpj}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Exemplo:** `GET /api/Hotels/cnpj/83.630.657/0001-60`

**Response 200 OK:** (mesmo formato do 2.1)

---

### 2.4 Criar Hotel

**Endpoint:** `POST /api/Hotels`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin` (apenas)

**Request Body:**
```json
{
  "name": "Hotel Novo",
  "brandName": "Rede Hotels",
  "cnpj": "12.345.678/0001-90",
  "email": "contato@novoh hotel.com",
  "phone": "+55 47 3000-0000",
  "addressLine1": "Rua Principal, 100",
  "addressLine2": "Sala 201",
  "city": "Joinville",
  "state": "SC",
  "postalCode": "89200-000",
  "countryCode": "BR"
}
```

**Response 201 Created:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Hotel Novo",
  // ... outros campos
}
```

---

### 2.5 Atualizar Hotel

**Endpoint:** `PUT /api/Hotels/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin` (com acesso ao hotel)

**Request Body:** (mesmo formato do 2.4)

**Response 200 OK:** (hotel atualizado)

---

### 2.6 Deletar Hotel

**Endpoint:** `DELETE /api/Hotels/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin` (apenas)

**Response 204 No Content**

---

## 🛏️ 3. Quartos

### 3.1 Listar Quartos

**Endpoint:** `GET /api/Rooms`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (required para Admin): ID do hotel
- `status` (optional): Status do quarto (AVAILABLE, OCCUPIED, CLEANING, MAINTENANCE, OUT_OF_SERVICE)

**Exemplo:** `GET /api/Rooms?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000&status=AVAILABLE`

**Response 200 OK:**
```json
[
  {
    "id": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "roomNumber": "101",
    "floor": 1,
    "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
    "status": "AVAILABLE",
    "cleaningPriority": 0,
    "maxOccupancy": 2,
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 3.2 Buscar Quarto por ID

**Endpoint:** `GET /api/Rooms/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (mesmo formato do 3.1)

---

### 3.3 Listar Quartos por Hotel

**Endpoint:** `GET /api/Rooms/hotel/{hotelId}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `status` (optional): Filtrar por status

**Response 200 OK:** (array de quartos)

---

### 3.4 Verificar Disponibilidade

**Endpoint:** `GET /api/Rooms/availability`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (required)
- `checkIn` (required): Data de check-in (formato: `2025-11-10T14:00:00Z`)
- `checkOut` (required): Data de check-out
- `adults` (optional): Número de adultos
- `children` (optional): Número de crianças

**Exemplo:** `GET /api/Rooms/availability?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000&checkIn=2025-11-10T14:00:00Z&checkOut=2025-11-15T12:00:00Z&adults=2`

**Response 200 OK:**
```json
[
  {
    "roomId": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
    "roomNumber": "101",
    "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
    "roomTypeName": "Standard",
    "available": true,
    "maxOccupancy": 2
  }
]
```

---

### 3.5 Criar Quarto

**Endpoint:** `POST /api/Rooms`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "roomNumber": "205",
  "floor": 2,
  "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
  "status": "AVAILABLE",
  "maxOccupancy": 3
}
```

**Response 201 Created:** (quarto criado)

---

### 3.6 Atualizar Quarto

**Endpoint:** `PUT /api/Rooms/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "roomNumber": "205A",
  "floor": 2,
  "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
  "status": "CLEANING",
  "maxOccupancy": 3
}
```

**Response 200 OK:** (quarto atualizado)

---

### 3.7 Deletar Quarto

**Endpoint:** `DELETE /api/Rooms/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 204 No Content**

---

## 🏷️ 4. Tipos de Quarto

### 4.1 Criar Tipo de Quarto

**Endpoint:** `POST /api/RoomTypes`  
**Autenticação:** ✅ Bearer Token

**Request Body:**
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "name": "Deluxe Suite",
  "description": "Suite luxuosa com vista para o mar",
  "maxOccupancy": 4,
  "defaultRatePerNight": 450.00,
  "amenities": "Ar condicionado, TV, Frigobar, Vista para o mar"
}
```

**Response 201 Created:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "name": "Deluxe Suite",
  // ... outros campos
}
```

---

### 4.2 Buscar Tipo de Quarto por ID

**Endpoint:** `GET /api/RoomTypes/{id}`  
**Autenticação:** ✅ Bearer Token

**Response 200 OK:** (mesmo formato do 4.1)

---

### 4.3 Listar Tipos de Quarto por Hotel

**Endpoint:** `GET /api/RoomTypes/hotel/{hotelId}`  
**Autenticação:** ✅ Bearer Token

**Query Parameters:**
- `activeOnly` (optional, default: true): Mostrar apenas ativos

**Exemplo:** `GET /api/RoomTypes/hotel/7a326969-3bf6-40d9-96dc-1aecef585000?activeOnly=true`

**Response 200 OK:** (array de tipos de quarto)

---

### 4.4 Atualizar Tipo de Quarto

**Endpoint:** `PUT /api/RoomTypes/{id}`  
**Autenticação:** ✅ Bearer Token

**Request Body:** (mesmo formato do 4.1)

**Response 200 OK:** (tipo de quarto atualizado)

---

### 4.5 Deletar Tipo de Quarto

**Endpoint:** `DELETE /api/RoomTypes/{id}`  
**Autenticação:** ✅ Bearer Token

**Response 204 No Content**

---

## 👤 5. Hóspedes

### 5.1 Listar Hóspedes

**Endpoint:** `GET /api/Guests`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (required para Admin): ID do hotel

**Response 200 OK:**
```json
[
  {
    "id": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "fullName": "Joni Cardoso",
    "email": null,
    "phone": null,
    "documentType": "CPF",
    "document": "791.300.709-53",
    "marketingConsent": false,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

### 5.2 Buscar Hóspede por ID

**Endpoint:** `GET /api/Guests/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (mesmo formato do 5.1)

---

### 5.3 Listar Hóspedes por Hotel

**Endpoint:** `GET /api/Guests/hotel/{hotelId}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (array de hóspedes)

---

### 5.4 Criar Hóspede

**Endpoint:** `POST /api/Guests`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "fullName": "Maria Silva",
  "email": "maria@email.com",
  "phone": "+55 11 98888-8888",
  "documentType": "CPF",
  "document": "987.654.321-00",
  "birthDate": "1985-05-15T00:00:00Z",
  "addressLine1": "Rua das Flores, 456",
  "city": "São Paulo",
  "neighborhood": "Jardins",
  "state": "SP",
  "postalCode": "01234-567",
  "countryCode": "BR",
  "marketingConsent": true
}
```

**Response 201 Created:** (hóspede criado)

---

### 5.5 Atualizar Hóspede

**Endpoint:** `PUT /api/Guests/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:** (mesmo formato do 5.4)

**Response 200 OK:** (hóspede atualizado)

---

### 5.6 Deletar Hóspede

**Endpoint:** `DELETE /api/Guests/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 204 No Content**

---

## 📅 6. Reservas

### 6.1 Listar Reservas

**Endpoint:** `GET /api/Bookings`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (optional): Filtrar por hotel
- `guestId` (optional): Filtrar por hóspede
- `startDate` (optional): Data inicial
- `endDate` (optional): Data final

**Exemplo:** `GET /api/Bookings?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000&startDate=2025-11-01&endDate=2025-11-30`

**Response 200 OK:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "code": "BK-2025-001",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "checkInDate": "2025-11-10T14:00:00Z",
    "checkOutDate": "2025-11-15T12:00:00Z",
    "adults": 2,
    "children": 1,
    "status": "CONFIRMED",
    "totalAmount": 1500.00,
    "currency": "BRL",
    "source": "DIRECT",
    "notes": "Vista para o mar",
    "createdAt": "2025-10-31T10:00:00Z",
    "updatedAt": "2025-10-31T10:00:00Z"
  }
]
```

---

### 6.2 Buscar Reserva por ID

**Endpoint:** `GET /api/Bookings/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (mesmo formato do 6.1)

---

### 6.3 Buscar Reserva por Código

**Endpoint:** `GET /api/Bookings/code/{code}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (required)

**Exemplo:** `GET /api/Bookings/code/BK-2025-001?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000`

**Response 200 OK:** (mesmo formato do 6.1)

---

### 6.4 Listar Reservas por Hotel

**Endpoint:** `GET /api/Bookings/hotel/{hotelId}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `startDate` (optional)
- `endDate` (optional)

**Response 200 OK:** (array de reservas)

---

### 6.5 Listar Reservas por Hóspede

**Endpoint:** `GET /api/Bookings/guest/{guestId}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:** (array de reservas)

---

### 6.6 Criar Reserva

**Endpoint:** `POST /api/Bookings`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "code": "BK-2025-002",
  "source": "DIRECT",
  "checkInDate": "2025-12-01T14:00:00Z",
  "checkOutDate": "2025-12-05T12:00:00Z",
  "adults": 2,
  "children": 0,
  "currency": "BRL",
  "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "channelRef": null,
  "notes": "Pedido de quarto alto",
  "bookingRooms": [
    {
      "roomId": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
      "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
      "ratePlanId": null,
      "priceTotal": 1200.00,
      "notes": null
    }
  ],
  "additionalGuestIds": []
}
```

**Response 201 Created:** (reserva criada)

---

### 6.7 Atualizar Reserva

**Endpoint:** `PUT /api/Bookings/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "checkInDate": "2025-12-01T15:00:00Z",
  "checkOutDate": "2025-12-06T12:00:00Z",
  "adults": 3,
  "children": 1,
  "notes": "Atualizado: quarto alto e cama extra"
}
```

**Response 200 OK:** (reserva atualizada)

---

### 6.8 Cancelar Reserva

**Endpoint:** `POST /api/Bookings/{id}/cancel`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `reason` (optional): Motivo do cancelamento

**Exemplo:** `POST /api/Bookings/{id}/cancel?reason=Solicitação do hóspede`

**Response 200 OK:**
```json
{
  "message": "Reserva cancelada com sucesso."
}
```

---

### 6.9 Confirmar Reserva

**Endpoint:** `POST /api/Bookings/{id}/confirm`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:**
```json
{
  "message": "Reserva confirmada com sucesso."
}
```

---

### 6.10 Check-In

**Endpoint:** `POST /api/Bookings/{id}/check-in`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:**
```json
{
  "message": "Check-in realizado com sucesso",
  "booking": {
    // ... reserva atualizada com status CHECKED_IN
  }
}
```

---

### 6.11 Check-Out

**Endpoint:** `POST /api/Bookings/{id}/check-out`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Response 200 OK:**
```json
{
  "message": "Check-out realizado com sucesso",
  "booking": {
    // ... reserva atualizada com status CHECKED_OUT
  }
}
```

---

## 🧾 7. Notas Fiscais (IPM NF-e)

### 7.1 Criar Nota Fiscal Simplificada

**Endpoint:** `POST /api/Invoices/simple/{roomId}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "guestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "checkInDate": "2025-11-10T14:00:00Z",
  "days": 5,
  "adults": 2,
  "children": 1,
  "totalValue": 1500.00,
  "description": "Hospedagem Standard",
  "observations": "Estadia completa"
}
```

**Response 200 OK:**
```json
{
  "success": true,
  "nfseNumber": "12345",
  "serieNfse": "1",
  "verificationCode": "ABCD1234",
  "xmlContent": "<?xml version=\"1.0\"...",
  "pdfContent": null,
  "rawResponse": "<xml>...</xml>"
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "errorMessage": "Credenciais IPM não configuradas para este hotel"
}
```

---

### 7.2 Criar Nota Fiscal Completa

**Endpoint:** `POST /api/Invoices`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Query Parameters:**
- `hotelId` (required)

**Request Body:**
```json
{
  "bookingId": "123e4567-e89b-12d3-a456-426614174000",
  "identifier": "unique-id-123",
  "serieNfse": "1",
  "fatoGeradorDate": "2025-11-10T14:00:00Z",
  "totalValue": 1500.00,
  "observations": "Nota fiscal de hospedagem",
  "tomadorGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "items": [
    {
      "tributaMunicipioPrestador": "N",
      "codigoLocalPrestacao": "8319",
      "unidadeCodigo": "1",
      "unidadeQuantidade": 5,
      "unidadeValorUnitario": 300.00,
      "codigoItemListaServico": "901",
      "descritivo": "Hospedagem - Quarto 101",
      "aliquotaItemLista": 2.01,
      "situacaoTributaria": "0",
      "valorTributavel": 1500.00
    }
  ]
}
```

**Response 200 OK:** (mesmo formato do 7.1)

---

### 7.3 Cancelar Nota Fiscal

**Endpoint:** `POST /api/Invoices/{hotelId}/cancel`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Request Body:**
```json
{
  "nfseNumber": "12345",
  "serieNfse": "1",
  "motivo": "Cancelamento a pedido do cliente"
}
```

**Response 200 OK:**
```json
{
  "success": true,
  "rawResponse": "<xml>...</xml>"
}
```

---

### 7.4 Buscar Nota por Código de Verificação

**Endpoint:** `GET /api/Invoices/{hotelId}/verification/{verificationCode}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Exemplo:** `GET /api/Invoices/7a326969-3bf6-40d9-96dc-1aecef585000/verification/ABCD1234`

**Response 200 OK:**
```json
{
  "success": true,
  "nfseNumber": "12345",
  "serieNfse": "1",
  "verificationCode": "ABCD1234",
  "xmlContent": "<?xml version=\"1.0\"...",
  "rawResponse": "<xml>...</xml>"
}
```

---

### 7.5 Buscar Nota por Número e Série

**Endpoint:** `GET /api/Invoices/{hotelId}/number/{nfseNumber}/serie/{serie}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin`, `Hotel-Admin`

**Exemplo:** `GET /api/Invoices/7a326969-3bf6-40d9-96dc-1aecef585000/number/12345/serie/1`

**Response 200 OK:** (mesmo formato do 7.4)

---

## 👥 8. Usuários

### 8.1 Listar Usuários

**Endpoint:** `GET /api/Users`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin` (apenas)

**Response 200 OK:**
```json
[
  {
    "id": "2975cf19-0baa-4507-9f98-968760deb546",
    "name": "Admin User",
    "email": "admin@avensuites.com",
    "roles": ["Admin"]
  },
  {
    "id": "f36d8acd-1822-4019-ac76-a6ea959d5193",
    "name": "Gustavo",
    "email": "gjose2980@gmail.com",
    "roles": ["Hotel-Admin"]
  }
]
```

---

### 8.2 Buscar Usuário por ID

**Endpoint:** `GET /api/Users/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Admin` ou o próprio usuário

**Response 200 OK:**
```json
{
  "id": "2975cf19-0baa-4507-9f98-968760deb546",
  "name": "Admin User",
  "email": "admin@avensuites.com",
  "roles": ["Admin"]
}
```

**Response 403 Forbidden:** (se tentar acessar outro usuário sem ser Admin)

---

### 8.3 Ver Perfil do Usuário Logado

**Endpoint:** `GET /api/Users/profile`  
**Autenticação:** ✅ Bearer Token

**Response 200 OK:**
```json
{
  "id": "2975cf19-0baa-4507-9f98-968760deb546",
  "name": "Admin User",
  "email": "admin@avensuites.com",
  "roles": ["Admin"]
}
```

---

## 🏠 9. Portal do Hóspede

### 9.1 Ver Perfil

**Endpoint:** `GET /api/GuestPortal/profile`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Guest` (apenas)

**Response 200 OK:**
```json
{
  "id": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "hotelName": "Hotel Avenida",
  "name": "João Silva",
  "email": "joao@email.com",
  "phone": "+55 11 99999-9999",
  "documentType": "CPF",
  "document": "123.456.789-00",
  "birthDate": "1990-01-01T00:00:00Z",
  "addressLine1": "Rua Exemplo, 123",
  "addressLine2": "Apto 45",
  "city": "São Paulo",
  "neighborhood": "Centro",
  "state": "SP",
  "postalCode": "01234-567",
  "countryCode": "BR",
  "marketingConsent": true,
  "createdAt": "2025-10-31T10:00:00Z"
}
```

---

### 9.2 Atualizar Perfil

**Endpoint:** `PUT /api/GuestPortal/profile`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Guest` (apenas)

**Request Body:** (mesmo formato do registro de hóspede - 1.4)

**Response 200 OK:** (perfil atualizado, mesmo formato do 9.1)

---

### 9.3 Listar Minhas Reservas

**Endpoint:** `GET /api/GuestPortal/bookings`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Guest` (apenas)

**Response 200 OK:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "code": "BK-2025-001",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "checkInDate": "2025-11-10T14:00:00Z",
    "checkOutDate": "2025-11-15T12:00:00Z",
    "adults": 2,
    "children": 1,
    "status": "CONFIRMED",
    "totalAmount": 1500.00,
    "currency": "BRL"
  }
]
```

---

### 9.4 Ver Detalhes de Reserva

**Endpoint:** `GET /api/GuestPortal/bookings/{id}`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Guest` (apenas)

**Response 200 OK:** (mesmo formato do 9.3, item único)

**Response 403 Forbidden:** (se a reserva não pertencer ao hóspede logado)

---

### 9.5 Cancelar Minha Reserva

**Endpoint:** `POST /api/GuestPortal/bookings/{id}/cancel`  
**Autenticação:** ✅ Bearer Token  
**Roles:** `Guest` (apenas)

**Request Body (opcional):**
```json
"Motivo do cancelamento"
```

**Response 200 OK:**
```json
{
  "message": "Reserva cancelada com sucesso"
}
```

**Response 403 Forbidden:** (se a reserva não pertencer ao hóspede logado)

---

## 📊 Códigos de Status HTTP

| Código | Significado | Descrição |
|--------|-------------|-----------|
| 200 | OK | Sucesso |
| 201 | Created | Recurso criado com sucesso |
| 204 | No Content | Sucesso, sem conteúdo de resposta |
| 400 | Bad Request | Dados inválidos ou erro de validação |
| 401 | Unauthorized | Token inválido ou expirado |
| 403 | Forbidden | Sem permissão para acessar o recurso |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro no servidor |

---

## 🔐 Autenticação

Todos os endpoints marcados com ✅ requerem um token JWT no header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Estrutura do Token JWT:**
```json
{
  "nameid": "user-id",
  "name": "Nome do Usuário",
  "email": "usuario@email.com",
  "HotelId": "hotel-id",  // apenas para Hotel-Admin
  "GuestId": "guest-id",  // apenas para Guest
  "role": "Admin",
  "exp": 1730462400,
  "iss": "AvenSuites-Api",
  "aud": "AvenSuites-Client"
}
```

**Expiração:** 1 hora após emissão

---

## 📝 Exemplos de Uso

### Exemplo 1: Login e Listar Hotéis

```bash
# 1. Login
curl -X POST https://localhost:7000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@avensuites.com",
    "password": "Admin123!"
  }'

# Response:
# { "token": "eyJhbG...", "expiresAt": "...", "user": {...} }

# 2. Listar Hotéis (usando o token)
curl -X GET https://localhost:7000/api/Hotels \
  -H "Authorization: Bearer eyJhbG..."
```

---

### Exemplo 2: Criar Reserva

```bash
curl -X POST https://localhost:7000/api/Bookings \
  -H "Authorization: Bearer eyJhbG..." \
  -H "Content-Type: application/json" \
  -d '{
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
    "code": "BK-2025-010",
    "source": "DIRECT",
    "checkInDate": "2025-12-10T14:00:00Z",
    "checkOutDate": "2025-12-15T12:00:00Z",
    "adults": 2,
    "children": 0,
    "currency": "BRL",
    "mainGuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "bookingRooms": [
      {
        "roomId": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
        "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
        "priceTotal": 1500.00
      }
    ]
  }'
```

---

### Exemplo 3: Portal do Hóspede

```bash
# 1. Registrar como hóspede
curl -X POST https://localhost:7000/api/Auth/register-guest \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@email.com",
    "password": "Senha123!",
    "phone": "+55 11 99999-9999",
    "documentType": "CPF",
    "document": "123.456.789-00",
    "birthDate": "1990-01-01",
    "addressLine1": "Rua Teste, 123",
    "city": "São Paulo",
    "state": "SP",
    "postalCode": "01234-567",
    "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000"
  }'

# 2. Ver minhas reservas (usando token Guest)
curl -X GET https://localhost:7000/api/GuestPortal/bookings \
  -H "Authorization: Bearer {token_guest}"
```

---

## 🎯 Resumo de Endpoints por Módulo

| Módulo | Total de Endpoints | GET | POST | PUT | DELETE |
|--------|-------------------|-----|------|-----|--------|
| Autenticação | 4 | 0 | 4 | 0 | 0 |
| Hotéis | 6 | 3 | 1 | 1 | 1 |
| Quartos | 7 | 4 | 1 | 1 | 1 |
| Tipos de Quarto | 5 | 2 | 1 | 1 | 1 |
| Hóspedes | 6 | 3 | 1 | 1 | 1 |
| Reservas | 11 | 5 | 4 | 1 | 0 |
| Notas Fiscais | 5 | 2 | 3 | 0 | 0 |
| Usuários | 3 | 3 | 0 | 0 | 0 |
| Portal Hóspede | 5 | 2 | 1 | 1 | 0 |
| **TOTAL** | **52** | **24** | **17** | **7** | **4** |

---

**Versão:** 1.0  
**Data:** 31/10/2025  
**Backend API:** AvenSuites-Api v9.0  
**Framework:** .NET 9.0

