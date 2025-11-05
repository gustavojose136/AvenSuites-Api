# 📮 AvenSuites API - Guia de Collection Postman/Insomnia

## 🎯 Setup Inicial

### 1. Variáveis de Ambiente

Crie um ambiente com as seguintes variáveis:

```json
{
  "baseUrl": "https://localhost:7000",
  "token": "",
  "adminEmail": "admin@avensuites.com",
  "adminPassword": "Admin123!",
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000",
  "guestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "roomId": "40d5718c-dbda-40c7-a4f4-644cd6f177bd",
  "bookingId": "",
  "userId": ""
}
```

---

## 📁 Estrutura de Pastas

```
AvenSuites API/
├── 🔐 Auth/
│   ├── Login Admin
│   ├── Login Guest
│   ├── Register Admin
│   ├── Register Guest
│   └── Validate Password
│
├── 🏨 Hotels/
│   ├── List Hotels
│   ├── Get Hotel by ID
│   ├── Get Hotel by CNPJ
│   ├── Create Hotel
│   ├── Update Hotel
│   └── Delete Hotel
│
├── 🛏️ Rooms/
│   ├── List Rooms
│   ├── Get Room by ID
│   ├── List Rooms by Hotel
│   ├── Check Availability
│   ├── Create Room
│   ├── Update Room
│   └── Delete Room
│
├── 🏷️ Room Types/
│   ├── List Room Types by Hotel
│   ├── Get Room Type by ID
│   ├── Create Room Type
│   ├── Update Room Type
│   └── Delete Room Type
│
├── 👤 Guests/
│   ├── List Guests
│   ├── Get Guest by ID
│   ├── List Guests by Hotel
│   ├── Create Guest
│   ├── Update Guest
│   └── Delete Guest
│
├── 📅 Bookings/
│   ├── List Bookings
│   ├── Get Booking by ID
│   ├── Get Booking by Code
│   ├── List Bookings by Hotel
│   ├── List Bookings by Guest
│   ├── Create Booking
│   ├── Update Booking
│   ├── Cancel Booking
│   ├── Confirm Booking
│   ├── Check-In
│   └── Check-Out
│
├── 🧾 Invoices/
│   ├── Create Simple Invoice
│   ├── Create Complete Invoice
│   ├── Cancel Invoice
│   ├── Get by Verification Code
│   └── Get by Number and Serie
│
├── 👥 Users/
│   ├── List Users
│   ├── Get User by ID
│   └── Get Profile
│
└── 🏠 Guest Portal/
    ├── Get Profile
    ├── Update Profile
    ├── My Bookings
    ├── Booking Details
    └── Cancel Booking
```

---

## 🔐 1. Auth

### 1.1 Login Admin

**Request:**
```
POST {{baseUrl}}/api/Auth/login
Content-Type: application/json
```

**Body:**
```json
{
  "email": "{{adminEmail}}",
  "password": "{{adminPassword}}"
}
```

**Tests (script pós-request):**
```javascript
// Salvar token na variável de ambiente
pm.test("Status 200", function() {
    pm.response.to.have.status(200);
});

let response = pm.response.json();
pm.environment.set("token", response.token);
pm.environment.set("userId", response.user.id);
```

---

### 1.2 Login Guest

**Request:**
```
POST {{baseUrl}}/api/Auth/login
Content-Type: application/json
```

**Body:**
```json
{
  "email": "joao@email.com",
  "password": "Senha123!"
}
```

---

### 1.3 Register Guest

**Request:**
```
POST {{baseUrl}}/api/Auth/register-guest
Content-Type: application/json
```

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao{{$randomInt}}@email.com",
  "password": "Senha123!",
  "phone": "+55 11 99999-9999",
  "documentType": "CPF",
  "document": "123.456.789-00",
  "birthDate": "1990-01-01T00:00:00Z",
  "addressLine1": "Rua Teste, 123",
  "addressLine2": "Apto 45",
  "city": "São Paulo",
  "neighborhood": "Centro",
  "state": "SP",
  "postalCode": "01234-567",
  "countryCode": "BR",
  "marketingConsent": true,
  "hotelId": "{{hotelId}}"
}
```

**Nota:** `{{$randomInt}}` gera um número aleatório para email único

---

## 🏨 2. Hotels

### 2.1 List Hotels

**Request:**
```
GET {{baseUrl}}/api/Hotels
Authorization: Bearer {{token}}
```

---

### 2.2 Get Hotel by ID

**Request:**
```
GET {{baseUrl}}/api/Hotels/{{hotelId}}
Authorization: Bearer {{token}}
```

---

### 2.3 Create Hotel

**Request:**
```
POST {{baseUrl}}/api/Hotels
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Hotel {{$randomLastName}}",
  "brandName": "Rede Hotels",
  "cnpj": "12.345.678/0001-90",
  "email": "contato{{$randomInt}}@hotel.com",
  "phone": "+55 47 3000-0000",
  "addressLine1": "Rua Principal, 100",
  "addressLine2": "Sala 201",
  "city": "Joinville",
  "state": "SC",
  "postalCode": "89200-000",
  "countryCode": "BR"
}
```

**Tests:**
```javascript
let response = pm.response.json();
pm.environment.set("newHotelId", response.id);
```

---

## 🛏️ 3. Rooms

### 3.1 List Rooms

**Request:**
```
GET {{baseUrl}}/api/Rooms?hotelId={{hotelId}}&status=AVAILABLE
Authorization: Bearer {{token}}
```

---

### 3.2 Check Availability

**Request:**
```
GET {{baseUrl}}/api/Rooms/availability?hotelId={{hotelId}}&checkIn=2025-12-01T14:00:00Z&checkOut=2025-12-05T12:00:00Z&adults=2
Authorization: Bearer {{token}}
```

---

### 3.3 Create Room

**Request:**
```
POST {{baseUrl}}/api/Rooms
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
{
  "hotelId": "{{hotelId}}",
  "roomNumber": "{{$randomInt}}",
  "floor": 2,
  "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
  "status": "AVAILABLE",
  "maxOccupancy": 2
}
```

---

## 👤 4. Guests

### 4.1 List Guests by Hotel

**Request:**
```
GET {{baseUrl}}/api/Guests/hotel/{{hotelId}}
Authorization: Bearer {{token}}
```

---

### 4.2 Create Guest

**Request:**
```
POST {{baseUrl}}/api/Guests
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
{
  "hotelId": "{{hotelId}}",
  "fullName": "{{$randomFirstName}} {{$randomLastName}}",
  "email": "{{$randomEmail}}",
  "phone": "+55 11 9{{$randomInt}}",
  "documentType": "CPF",
  "document": "{{$randomInt}}.{{$randomInt}}.{{$randomInt}}-{{$randomInt}}",
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

**Tests:**
```javascript
let response = pm.response.json();
pm.environment.set("newGuestId", response.id);
```

---

## 📅 5. Bookings

### 5.1 List Bookings by Hotel

**Request:**
```
GET {{baseUrl}}/api/Bookings/hotel/{{hotelId}}?startDate=2025-11-01&endDate=2025-12-31
Authorization: Bearer {{token}}
```

---

### 5.2 Create Booking

**Request:**
```
POST {{baseUrl}}/api/Bookings
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
{
  "hotelId": "{{hotelId}}",
  "code": "BK-{{$timestamp}}",
  "source": "DIRECT",
  "checkInDate": "2025-12-10T14:00:00Z",
  "checkOutDate": "2025-12-15T12:00:00Z",
  "adults": 2,
  "children": 0,
  "currency": "BRL",
  "mainGuestId": "{{guestId}}",
  "channelRef": null,
  "notes": "Teste de reserva via API",
  "bookingRooms": [
    {
      "roomId": "{{roomId}}",
      "roomTypeId": "2318702e-1c6d-4d1c-8f07-d6e0ace9d441",
      "ratePlanId": null,
      "priceTotal": 1500.00,
      "notes": null
    }
  ],
  "additionalGuestIds": []
}
```

**Tests:**
```javascript
let response = pm.response.json();
pm.environment.set("bookingId", response.id);
pm.environment.set("bookingCode", response.code);
```

---

### 5.3 Check-In

**Request:**
```
POST {{baseUrl}}/api/Bookings/{{bookingId}}/check-in
Authorization: Bearer {{token}}
```

---

### 5.4 Check-Out

**Request:**
```
POST {{baseUrl}}/api/Bookings/{{bookingId}}/check-out
Authorization: Bearer {{token}}
```

---

### 5.5 Cancel Booking

**Request:**
```
POST {{baseUrl}}/api/Bookings/{{bookingId}}/cancel?reason=Teste de cancelamento
Authorization: Bearer {{token}}
```

---

## 🧾 6. Invoices

### 6.1 Create Simple Invoice

**Request:**
```
POST {{baseUrl}}/api/Invoices/simple/{{roomId}}
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
{
  "guestId": "{{guestId}}",
  "checkInDate": "2025-11-10T14:00:00Z",
  "days": 5,
  "adults": 2,
  "children": 1,
  "totalValue": 1500.00,
  "description": "Hospedagem Standard",
  "observations": "Estadia completa"
}
```

**Tests:**
```javascript
let response = pm.response.json();
if(response.success) {
    pm.environment.set("nfseNumber", response.nfseNumber);
    pm.environment.set("verificationCode", response.verificationCode);
}
```

---

### 6.2 Get Invoice by Verification Code

**Request:**
```
GET {{baseUrl}}/api/Invoices/{{hotelId}}/verification/{{verificationCode}}
Authorization: Bearer {{token}}
```

---

## 👥 7. Users

### 7.1 List Users

**Request:**
```
GET {{baseUrl}}/api/Users
Authorization: Bearer {{token}}
```

---

### 7.2 Get Profile

**Request:**
```
GET {{baseUrl}}/api/Users/profile
Authorization: Bearer {{token}}
```

---

## 🏠 8. Guest Portal

### 8.1 Get Profile

**Request:**
```
GET {{baseUrl}}/api/GuestPortal/profile
Authorization: Bearer {{token}}
```

**Nota:** Use token de Guest (login com hóspede)

---

### 8.2 My Bookings

**Request:**
```
GET {{baseUrl}}/api/GuestPortal/bookings
Authorization: Bearer {{token}}
```

---

### 8.3 Cancel My Booking

**Request:**
```
POST {{baseUrl}}/api/GuestPortal/bookings/{{bookingId}}/cancel
Authorization: Bearer {{token}}
Content-Type: application/json
```

**Body:**
```json
"Mudança de planos"
```

---

## 🔄 Fluxo Completo de Teste

### Sequência Recomendada:

1. **Setup:**
   - ✅ Login Admin → Salvar token

2. **Criar Dados:**
   - ✅ Create Guest → Salvar guestId
   - ✅ Create Room (se necessário)

3. **Fluxo de Reserva:**
   - ✅ Check Availability
   - ✅ Create Booking → Salvar bookingId
   - ✅ Confirm Booking
   - ✅ Check-In
   - ✅ Create Simple Invoice
   - ✅ Check-Out

4. **Portal do Hóspede:**
   - ✅ Register Guest → Salvar token Guest
   - ✅ Get Profile
   - ✅ My Bookings

---

## 🎨 Pre-request Scripts Úteis

### Script Global (Pre-request Script na Collection)

```javascript
// Renovar token se expirado (simplificado)
const token = pm.environment.get("token");
if (!token && pm.request.headers.has("Authorization")) {
    console.log("Token ausente, faça login primeiro!");
}
```

---

### Script para Gerar Data Futura

```javascript
// Gerar data de check-in (7 dias no futuro)
const checkIn = new Date();
checkIn.setDate(checkIn.getDate() + 7);
pm.environment.set("checkInDate", checkIn.toISOString());

// Gerar data de check-out (12 dias no futuro)
const checkOut = new Date();
checkOut.setDate(checkOut.getDate() + 12);
pm.environment.set("checkOutDate", checkOut.toISOString());
```

---

## 🧪 Testes Automatizados

### Test Suite Básico (adicionar em cada request)

```javascript
// Status 200 ou 201
pm.test("Status code is success", function () {
    pm.expect(pm.response.code).to.be.oneOf([200, 201, 204]);
});

// Response time < 2s
pm.test("Response time is acceptable", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

// Content-Type é JSON
pm.test("Content-Type is JSON", function () {
    pm.expect(pm.response.headers.get("Content-Type")).to.include("application/json");
});
```

---

### Test para Login

```javascript
pm.test("Login successful", function () {
    pm.response.to.have.status(200);
    
    let response = pm.response.json();
    
    pm.test("Token exists", function() {
        pm.expect(response.token).to.be.a('string');
        pm.expect(response.token.length).to.be.above(10);
    });
    
    pm.test("User data exists", function() {
        pm.expect(response.user).to.have.property('id');
        pm.expect(response.user).to.have.property('email');
        pm.expect(response.user).to.have.property('roles');
    });
    
    // Salvar dados
    pm.environment.set("token", response.token);
    pm.environment.set("userId", response.user.id);
    pm.environment.set("userEmail", response.user.email);
});
```

---

### Test para Create

```javascript
pm.test("Resource created", function () {
    pm.response.to.have.status(201);
    
    let response = pm.response.json();
    
    pm.test("ID exists", function() {
        pm.expect(response.id).to.be.a('string');
        pm.expect(response.id).to.match(/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i);
    });
    
    // Salvar ID
    pm.environment.set("lastCreatedId", response.id);
});
```

---

## 📊 Runners

### Collection Runner - Fluxo Completo

Execute nesta ordem:
1. Auth → Login Admin
2. Guests → Create Guest
3. Bookings → Create Booking
4. Bookings → Confirm Booking
5. Bookings → Check-In
6. Invoices → Create Simple Invoice
7. Bookings → Check-Out

---

## 🐛 Debug Tips

### Ver Request Completo

```javascript
console.log("Request URL:", pm.request.url.toString());
console.log("Request Body:", JSON.stringify(pm.request.body.raw));
console.log("Response Body:", pm.response.text());
```

### Ver Token Decodificado

```javascript
function parseJwt(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(base64));
}

const token = pm.environment.get("token");
console.log("Token decoded:", parseJwt(token));
```

---

## 📝 Notas Importantes

1. **Sempre faça login primeiro** para obter o token
2. **Verifique variáveis de ambiente** antes de executar
3. **Use {{$randomInt}}** para dados únicos em testes
4. **Salve IDs importantes** nos tests para usar depois
5. **Token expira em 1 hora** - faça login novamente se expirar

---

## 🔗 Links Úteis

- Documentação Completa: `API_ENDPOINTS_COMPLETE_DOCS.md`
- Referência Rápida: `API_QUICK_REFERENCE.md`
- Portal do Hóspede: `GUEST_PORTAL_API_DOCS.md`

---

**Versão:** 1.0  
**Data:** 31/10/2025

