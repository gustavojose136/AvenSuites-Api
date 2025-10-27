# AvenSuites - Documentação do Banco de Dados

## 📊 Visão Geral

O banco de dados **AvenSuites** foi projetado para suportar um sistema completo de gestão hoteleira, incluindo gestão de hotéis, quartos, reservas, hóspedes, faturamento e comunicação.

### Características Principais

- **Banco**: MySQL 8.0 com charset utf8mb4
- **Charset**: utf8mb4_0900_ai_ci
- **Criptografia**: PII (Dados Pessoais) com AES-GCM e derivação via Argon2id
- **Arquitetura**: Modular em microserviços orientada a eventos
- **Padrão**: Nomes em snake_case compatível com EF Core

---

## 🏗️ Estrutura do Banco

### **1. Gestão de Hotéis e Usuários**

#### **hotels**
Armazena informações dos hotéis cadastrados no sistema.

```sql
hotels (
  hotel_id,           -- CHAR(36) PK
  name,              -- Nome do hotel
  trade_name,        -- Nome fantasia
  cnpj,             -- CNPJ único
  email, phone_e164,
  timezone,         -- Default: America/Sao_Paulo
  address_line1/2, city, state, postal_code,
  country_code,     -- Default: 'BR'
  status            -- ACTIVE/INACTIVE
)
```

#### **users**
Usuários do sistema (funcionários do hotel).

```sql
users (
  user_id,          -- CHAR(36) PK
  hotel_id,        -- FK → hotels
  full_name, email,
  email_verified,
  password_hash,   -- VARBINARY(200) - Argon2
  status,
  created_at, updated_at
)
```

#### **roles** e **user_roles**
Sistema de autorização por roles (Admin, User, Gestor, etc.)

```sql
roles (
  role_id,         -- CHAR(36) PK
  name, created_at
)

user_roles (
  user_id,         -- PK/FK → users
  role_id          -- PK/FK → roles
)
```

---

### **2. Módulo de Hóspedes**

#### **guests**
Tabela principal de hóspedes.

```sql
guests (
  guest_id,        -- CHAR(36) PK
  hotel_id,       -- FK → hotels
  marketing_consent, -- BOOLEAN
  created_at, updated_at
)
```

#### **guest_pii**
**Dados Pessoais Identificáveis (LGPD)** com criptografia AES-GCM.

```sql
guest_pii (
  guest_id,        -- CHAR(36) PK/FK → guests
  
  -- Dados PII criptografados
  full_name, email, phone_e164,
  document_type, document_plain,
  birth_date,
  address_line1/2, city, state, postal_code,
  
  -- Hashes SHA256 para busca
  email_sha256,
  phone_sha256,
  document_sha256,
  
  -- Criptografia AES-GCM
  document_cipher,     -- VARBINARY(128)
  document_nonce,      -- VARBINARY(12)
  document_tag,        -- VARBINARY(16)
  document_key_version
)
```

**Segurança:**
- Dados sensíveis (CPF, RG) criptografados com AES-GCM
- Hashes SHA256 para pesquisa sem descriptografar
- Versão de chave para rotacionamento

#### **hotel_keys**
Chaves de criptografia por hotel (Argon2id).

```sql
hotel_keys (
  hotel_id,       -- PK/FK → hotels
  key_version,    -- PK (INT)
  kdf_salt,      -- VARBINARY(16)
  created_at, active
)
```

---

### **3. Módulo de Quartos**

#### **room_types**
Tipos de quarto (Suite, Standard, Deluxe, etc.)

```sql
room_types (
  room_type_id,    -- CHAR(36) PK
  hotel_id,       -- FK → hotels
  code,           -- VARCHAR(30) - único por hotel
  name, description,
  capacity_adults,    -- Default: 2
  capacity_children,  -- Default: 0
  base_price,
  active,
  created_at, updated_at
)
```

#### **amenities** e **room_type_amenities**
Amenidades (Wi-Fi, Ar Condicionado, etc.)

```sql
amenities (
  amenity_id, code, name
)

room_type_amenities (
  room_type_id,  -- PK/FK → room_types
  amenity_id    -- PK/FK → amenities
)
```

#### **rooms**
Quartos físicos do hotel.

```sql
rooms (
  room_id,        -- CHAR(36) PK
  hotel_id,      -- FK → hotels
  room_type_id,  -- FK → room_types
  room_number,   -- VARCHAR(20) - único por hotel
  floor,
  status,        -- ACTIVE/MAINTENANCE/OCCUPIED
  created_at, updated_at
)
```

#### **maintenance_blocks**
Bloqueios de manutenção dos quartos.

```sql
maintenance_blocks (
  block_id,       -- CHAR(36) PK
  room_id,        -- FK → rooms
  start_date, end_date,
  reason,
  status,         -- ACTIVE/COMPLETED
  created_by,
  created_at, updated_at
)
```

---

### **4. Módulo de Planos de Tarifas**

#### **rate_plans**
Planos de tarifa (Flexível, Não Reembolsável, etc.)

```sql
rate_plans (
  rate_plan_id,   -- CHAR(36) PK
  hotel_id,      -- FK → hotels
  room_type_id,  -- FK → room_types
  name, currency, -- Default: 'BRL'
  meal_plan,     -- ROOM_ONLY/BREAKFAST/HALF_BOARD
  cancellation_policy,
  active,
  created_at, updated_at
)
```

#### **rate_plan_prices**
Preços por data (precificação dinâmica).

```sql
rate_plan_prices (
  rpp_id,         -- CHAR(36) PK
  rate_plan_id,  -- FK → rate_plans
  price_date,    -- DATE
  price_amount,
  min_stay,      -- Default: 1
  max_stay,
  created_at, updated_at
)
```

---

### **5. Módulo de Reservas (Bookings)**

#### **bookings**
Reservas principais.

```sql
bookings (
  booking_id,     -- CHAR(36) PK
  hotel_id,      -- FK → hotels
  code,          -- VARCHAR(20) - único por hotel (ex: "RES-2025-001")
  status,        -- PENDING/CONFIRMED/CANCELLED/CHECKED_IN/CHECKED_OUT
  source,        -- WEBSITE/WHATSAPP/PHONE/API
  check_in_date, -- DATE
  check_out_date,
  adults, children,
  currency, total_amount,
  main_guest_id, -- FK → guests
  channel_ref,   -- Referência externa
  notes,
  created_by,    -- FK → users
  created_at, updated_at
)
```

#### **booking_guests**
Hóspedes da reserva (principal + acompanhantes).

```sql
booking_guests (
  booking_id,     -- PK/FK → bookings
  guest_id,      -- PK/FK → guests
  role           -- MAIN_GUEST/COMPANION
)
```

#### **booking_rooms**
Quartos da reserva.

```sql
booking_rooms (
  booking_room_id, -- CHAR(36) PK
  booking_id,     -- FK → bookings
  room_id,        -- FK → rooms
  room_type_id,   -- FK → room_types
  rate_plan_id,   -- FK → rate_plans
  price_total,
  notes,
  created_at, updated_at
)
```

#### **booking_room_nights**
Desagregação da reserva por noite (para faturamento).

```sql
booking_room_nights (
  brn_id,         -- CHAR(36) PK
  booking_room_id,-- FK → booking_rooms
  room_id,        -- FK → rooms
  stay_date,      -- DATE
  price_amount,   -- Preço da noite
  tax_amount,
  created_at
)
```

#### **booking_payments**
Pagamentos da reserva.

```sql
booking_payments (
  payment_id,     -- CHAR(36) PK
  booking_id,    -- FK → bookings
  method,        -- CREDIT_CARD/DEBIT/PIX
  status,         -- PENDING/PAID/FAILED
  amount, currency,
  transaction_id,
  paid_at,
  created_at, updated_at
)
```

#### **booking_status_history**
Auditoria de mudanças de status.

```sql
booking_status_history (
  bsh_id,         -- CHAR(36) PK
  booking_id,     -- FK → bookings
  old_status,
  new_status,
  changed_by,     -- FK → users
  changed_at,
  notes
)
```

---

### **6. Módulo de Faturamento (Invoices)**

#### **invoices**
Notas fiscais eletrônicas (NF-e).

```sql
invoices (
  invoice_id,     -- CHAR(36) PK
  booking_id,     -- FK → bookings (UNIQUE)
  hotel_id,      -- FK → hotels
  status,        -- PENDING/PROCESSING/ISSUED/CANCELLED
  issue_date,
  nfse_number,   -- Número da NFSe
  nfse_series,
  rps_number,
  verification_code,
  erp_provider,
  erp_protocol,
  total_services,
  total_taxes,
  xml_s3_key,    -- Chave S3 do XML
  pdf_s3_key,   -- Chave S3 do PDF
  raw_response_json,
  created_at, updated_at
)
```

#### **invoice_items**
Itens da nota fiscal.

```sql
invoice_items (
  invoice_item_id, -- CHAR(36) PK
  invoice_id,     -- FK → invoices
  description,
  quantity,
  unit_price,
  tax_code,       -- Código tributário
  tax_rate,
  total
)
```

#### **erp_integration_logs**
Log de integração com ERP fiscal.

```sql
erp_integration_logs (
  erp_log_id,     -- CHAR(36) PK
  booking_id,     -- FK → bookings
  invoice_id,     -- FK → invoices
  endpoint,
  success,
  error_message,
  request_json,
  response_json,
  created_at
)
```

---

### **7. Módulo de Notificações**

#### **notification_templates**
Templates de e-mail/WhatsApp.

```sql
notification_templates (
  template_key,   -- VARCHAR(80) PK
  channel,        -- EMAIL/WHATSAPP/SMS
  subject_tpl, body_tpl,
  created_at
)
```

#### **notification_logs**
Histórico de envios.

```sql
notification_logs (
  notification_id,  -- CHAR(36) PK
  channel, template_key,
  to_address, to_whatsapp,
  subject, body,
  related_booking_id, related_invoice_id,
  provider_message_id,
  status,           -- QUEUED/SENT/FAILED
  error_message,
  sent_at, created_at
)
```

---

### **8. Módulo de Chat (WhatsApp/Baileys)**

#### **chat_sessions**
Sessões de chat.

```sql
chat_sessions (
  session_id,     -- CHAR(36) PK
  hotel_id,      -- FK → hotels
  guest_id,      -- FK → guests
  wa_user_jid,   -- WhatsApp JID
  state_json,    -- Estado da sessão
  last_interaction_at,
  created_at, updated_at
)
```

#### **chat_messages**
Mensagens do chat.

```sql
chat_messages (
  chat_message_id, -- CHAR(36) PK
  session_id,     -- FK → chat_sessions
  direction,      -- INBOUND/OUTBOUND
  message_id_ext, content_text,
  raw_payload,
  created_at
)
```

---

### **9. Eventos de Integração**

#### **integration_event_outbox**
Outbox Pattern para eventos (Polly).

```sql
integration_event_outbox (
  outbox_id,       -- CHAR(36) PK
  aggregate_type,  -- Booking/Invoice
  aggregate_id,
  event_type,      -- BookingCreated/InvoiceIssued
  payload_json,
  status,          -- PENDING/PUBLISHED
  attempts,
  last_error,
  created_at, published_at
)
```

#### **integration_event_inbox**
Idempotência de eventos consumidos.

```sql
integration_event_inbox (
  inbox_id,        -- CHAR(36) PK
  event_id_ext,
  event_type,
  consumed_at,
  payload_hash
)
```

---

### **10. Outras Tabelas**

#### **api_idempotency_keys**
Idempotência de requisições API.

```sql
api_idempotency_keys (
  idempotency_key, -- VARCHAR(80) PK
  scope, request_hash,
  response_code, response_body,
  created_at, expires_at
)
```

#### **audit_logs**
Auditoria geral do sistema.

```sql
audit_logs (
  audit_id,
  hotel_id, actor_user_id,
  entity_name, entity_id,
  action,         -- INSERT/UPDATE/DELETE
  changes_json,
  created_at
)
```

---

## 🔗 Diagrama de Relacionamentos

```
┌─────────────┐
│   hotels    │
│  (PK: id)   │
└──────┬──────┘
       │
       ├──────────────┬────────────────┬──────────────┬────────────────┬─────────┐
       │              │                │              │                │         │
┌──────▼──────┐ ┌─────▼──────┐ ┌───────▼─────┐ ┌─────────▼─────┐ ┌──▼──────┐
│   users     │ │   guests   │ │  room_types │ │    rooms     │ │ bookings│
│ FK: hotel_id│ │ FK:hotel_id│ │FK:hotel_id  │ │FK:hotel_id   │ │FK:hotel │
└──────┬──────┘ └──────┬──────┘ │             │ │FK:room_type_id│ │         │
       │               │        │             │ │             │ │         │
       │               │        └───────┬─────┘ └─────────┬───┘ │         │
       │               │                │                  │     │         │
┌──────▼──────┐ ┌──────▼──────┐        │                  │     └────┬────┘
│ user_roles │ │  guest_pii  │        │                  │          │
│ (Composite)│ │FK:guest_id   │        │                  │          │
└────────────┘ └─────────────┘        │                  │          │
                                       │                  │          │
                                ┌──────▼──────┐   ┌───────▼──────┐   │
                                │rate_plans   │   │booking_rooms │   │
                                │FK:hotel_id  │   │FK:booking_id │   │
                                │FK:room_type │   │FK:room_id   │   │
                                └─────┬────────┘   └──────┬───────┘   │
                                      │                   │          │
                                      │                   │          │
                                ┌─────▼────────┐  ┌──────▼─────────┐
                                │rate_plan_    │  │booking_room_  │
                                │  prices      │  │  nights       │
                                │FK:rate_plan  │  │FK:booking_room│
                                └──────────────┘  └────────────────┘
```

---

## 📋 Fluxos Principais

### **1. Fluxo de Reserva**

```
1. POST /api/bookings
   └─ Cria: booking
      ├─ booking_guests (associa hóspedes)
      ├─ booking_rooms (associa quartos)
      └─ booking_room_nights (quebra por noite)

2. GET /api/bookings/{id}
   └─ Retorna: booking + guests + rooms + nights

3. PUT /api/bookings/{id}
   └─ Atualiza: booking
      └─ Cria: booking_status_history (auditoria)
```

### **2. Fluxo de Faturamento**

```
1. POST /api/invoices/{bookingId}
   └─ Gera: invoice + invoice_items
      └─ Consome: erp_integration_logs

2. Webhook ERP → Invoice
   └─ Atualiza: invoice (status, nfse_number, protocol)
```

### **3. Fluxo de Notificações (RabbitMQ)**

```
1. BookingCreated Event
   └─ INSERT: integration_event_outbox
   └─ Worker consome → notification_logs
   └─ Envia: e-mail de confirmação

2. InvoiceCreated Event
   └─ INSERT: integration_event_outbox
   └─ Worker consome → notification_logs
   └─ Envia: e-mail com NFSe
```

---

## 🔐 Segurança

### **Criptografia de PII**

Os dados pessoais dos hóspedes são criptografados usando **AES-GCM**:

- `document_cipher`: Dados criptografados
- `document_nonce`: Nonce único por mensagem
- `document_tag`: Tag de autenticação
- `document_key_version`: Versão da chave

**Fluxo:**
1. Hash SHA256 do valor original para busca
2. Criptografia AES-GCM com chave derivada via Argon2id
3. Armazenamento seguro no banco

### **Hashing para Busca**

Campos sensíveis são indexados via hash SHA256:
- `email_sha256`
- `phone_sha256`
- `document_sha256`

Isso permite buscar sem descriptografar.

---

## 🗂️ Índices Estratégicos

### **Performance**

```sql
-- Consultas de disponibilidade
CREATE INDEX ix_rooms_hotel_type_status ON rooms(hotel_id, room_type_id, status);
CREATE INDEX ix_bookings_hotel_dates ON bookings(hotel_id, check_in_date, check_out_date, status);

-- Busca de PII (hashes)
CREATE INDEX ix_guest_pii_email_sha256 ON guest_pii(email_sha256);
CREATE INDEX ix_guest_pii_phone_sha256 ON guest_pii(phone_sha256);
CREATE INDEX ix_guest_pii_document_sha256 ON guest_pii(document_sha256);

-- Auditoria
CREATE INDEX ix_audit_hotel ON audit_logs(hotel_id, created_at);
CREATE INDEX ix_audit_entity ON audit_logs(entity_name, entity_id, created_at);

-- Eventos
CREATE INDEX ix_outbox_status ON integration_event_outbox(status, created_at);
CREATE INDEX ix_inbox_event_id ON integration_event_inbox(event_id_ext) UNIQUE;
```

---

## 📊 Métricas e KPIs

O banco suporta cálculos de:

- **Taxa de Ocupação**: `rooms` × `bookings` com status CONFIRMED
- **Receita**: Soma de `booking_payments` com status PAID
- **ADR (Average Daily Rate)**: `booking_room_nights.price_amount`
- **Check-ins/Check-outs**: Status do `booking`
- **Notas Fiscais Emitidas**: `invoices` com status ISSUED

---

## 🚀 Próximos Passos

1. **Criar Migration**: 
   ```bash
   dotnet ef migrations add InitialSchema
   ```

2. **Aplicar ao Banco**:
   ```bash
   dotnet ef database update
   ```

3. **Seed de Dados**:
   - Roles (Admin, User, Gestor)
   - Amenities padrão
   - Templates de notificação

4. **Configurar RabbitMQ** para eventos

5. **Integrar Redis** para cache de disponibilidade

---

## 📝 Convenções

- **IDs**: `CHAR(36)` (GUIDs formatados)
- **Datas**: `DATETIME(6)` com precisão de microsegundos
- **Decimais**: `DECIMAL(12,2)` para valores monetários
- **Booleanos**: `TINYINT(1)` (0/1)
- **Múltiplas Keys**: Usar `[Key]` annotation em ambas propriedades
- **Soft Delete**: Campo `status` em vez de DELETE físico
- **Timestamps**: `created_at`, `updated_at` automáticos

---

**Documento gerado automaticamente com base no schema SQL fornecido.**

