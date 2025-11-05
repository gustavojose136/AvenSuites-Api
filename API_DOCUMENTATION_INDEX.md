# 📚 AvenSuites API - Índice de Documentação

## 🎯 Guia Rápido

Escolha a documentação apropriada para sua necessidade:

| Documentação | Público | Quando Usar |
|--------------|---------|-------------|
| **📖 Documentação Completa** | Desenvolvedores | Detalhes completos de cada endpoint |
| **⚡ Referência Rápida** | Todos | Consulta rápida de endpoints |
| **🏠 Portal do Hóspede** | Frontend | Implementar portal do cliente |
| **📮 Guia Postman** | QA/Testes | Testar API com Postman/Insomnia |

---

## 📄 Documentações Disponíveis

### 1. 📖 API_ENDPOINTS_COMPLETE_DOCS.md
**👉 Documentação Técnica Completa**

**Contém:**
- ✅ Todos os 52 endpoints detalhados
- ✅ Request/Response completos com exemplos
- ✅ Códigos de status HTTP
- ✅ Estrutura do JWT
- ✅ Exemplos de uso com cURL
- ✅ Tabela de resumo por módulo

**Use quando:**
- Precisar de detalhes técnicos completos
- Implementar integração com a API
- Consultar formato exato de requests/responses
- Entender regras de autorização

---

### 2. ⚡ API_QUICK_REFERENCE.md
**👉 Referência Rápida de Consulta**

**Contém:**
- ✅ Tabela de todos os endpoints
- ✅ Métodos HTTP e roles necessários
- ✅ IDs úteis do seed data
- ✅ Status de quartos e reservas
- ✅ Headers obrigatórios
- ✅ Exemplos rápidos de cURL

**Use quando:**
- Precisa encontrar um endpoint rapidamente
- Quer ver todos os endpoints de um módulo
- Precisa lembrar IDs de teste
- Consulta rápida durante desenvolvimento

---

### 3. 🏠 GUEST_PORTAL_API_DOCS.md
**👉 Documentação para Frontend - Portal do Hóspede**

**Contém:**
- ✅ Todos os endpoints do portal do hóspede
- ✅ Fluxo de registro e login
- ✅ Exemplos de código JavaScript
- ✅ Estrutura do token JWT para guests
- ✅ Sugestões de UI/UX
- ✅ Tratamento de erros

**Use quando:**
- Implementar frontend do portal do cliente
- Criar tela de registro de hóspede
- Implementar área do cliente
- Entender fluxo de autenticação de guests

---

### 4. 📮 POSTMAN_COLLECTION_GUIDE.md
**👉 Guia para Testes com Postman/Insomnia**

**Contém:**
- ✅ Estrutura de collection organizada
- ✅ Variáveis de ambiente
- ✅ Exemplos de requests prontos
- ✅ Scripts de teste automatizados
- ✅ Fluxo completo de teste
- ✅ Pre-request scripts úteis

**Use quando:**
- Testar a API manualmente
- Criar collection Postman
- Automatizar testes
- Validar fluxos completos
- Debug de problemas

---

## 🚀 Começando Rápido

### Se você é...

#### 👨‍💻 Desenvolvedor Backend
1. Leia: `API_ENDPOINTS_COMPLETE_DOCS.md`
2. Teste: `POSTMAN_COLLECTION_GUIDE.md`
3. Consulta: `API_QUICK_REFERENCE.md`

#### 👩‍💻 Desenvolvedor Frontend
1. Comece: `GUEST_PORTAL_API_DOCS.md` (se for portal)
2. Consulte: `API_ENDPOINTS_COMPLETE_DOCS.md`
3. Teste: Use Postman para entender respostas

#### 🧪 QA/Tester
1. Setup: `POSTMAN_COLLECTION_GUIDE.md`
2. Referência: `API_QUICK_REFERENCE.md`
3. Validação: `API_ENDPOINTS_COMPLETE_DOCS.md`

#### 📊 Product Owner/Manager
1. Visão Geral: `API_QUICK_REFERENCE.md`
2. Detalhes: `API_ENDPOINTS_COMPLETE_DOCS.md`

---

## 📑 Documentações Técnicas Adicionais

### Implementação e Mudanças

| Arquivo | Descrição |
|---------|-----------|
| `GUEST_SYSTEM_CHANGELOG.md` | Mudanças técnicas do sistema de hóspedes |
| `IMPLEMENTACAO_PORTAL_HOSPEDE.md` | Resumo da implementação do portal |
| `ALTERACOES_JWT.md` | Mudanças no token JWT |
| `SOLUCAO_ERRO_HOTELID.md` | Troubleshooting de erros |

---

## 🔍 Como Encontrar o que Precisa

### Por Módulo:

**🔐 Autenticação**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 1
- Rápida: API_QUICK_REFERENCE.md → Seção 1
- Hóspede: GUEST_PORTAL_API_DOCS.md → Seção 1-2

**🏨 Hotéis**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 2
- Rápida: API_QUICK_REFERENCE.md → Seção 2
- Postman: POSTMAN_COLLECTION_GUIDE.md → Seção 2

**🛏️ Quartos**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 3
- Rápida: API_QUICK_REFERENCE.md → Seção 3
- Postman: POSTMAN_COLLECTION_GUIDE.md → Seção 3

**👤 Hóspedes**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 5
- Rápida: API_QUICK_REFERENCE.md → Seção 5
- Portal: GUEST_PORTAL_API_DOCS.md → Completo
- Postman: POSTMAN_COLLECTION_GUIDE.md → Seção 4

**📅 Reservas**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 6
- Rápida: API_QUICK_REFERENCE.md → Seção 6
- Portal: GUEST_PORTAL_API_DOCS.md → Seção 3-7
- Postman: POSTMAN_COLLECTION_GUIDE.md → Seção 5

**🧾 Notas Fiscais**
- Completa: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 7
- Rápida: API_QUICK_REFERENCE.md → Seção 7
- Postman: POSTMAN_COLLECTION_GUIDE.md → Seção 6

---

## 📊 Estatísticas da API

### Endpoints por Módulo

```
┌─────────────────────┬──────────┐
│ Módulo              │ Endpoints│
├─────────────────────┼──────────┤
│ Autenticação        │    4     │
│ Hotéis              │    6     │
│ Quartos             │    7     │
│ Tipos de Quarto     │    5     │
│ Hóspedes            │    6     │
│ Reservas            │   11     │
│ Notas Fiscais       │    5     │
│ Usuários            │    3     │
│ Portal do Hóspede   │    5     │
├─────────────────────┼──────────┤
│ TOTAL               │   52     │
└─────────────────────┴──────────┘
```

### Métodos HTTP

```
┌────────┬───────┐
│ Método │ Count │
├────────┼───────┤
│ GET    │  24   │
│ POST   │  17   │
│ PUT    │   7   │
│ DELETE │   4   │
└────────┴───────┘
```

---

## 🎓 Tutoriais por Caso de Uso

### 1. Implementar Autenticação
```
1. Ler: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 1
2. Ver: GUEST_PORTAL_API_DOCS.md → Exemplos JavaScript
3. Testar: POSTMAN_COLLECTION_GUIDE.md → Auth
```

### 2. Criar Sistema de Reservas
```
1. Ler: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 6
2. Consultar: API_QUICK_REFERENCE.md → Reservas
3. Testar: POSTMAN_COLLECTION_GUIDE.md → Fluxo Completo
```

### 3. Portal do Cliente
```
1. Ler: GUEST_PORTAL_API_DOCS.md → Completo
2. Testar: POSTMAN_COLLECTION_GUIDE.md → Guest Portal
3. Referência: API_QUICK_REFERENCE.md → Portal
```

### 4. Integração com NF-e
```
1. Ler: API_ENDPOINTS_COMPLETE_DOCS.md → Seção 7
2. Testar: POSTMAN_COLLECTION_GUIDE.md → Invoices
3. Debug: Logs da aplicação
```

---

## 🔗 Links Rápidos

### Endpoints Mais Usados

| Ação | Link Documentação |
|------|-------------------|
| Login | [API_ENDPOINTS_COMPLETE_DOCS.md#11-login](./API_ENDPOINTS_COMPLETE_DOCS.md) |
| Listar Hotéis | [API_ENDPOINTS_COMPLETE_DOCS.md#21-listar-hotéis](./API_ENDPOINTS_COMPLETE_DOCS.md) |
| Criar Reserva | [API_ENDPOINTS_COMPLETE_DOCS.md#66-criar-reserva](./API_ENDPOINTS_COMPLETE_DOCS.md) |
| Check Availability | [API_ENDPOINTS_COMPLETE_DOCS.md#34-verificar-disponibilidade](./API_ENDPOINTS_COMPLETE_DOCS.md) |
| Registrar Hóspede | [GUEST_PORTAL_API_DOCS.md#registro](./GUEST_PORTAL_API_DOCS.md) |

---

## ⚙️ Configuração Base

### URLs da API

```
Desenvolvimento: https://localhost:7000
Produção: https://api.avensuites.com
```

### IDs de Teste (Seed Data)

```yaml
Hotel Avenida: 7a326969-3bf6-40d9-96dc-1aecef585000
Admin User: 2975cf19-0baa-4507-9f98-968760deb546
Hotel-Admin (Gustavo): f36d8acd-1822-4019-ac76-a6ea959d5193
Quarto 101: 40d5718c-dbda-40c7-a4f4-644cd6f177bd
Hóspede (Joni): 87f086dd-d461-49c8-a63c-1fc7b6a55441
```

### Credenciais Padrão

```yaml
Admin:
  Email: admin@avensuites.com
  Password: Admin123!

Hotel-Admin:
  Email: gjose2980@gmail.com
  Password: Admin123!
```

---

## 📞 Suporte e Contribuição

### Reportar Problemas
- Issues no repositório
- Email da equipe de desenvolvimento

### Sugerir Melhorias
- Pull requests são bem-vindos
- Discutir na issue primeiro para grandes mudanças

---

## 📝 Notas de Versão

### Versão Atual: 1.0

**Novidades:**
- ✅ Sistema completo de autenticação
- ✅ Portal do hóspede (self-service)
- ✅ Gestão de hotéis, quartos e reservas
- ✅ Integração com IPM NF-e
- ✅ Role-based access control (RBAC)
- ✅ JWT com claims customizados

**Próximas Versões:**
- 🔜 Reset de senha
- 🔜 Notificações por email
- 🔜 Upload de imagens
- 🔜 Relatórios e dashboards

---

## 🎯 Checklist de Implementação

### Backend
- [ ] Ler documentação completa
- [ ] Configurar ambiente local
- [ ] Testar todos os endpoints no Postman
- [ ] Implementar tratamento de erros
- [ ] Adicionar logs

### Frontend
- [ ] Ler documentação do Portal do Hóspede
- [ ] Implementar autenticação
- [ ] Criar telas principais
- [ ] Adicionar validações
- [ ] Testar fluxos completos

### QA
- [ ] Criar collection Postman
- [ ] Testar todos os endpoints
- [ ] Validar roles e permissões
- [ ] Testar casos de erro
- [ ] Documentar bugs encontrados

---

## 📚 Recursos Adicionais

- **Swagger/OpenAPI:** Disponível em `/swagger` (quando habilitado)
- **Postman Collection:** Criar usando `POSTMAN_COLLECTION_GUIDE.md`
- **Changelog:** Ver `GUEST_SYSTEM_CHANGELOG.md`

---

**Última Atualização:** 31/10/2025  
**Versão da API:** 1.0  
**Framework:** .NET 9.0

