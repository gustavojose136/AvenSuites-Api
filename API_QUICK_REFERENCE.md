# 🚀 AvenSuites API - Referência Rápida

## 📊 Visão Geral

**Total de Endpoints:** 52  
**Base URL:** `https://localhost:7000` (dev) | `https://api.avensuites.com` (prod)

---

## 🔐 Autenticação (4 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| POST | `/api/Auth/login` | ❌ | - | Login de usuário |
| POST | `/api/Auth/register` | ❌ | - | Registrar admin/staff |
| POST | `/api/Auth/validate` | ❌ | - | Validar credenciais |
| POST | `/api/Auth/register-guest` | ❌ | - | Registrar hóspede |

---

## 🏨 Hotéis (6 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/Hotels` | ✅ | Admin, Hotel-Admin | Listar hotéis |
| GET | `/api/Hotels/{id}` | ✅ | Admin, Hotel-Admin | Buscar por ID |
| GET | `/api/Hotels/cnpj/{cnpj}` | ✅ | Admin, Hotel-Admin | Buscar por CNPJ |
| POST | `/api/Hotels` | ✅ | Admin | Criar hotel |
| PUT | `/api/Hotels/{id}` | ✅ | Admin, Hotel-Admin | Atualizar hotel |
| DELETE | `/api/Hotels/{id}` | ✅ | Admin | Deletar hotel |

---

## 🛏️ Quartos (7 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/Rooms` | ✅ | Admin, Hotel-Admin | Listar quartos |
| GET | `/api/Rooms/{id}` | ✅ | Admin, Hotel-Admin | Buscar por ID |
| GET | `/api/Rooms/hotel/{hotelId}` | ✅ | Admin, Hotel-Admin | Listar por hotel |
| GET | `/api/Rooms/availability` | ✅ | Admin, Hotel-Admin | Verificar disponibilidade |
| POST | `/api/Rooms` | ✅ | Admin, Hotel-Admin | Criar quarto |
| PUT | `/api/Rooms/{id}` | ✅ | Admin, Hotel-Admin | Atualizar quarto |
| DELETE | `/api/Rooms/{id}` | ✅ | Admin, Hotel-Admin | Deletar quarto |

---

## 🏷️ Tipos de Quarto (5 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| POST | `/api/RoomTypes` | ✅ | - | Criar tipo |
| GET | `/api/RoomTypes/{id}` | ✅ | - | Buscar por ID |
| GET | `/api/RoomTypes/hotel/{hotelId}` | ✅ | - | Listar por hotel |
| PUT | `/api/RoomTypes/{id}` | ✅ | - | Atualizar tipo |
| DELETE | `/api/RoomTypes/{id}` | ✅ | - | Deletar tipo |

---

## 👤 Hóspedes (6 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/Guests` | ✅ | Admin, Hotel-Admin | Listar hóspedes |
| GET | `/api/Guests/{id}` | ✅ | Admin, Hotel-Admin | Buscar por ID |
| GET | `/api/Guests/hotel/{hotelId}` | ✅ | Admin, Hotel-Admin | Listar por hotel |
| POST | `/api/Guests` | ✅ | Admin, Hotel-Admin | Criar hóspede |
| PUT | `/api/Guests/{id}` | ✅ | Admin, Hotel-Admin | Atualizar hóspede |
| DELETE | `/api/Guests/{id}` | ✅ | Admin, Hotel-Admin | Deletar hóspede |

---

## 📅 Reservas (11 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/Bookings` | ✅ | Admin, Hotel-Admin | Listar reservas |
| GET | `/api/Bookings/{id}` | ✅ | Admin, Hotel-Admin | Buscar por ID |
| GET | `/api/Bookings/code/{code}` | ✅ | Admin, Hotel-Admin | Buscar por código |
| GET | `/api/Bookings/hotel/{hotelId}` | ✅ | Admin, Hotel-Admin | Listar por hotel |
| GET | `/api/Bookings/guest/{guestId}` | ✅ | Admin, Hotel-Admin | Listar por hóspede |
| POST | `/api/Bookings` | ✅ | Admin, Hotel-Admin | Criar reserva |
| PUT | `/api/Bookings/{id}` | ✅ | Admin, Hotel-Admin | Atualizar reserva |
| POST | `/api/Bookings/{id}/cancel` | ✅ | Admin, Hotel-Admin | Cancelar reserva |
| POST | `/api/Bookings/{id}/confirm` | ✅ | Admin, Hotel-Admin | Confirmar reserva |
| POST | `/api/Bookings/{id}/check-in` | ✅ | Admin, Hotel-Admin | Check-in |
| POST | `/api/Bookings/{id}/check-out` | ✅ | Admin, Hotel-Admin | Check-out |

---

## 🧾 Notas Fiscais (5 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| POST | `/api/Invoices/simple/{roomId}` | ✅ | Admin, Hotel-Admin | Criar NF-e simplificada |
| POST | `/api/Invoices` | ✅ | Admin, Hotel-Admin | Criar NF-e completa |
| POST | `/api/Invoices/{hotelId}/cancel` | ✅ | Admin, Hotel-Admin | Cancelar NF-e |
| GET | `/api/Invoices/{hotelId}/verification/{code}` | ✅ | Admin, Hotel-Admin | Buscar por código |
| GET | `/api/Invoices/{hotelId}/number/{num}/serie/{serie}` | ✅ | Admin, Hotel-Admin | Buscar por número |

---

## 👥 Usuários (3 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/Users` | ✅ | Admin | Listar usuários |
| GET | `/api/Users/{id}` | ✅ | Admin ou próprio | Buscar por ID |
| GET | `/api/Users/profile` | ✅ | - | Ver perfil logado |

---

## 🏠 Portal do Hóspede (5 endpoints)

| Método | Endpoint | Auth | Roles | Descrição |
|--------|----------|------|-------|-----------|
| GET | `/api/GuestPortal/profile` | ✅ | Guest | Ver perfil |
| PUT | `/api/GuestPortal/profile` | ✅ | Guest | Atualizar perfil |
| GET | `/api/GuestPortal/bookings` | ✅ | Guest | Minhas reservas |
| GET | `/api/GuestPortal/bookings/{id}` | ✅ | Guest | Detalhes da reserva |
| POST | `/api/GuestPortal/bookings/{id}/cancel` | ✅ | Guest | Cancelar reserva |

---

## 🔑 Roles e Acessos

| Role | Acesso | Endpoints |
|------|--------|-----------|
| **Admin** | Global - todos os hotéis | Todos + gerenciamento de hotéis |
| **Hotel-Admin** | Hotel específico | Gestão do próprio hotel |
| **Guest** | Dados próprios | Portal do hóspede apenas |
| **User** | Básico | Limitado |

---

## 📊 Status dos Quartos

- `AVAILABLE` - Disponível
- `OCCUPIED` - Ocupado
- `CLEANING` - Em limpeza
- `MAINTENANCE` - Manutenção
- `OUT_OF_SERVICE` - Fora de serviço

---

## 📊 Status das Reservas

- `PENDING` - Pendente
- `CONFIRMED` - Confirmada
- `CHECKED_IN` - Check-in realizado
- `CHECKED_OUT` - Check-out realizado
- `CANCELLED` - Cancelada
- `NO_SHOW` - Não compareceu

---

## 🔐 Headers Obrigatórios

### Para Endpoints Autenticados:
```http
Authorization: Bearer {token}
Content-Type: application/json
```

### Para Endpoints Públicos:
```http
Content-Type: application/json
```

---

## ⚡ Exemplos Rápidos

### Login
```bash
curl -X POST http://localhost:7000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@avensuites.com","password":"Admin123!"}'
```

### Listar Quartos
```bash
curl -X GET "http://localhost:7000/api/Rooms?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000" \
  -H "Authorization: Bearer {token}"
```

### Criar Reserva
```bash
curl -X POST http://localhost:7000/api/Bookings \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "hotelId":"7a326969-3bf6-40d9-96dc-1aecef585000",
    "code":"BK-2025-001",
    "source":"DIRECT",
    "checkInDate":"2025-12-01T14:00:00Z",
    "checkOutDate":"2025-12-05T12:00:00Z",
    "adults":2,
    "mainGuestId":"87f086dd-d461-49c8-a63c-1fc7b6a55441",
    "bookingRooms":[{"roomId":"40d5718c-dbda-40c7-a4f4-644cd6f177bd","roomTypeId":"2318702e-1c6d-4d1c-8f07-d6e0ace9d441","priceTotal":1200.00}]
  }'
```

---

## 🎯 IDs Úteis (Seed Data)

### Hotel
- **Hotel Avenida:** `7a326969-3bf6-40d9-96dc-1aecef585000`

### Roles
- **Admin:** `60ccaec1-6c42-4fb5-a104-2036b42585a3`
- **Hotel-Admin:** `a1b2c3d4-e5f6-7890-abcd-ef1234567890`
- **Guest:** `b2c3d4e5-f6a7-8901-bcde-f12345678901`

### Usuários
- **Admin:** `2975cf19-0baa-4507-9f98-968760deb546`
- **Gustavo (Hotel-Admin):** `f36d8acd-1822-4019-ac76-a6ea959d5193`

### Quartos
- **Quarto 101:** `40d5718c-dbda-40c7-a4f4-644cd6f177bd`
- **Quarto 102:** `4cdcf044-587e-4047-b164-a8cd64bad303`
- **Quarto 201:** `6bd29bd5-4826-45a0-b734-3197fec5cfbd`

### Hóspede
- **Joni Cardoso:** `87f086dd-d461-49c8-a63c-1fc7b6a55441`

---

## 📝 Notas Importantes

1. **Token JWT expira em 1 hora**
2. **Datas em formato ISO 8601:** `2025-11-10T14:00:00Z`
3. **IDs são UUIDs/GUIDs:** formato `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
4. **Moeda padrão:** BRL (Real Brasileiro)
5. **Telefones em formato E.164:** `+55 47 3433-0000`

---

## 🔍 Busca e Filtros

### Query Parameters Comuns:

| Parâmetro | Tipo | Descrição | Exemplo |
|-----------|------|-----------|---------|
| `hotelId` | GUID | Filtrar por hotel | `?hotelId=7a326969-3bf6-40d9-96dc-1aecef585000` |
| `status` | String | Filtrar por status | `?status=AVAILABLE` |
| `startDate` | DateTime | Data inicial | `?startDate=2025-11-01` |
| `endDate` | DateTime | Data final | `?endDate=2025-11-30` |
| `guestId` | GUID | Filtrar por hóspede | `?guestId=87f086dd-d461-49c8-a63c-1fc7b6a55441` |
| `activeOnly` | Boolean | Apenas ativos | `?activeOnly=true` |

---

## ⚠️ Erros Comuns

| Status | Erro | Solução |
|--------|------|---------|
| 401 | Token inválido | Fazer login novamente |
| 403 | Sem permissão | Verificar role e acesso ao hotel |
| 404 | Não encontrado | Verificar se o ID existe |
| 400 | Dados inválidos | Verificar validações do request |

---

## 📚 Documentação Completa

Para detalhes completos de cada endpoint, consulte:
- **`API_ENDPOINTS_COMPLETE_DOCS.md`** - Documentação detalhada com exemplos
- **`GUEST_PORTAL_API_DOCS.md`** - Documentação específica do portal do hóspede

---

**Versão:** 1.0  
**Última Atualização:** 31/10/2025

