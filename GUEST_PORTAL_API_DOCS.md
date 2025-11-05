# 🏨 API Portal do Hóspede - Documentação Frontend

## 📋 Visão Geral

Agora os hóspedes podem se registrar no sistema e ter acesso ao **Portal do Hóspede**, onde podem:
- ✅ Criar uma conta própria
- ✅ Fazer login
- ✅ Ver e atualizar seu perfil
- ✅ Ver suas reservas
- ✅ Cancelar reservas

---

## 🔑 Novo Role: "Guest"

Foi criado um novo role **"Guest"** para hóspedes. Agora temos:
- **Admin**: acesso total a todos os hotéis
- **Hotel-Admin**: acesso a um hotel específico
- **Guest**: acesso apenas aos seus próprios dados

---

## 🚀 Fluxo de Uso

### 1️⃣ Registro de Novo Hóspede

**Endpoint:** `POST /api/Auth/register-guest`  
**Autenticação:** ❌ Público (não precisa de token)

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

**Validações:**
- `name`: obrigatório, max 100 caracteres
- `email`: obrigatório, formato válido, max 100 caracteres
- `password`: obrigatório, mínimo 6 caracteres
- `phone`: obrigatório, formato válido
- `documentType`: obrigatório, max 10 caracteres (CPF/CNPJ)
- `document`: obrigatório, max 20 caracteres
- `birthDate`: obrigatório
- `addressLine1`: obrigatório, max 200 caracteres
- `city`: obrigatório, max 100 caracteres
- `state`: obrigatório, 2 caracteres (UF)
- `postalCode`: obrigatório, max 10 caracteres
- `hotelId`: obrigatório (ID do hotel)

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

**Response 400 Bad Request:**
```json
{
  "message": "Email já cadastrado"
}
```

---

### 2️⃣ Login de Hóspede

**Endpoint:** `POST /api/Auth/login`  
**Autenticação:** ❌ Público

**Request Body:**
```json
{
  "email": "joao@email.com",
  "password": "Senha123!"
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

## 🏠 Endpoints do Portal do Hóspede

Todos os endpoints abaixo requerem:
- **Autenticação:** ✅ `Authorization: Bearer {token}`
- **Role:** `Guest`

---

### 3️⃣ Ver Perfil do Hóspede

**Endpoint:** `GET /api/GuestPortal/profile`  
**Autenticação:** ✅ Bearer Token (Guest)

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

**Response 400 Bad Request:**
```json
{
  "message": "GuestId não encontrado no token"
}
```

---

### 4️⃣ Atualizar Perfil do Hóspede

**Endpoint:** `PUT /api/GuestPortal/profile`  
**Autenticação:** ✅ Bearer Token (Guest)

**Request Body:** (mesma estrutura do registro)
```json
{
  "name": "João Silva Santos",
  "email": "joao.santos@email.com",
  "password": "NovaSenha123!",
  "phone": "+55 11 98888-8888",
  "documentType": "CPF",
  "document": "123.456.789-00",
  "birthDate": "1990-01-01T00:00:00",
  "addressLine1": "Rua Nova, 456",
  "addressLine2": "Casa",
  "city": "São Paulo",
  "neighborhood": "Jardins",
  "state": "SP",
  "postalCode": "01234-999",
  "countryCode": "BR",
  "marketingConsent": false,
  "hotelId": "7a326969-3bf6-40d9-96dc-1aecef585000"
}
```

**Response 200 OK:** (retorna o perfil atualizado, mesmo formato do GET)

---

### 5️⃣ Listar Minhas Reservas

**Endpoint:** `GET /api/GuestPortal/bookings`  
**Autenticação:** ✅ Bearer Token (Guest)

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

### 6️⃣ Ver Detalhes de uma Reserva

**Endpoint:** `GET /api/GuestPortal/bookings/{id}`  
**Autenticação:** ✅ Bearer Token (Guest)

**Response 200 OK:**
```json
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
```

**Response 404 Not Found:**
```json
{
  "message": "Reserva não encontrada"
}
```

**Response 403 Forbidden:**
Retornado se a reserva não pertence ao hóspede logado.

---

### 7️⃣ Cancelar uma Reserva

**Endpoint:** `POST /api/GuestPortal/bookings/{id}/cancel`  
**Autenticação:** ✅ Bearer Token (Guest)

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

**Response 404 Not Found:**
```json
{
  "message": "Reserva não encontrada"
}
```

**Response 403 Forbidden:**
Retornado se a reserva não pertence ao hóspede logado.

**Response 400 Bad Request:**
```json
{
  "message": "Não foi possível cancelar a reserva"
}
```

---

## 🔐 Estrutura do Token JWT

Quando um hóspede faz login ou se registra, o token JWT contém:

```json
{
  "nameid": "550e8400-e29b-41d4-a716-446655440000",
  "name": "João Silva",
  "email": "joao@email.com",
  "GuestId": "87f086dd-d461-49c8-a63c-1fc7b6a55441",
  "role": "Guest",
  "exp": 1730462400,
  "iss": "AvenSuites-Api",
  "aud": "AvenSuites-Client"
}
```

**Claims importantes:**
- `nameid`: ID do usuário
- `GuestId`: ID do hóspede (use para operações)
- `role`: "Guest"
- `exp`: timestamp de expiração (1 hora após login)

---

## 🎨 Exemplo de Fluxo Completo no Frontend

### 1. Tela de Registro

```javascript
async function registrarHospede(formData) {
  const response = await fetch('https://api.avensuites.com/api/Auth/register-guest', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      name: formData.name,
      email: formData.email,
      password: formData.password,
      phone: formData.phone,
      documentType: "CPF",
      document: formData.cpf,
      birthDate: formData.birthDate,
      addressLine1: formData.address,
      addressLine2: formData.complement,
      city: formData.city,
      neighborhood: formData.neighborhood,
      state: formData.state,
      postalCode: formData.cep,
      countryCode: "BR",
      marketingConsent: formData.acceptMarketing,
      hotelId: formData.selectedHotelId
    })
  });

  if (response.ok) {
    const data = await response.json();
    // Salvar token no localStorage/sessionStorage
    localStorage.setItem('authToken', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
    // Redirecionar para o portal
    window.location.href = '/portal';
  } else {
    const error = await response.json();
    alert(error.message);
  }
}
```

### 2. Tela de Login

```javascript
async function fazerLogin(email, password) {
  const response = await fetch('https://api.avensuites.com/api/Auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ email, password })
  });

  if (response.ok) {
    const data = await response.json();
    localStorage.setItem('authToken', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
    window.location.href = '/portal';
  } else {
    alert('Email ou senha inválidos');
  }
}
```

### 3. Portal do Hóspede - Ver Perfil

```javascript
async function carregarPerfil() {
  const token = localStorage.getItem('authToken');
  
  const response = await fetch('https://api.avensuites.com/api/GuestPortal/profile', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  if (response.ok) {
    const perfil = await response.json();
    exibirPerfil(perfil);
  } else if (response.status === 401) {
    // Token expirado
    window.location.href = '/login';
  }
}
```

### 4. Portal do Hóspede - Listar Reservas

```javascript
async function carregarReservas() {
  const token = localStorage.getItem('authToken');
  
  const response = await fetch('https://api.avensuites.com/api/GuestPortal/bookings', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  if (response.ok) {
    const reservas = await response.json();
    exibirReservas(reservas);
  }
}
```

### 5. Cancelar Reserva

```javascript
async function cancelarReserva(bookingId, motivo) {
  const token = localStorage.getItem('authToken');
  
  const response = await fetch(
    `https://api.avensuites.com/api/GuestPortal/bookings/${bookingId}/cancel`,
    {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(motivo)
    }
  );

  if (response.ok) {
    alert('Reserva cancelada com sucesso!');
    carregarReservas(); // Recarregar lista
  } else {
    const error = await response.json();
    alert(error.message);
  }
}
```

---

## ⚠️ Tratamento de Erros

### Status Codes Comuns

| Status | Significado | Ação |
|--------|-------------|------|
| 200 | OK | Sucesso |
| 400 | Bad Request | Validar campos do formulário |
| 401 | Unauthorized | Token inválido/expirado → redirecionar para login |
| 403 | Forbidden | Sem permissão para acessar recurso |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro no servidor |

### Exemplo de Interceptor (Axios)

```javascript
axios.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      // Token expirado
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
```

---

## 🔒 Segurança

### Proteção de Rotas

Certifique-se de verificar no frontend se o usuário está autenticado e tem o role correto:

```javascript
function protegerRota() {
  const token = localStorage.getItem('authToken');
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  if (!token || !user.roles?.includes('Guest')) {
    window.location.href = '/login';
    return false;
  }

  return true;
}
```

### Verificar Expiração do Token

```javascript
function tokenExpirado(token) {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
}
```

---

## 📱 Exemplo de UI/UX Recomendada

### Tela de Registro
```
┌─────────────────────────────────────┐
│  🏨 Cadastre-se no Hotel Avenida    │
├─────────────────────────────────────┤
│ Nome Completo:     [____________]   │
│ Email:             [____________]   │
│ Senha:             [____________]   │
│ Telefone:          [____________]   │
│ CPF:               [____________]   │
│ Data Nascimento:   [__/__/____]     │
│                                      │
│ 📍 Endereço                          │
│ Logradouro:        [____________]   │
│ Número:            [____]            │
│ Complemento:       [____________]   │
│ Bairro:            [____________]   │
│ Cidade:            [____________]   │
│ Estado:            [__]              │
│ CEP:               [_____-___]       │
│                                      │
│ ☐ Aceito receber ofertas             │
│                                      │
│         [  Criar Conta  ]            │
│                                      │
│ Já tem conta? [Fazer login]         │
└─────────────────────────────────────┘
```

### Portal do Hóspede
```
┌─────────────────────────────────────┐
│  🏨 Olá, João Silva!                │
├─────────────────────────────────────┤
│ [Meu Perfil] [Minhas Reservas]      │
├─────────────────────────────────────┤
│                                      │
│ 📅 Próximas Reservas:                │
│                                      │
│ ┌─────────────────────────────────┐ │
│ │ 🏨 Hotel Avenida                │ │
│ │ Check-in:  10/11/2025           │ │
│ │ Check-out: 15/11/2025           │ │
│ │ Status: ✅ Confirmada            │ │
│ │                                  │ │
│ │ [Ver Detalhes] [Cancelar]       │ │
│ └─────────────────────────────────┘ │
│                                      │
└─────────────────────────────────────┘
```

---

## 🎯 Status da Implementação

| Feature | Status |
|---------|--------|
| Registro de hóspede | ✅ Implementado |
| Login de hóspede | ✅ Implementado |
| JWT com GuestId | ✅ Implementado |
| Ver perfil | ✅ Implementado |
| Atualizar perfil | ✅ Implementado |
| Listar reservas | ✅ Implementado |
| Ver detalhes reserva | ✅ Implementado |
| Cancelar reserva | ✅ Implementado |
| Criar reserva (guest) | ⏳ Em desenvolvimento |

---

## 📞 Suporte

Para dúvidas ou problemas, entre em contato com a equipe de backend.

**Base URL:** `https://api.avensuites.com` (produção)  
**Base URL:** `https://localhost:7000` (desenvolvimento)

---

## 📝 Notas Importantes

1. **Token expira em 1 hora** - implemente refresh ou peça novo login
2. **Validações são feitas no backend** - mas faça validações no frontend também
3. **Sempre envie o header Authorization** nos endpoints protegidos
4. **CPF/CNPJ** podem ser enviados com ou sem formatação
5. **Telefone** deve estar no formato E.164: `+55 11 99999-9999`
6. **Datas** devem estar no formato ISO 8601: `2025-10-31T10:00:00Z`

---

**Versão:** 1.0  
**Data:** 31/10/2025  
**Backend API:** AvenSuites-Api v9.0

