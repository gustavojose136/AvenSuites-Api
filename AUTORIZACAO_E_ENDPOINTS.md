# 🔐 Sistema de Autorização e Endpoints - AvenSuites API

## ✅ Implementações Concluídas

### 1. **Novo Role "Hotel-Admin"**
- ✅ Criado role `Hotel-Admin` no seed
- ✅ Role permite acesso apenas aos dados do hotel específico
- ✅ Gustavo (gjose2980@gmail.com) configurado como Hotel-Admin do "Hotel Avenida"

**Seed atualizado:**
```csharp
new Role {
    Id = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
    Name = "Hotel-Admin",
    Description = "Hotel administrator role with access to specific hotel only"
}
```

### 2. **Serviço de Contexto do Usuário (ICurrentUserService)**

Criado serviço para extrair informações do token JWT:

**Interface:**
- `GetUserId()` - Retorna ID do usuário logado
- `GetUserEmail()` - Retorna email do usuário
- `GetUserHotelId()` - Retorna ID do hotel do usuário (se Hotel-Admin)
- `GetUserRoles()` - Retorna lista de roles
- `IsAdmin()` - Verifica se é Admin  
- `IsHotelAdmin()` - Verifica se é Hotel-Admin
- `HasAccessToHotel(hotelId)` - Verifica se tem acesso ao hotel

**Lógica de Acesso:**
- **Admin**: Acesso a TODOS os hotéis
- **Hotel-Admin**: Acesso apenas ao próprio hotel
- **User**: Sem acesso administrativo

### 3. **Configuração**
- ✅ `HttpContextAccessor` adicionado ao Program.cs
- ✅ `ICurrentUserService` registrado no DependencyInjection

---

## 📋 Endpoints Existentes

### ✅ **Autenticação**
- `POST /Auth/login` - Login (retorna JWT)
- `POST /Auth/register` - Registro
- `POST /Auth/validate` - Validar credenciais

### ✅ **Hotéis (HotelsController)**
- `POST /Hotel` - Criar hotel
- `GET /Hotel/{id}` - Buscar por ID
- `GET /Hotel/cnpj/{cnpj}` - Buscar por CNPJ
- `GET /Hotel` - Listar todos
- `PUT /Hotel/{id}` - Atualizar
- `DELETE /Hotel/{id}` - Deletar

### ✅ **Quartos (RoomsController)**
- `POST /Room` - Criar quarto
- `GET /Room/{id}` - Buscar por ID
- `GET /Room/hotel/{hotelId}` - Listar por hotel
- `GET /Room/availability` - Verificar disponibilidade
- `PUT /Room/{id}` - Atualizar
- `DELETE /Room/{id}` - Deletar

### ✅ **Hóspedes (GuestsController)**
- `POST /Guest` - Criar hóspede
- `GET /Guest/{id}` - Buscar por ID
- `GET /Guest/hotel/{hotelId}` - Listar por hotel
- `PUT /Guest/{id}` - Atualizar
- `DELETE /Guest/{id}` - Deletar

### ✅ **Reservas (BookingsController)**
- `POST /Booking` - Criar reserva
- `GET /Booking/{id}` - Buscar por ID
- `GET /Booking/code/{code}` - Buscar por código
- `GET /Booking/hotel/{hotelId}` - Listar por hotel
- `GET /Booking/guest/{guestId}` - Listar por hóspede
- `PUT /Booking/{id}` - Atualizar
- `POST /Booking/{id}/cancel` - Cancelar
- `POST /Booking/{id}/confirm` - Confirmar

### ✅ **Notas Fiscais (InvoicesController)**
- `POST /Invoice/simple/{roomId}` - Criar NF-e simplificada
- Outros endpoints precisam ser verificados

---

## ❌ Endpoints Faltando (Conforme Documentação)

### 🔴 **Prioridade Alta**

#### **Autenticação**
- ❌ `LoginResponse` precisa incluir `roles` e `image`
- ❌ `LoginResponse` precisa incluir `expiresAt`

#### **Quartos**
- ❌ `GET /Room` - Listar TODOS os quartos (atualmente só por hotel)
  - Implementar query parameters: `?hotelId=&status=&type=`

#### **Hóspedes**
- ❌ `GET /Guest` - Listar TODOS os hóspedes (atualmente só por hotel)
  - Implementar filtros

#### **Reservas**
- ❌ `GET /Booking` - Listar TODAS as reservas (atualmente só por hotel/guest)
  - Implementar query parameters: `?hotelId=&guestId=&status=&startDate=&endDate=`
- ❌ `POST /Booking/{id}/check-in` - Realizar check-in
- ❌ `POST /Booking/{id}/check-out` - Realizar check-out

#### **Notas Fiscais**
- ❌ `GET /Invoice` - Listar todas as notas fiscais
  - Implementar query parameters: `?status=&startDate=&endDate=&guestId=`
- ❌ `GET /Invoice/{id}` - Buscar nota fiscal por ID
- ❌ `POST /Invoice` - Criar nota fiscal completa
- ❌ `PUT /Invoice/{id}` - Atualizar nota fiscal
- ❌ `POST /Invoice/{id}/pay` - Registrar pagamento

---

## 🚀 Próximos Passos de Implementação

### **Fase 1: Atualizar Controllers com Autorização**

Todos os controllers precisam:
1. Injetar `ICurrentUserService`
2. Validar acesso por hotel
3. Filtrar dados baseado no role

**Exemplo de implementação:**

```csharp
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ICurrentUserService _currentUser;

    public HotelsController(
        IHotelService _hotelService,
        ICurrentUserService currentUser)
    {
        _hotelService = hotelService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    public async Task<ActionResult<IEnumerable<HotelResponse>>> GetAll()
    {
        if (_currentUser.IsAdmin())
        {
            // Admin vê todos os hotéis
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }
        
        if (_currentUser.IsHotelAdmin())
        {
            // Hotel-Admin vê apenas o próprio hotel
            var hotelId = _currentUser.GetUserHotelId();
            if (!hotelId.HasValue)
                return Forbid();
                
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId.Value);
            return Ok(new[] { hotel });
        }
        
        return Forbid();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    public async Task<ActionResult<HotelResponse>> GetById(Guid id)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(id))
            return Forbid();

        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HotelResponse>> Create([FromBody] HotelCreateRequest request)
    {
        // Apenas Admin pode criar hotéis
        var hotel = await _hotelService.CreateHotelAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    public async Task<ActionResult<HotelResponse>> Update(Guid id, [FromBody] HotelCreateRequest request)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(id))
            return Forbid();

        var hotel = await _hotelService.UpdateHotelAsync(id, request);
        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Apenas Admin pode deletar hotéis
        var result = await _hotelService.DeleteHotelAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
```

### **Fase 2: Adicionar Endpoints Faltantes**

#### **1. BookingController - Check-in/Check-out**
```csharp
[HttpPost("{id}/check-in")]
[Authorize(Roles = "Admin,Hotel-Admin")]
public async Task<ActionResult> CheckIn(Guid id)
{
    var booking = await _bookingService.GetBookingByIdAsync(id);
    if (booking == null)
        return NotFound();

    // Verificar se tem acesso ao hotel
    if (!_currentUser.HasAccessToHotel(booking.HotelId))
        return Forbid();

    var result = await _bookingService.CheckInAsync(id);
    if (!result)
        return BadRequest(new { message = "Não foi possível realizar check-in" });

    return Ok(new { message = "Check-in realizado com sucesso", booking });
}

[HttpPost("{id}/check-out")]
[Authorize(Roles = "Admin,Hotel-Admin")]
public async Task<ActionResult> CheckOut(Guid id)
{
    var booking = await _bookingService.GetBookingByIdAsync(id);
    if (booking == null)
        return NotFound();

    // Verificar se tem acesso ao hotel
    if (!_currentUser.HasAccessToHotel(booking.HotelId))
        return Forbid();

    var result = await _bookingService.CheckOutAsync(id);
    if (!result)
        return BadRequest(new { message = "Não foi possível realizar check-out" });

    return Ok(new { message = "Check-out realizado com sucesso", booking });
}
```

#### **2. InvoiceController - Endpoints Faltantes**
```csharp
[HttpGet]
[Authorize(Roles = "Admin,Hotel-Admin")]
public async Task<ActionResult<IEnumerable<InvoiceResponse>>> GetAll(
    [FromQuery] string? status = null,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] Guid? guestId = null)
{
    if (_currentUser.IsAdmin())
    {
        // Admin vê todas as invoices
        var invoices = await _invoiceService.GetAllAsync(status, startDate, endDate, guestId);
        return Ok(invoices);
    }
    
    if (_currentUser.IsHotelAdmin())
    {
        var hotelId = _currentUser.GetUserHotelId();
        if (!hotelId.HasValue)
            return Forbid();
            
        // Hotel-Admin vê apenas invoices do próprio hotel
        var invoices = await _invoiceService.GetByHotelAsync(hotelId.Value, status, startDate, endDate, guestId);
        return Ok(invoices);
    }
    
    return Forbid();
}

[HttpPost("{id}/pay")]
[Authorize(Roles = "Admin,Hotel-Admin")]
public async Task<ActionResult> Pay(Guid id, [FromBody] PayInvoiceRequest request)
{
    var invoice = await _invoiceService.GetByIdAsync(id);
    if (invoice == null)
        return NotFound();

    // Verificar se tem acesso ao hotel
    if (!_currentUser.HasAccessToHotel(invoice.HotelId))
        return Forbid();

    var result = await _invoiceService.PayAsync(id, request);
    if (!result)
        return BadRequest(new { message = "Não foi possível registrar o pagamento" });

    return Ok(new { message = "Pagamento registrado com sucesso", invoice });
}
```

### **Fase 3: Implementar Filtros e Paginação**

Criar DTOs para paginação:

```csharp
public class PaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "createdAt";
    public string? SortOrder { get; set; } = "desc";
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
```

---

## 🔑 Atualizar JWT Service para incluir HotelId

O `JwtService` precisa incluir o `HotelId` nas claims do token para Hotel-Admin:

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Name, user.Name)
};

// Adicionar roles
foreach (var role in user.Roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role.Name));
}

// Se for Hotel-Admin, adicionar HotelId
if (user.Roles.Any(r => r.Name == "Hotel-Admin"))
{
    var hotelId = await _userRepository.GetUserHotelIdAsync(user.Id);
    if (hotelId.HasValue)
    {
        claims.Add(new Claim("HotelId", hotelId.Value.ToString()));
    }
}
```

---

## 📝 Checklist de Implementação

### ✅ Fase 1 - Infraestrutura (Concluído)
- [x] Criar role "Hotel-Admin"
- [x] Criar `ICurrentUserService`
- [x] Configurar `HttpContextAccessor`
- [x] Registrar serviços

### ⏳ Fase 2 - Atualizar Controllers (Pendente)
- [ ] Atualizar `HotelsController` com autorização
- [ ] Atualizar `RoomsController` com filtros e autorização
- [ ] Atualizar `GuestsController` com filtros e autorização
- [ ] Atualizar `BookingsController` com check-in/check-out
- [ ] Atualizar `InvoicesController` com endpoints faltantes

### ⏳ Fase 3 - Atualizar Services (Pendente)
- [ ] Adicionar `CheckInAsync` e `CheckOutAsync` no `IBookingService`
- [ ] Adicionar `GetAllAsync` e `PayAsync` no `IInvoiceService`
- [ ] Implementar filtros nos métodos dos services

### ⏳ Fase 4 - Atualizar JWT (Pendente)
- [ ] Adicionar `HotelId` ao token para Hotel-Admin
- [ ] Incluir `roles` e `image` no `LoginResponse`
- [ ] Adicionar `expiresAt` no `LoginResponse`

### ⏳ Fase 5 - Endpoints GET Globais (Pendente)
- [ ] `GET /Room` - listar todos os quartos (com filtros)
- [ ] `GET /Guest` - listar todos os hóspedes (com filtros)
- [ ] `GET /Booking` - listar todas as reservas (com filtros)
- [ ] `GET /Invoice` - listar todas as notas fiscais (com filtros)

### ⏳ Fase 6 - Paginação (Pendente)
- [ ] Implementar paginação em endpoints de listagem
- [ ] Criar DTOs `PaginatedRequest` e `PaginatedResponse<T>`

---

## 🎯 Resumo

**O que foi feito:**
1. ✅ Criado role "Hotel-Admin"
2. ✅ Implementado `ICurrentUserService` para extrair info do JWT
3. ✅ Configurada infraestrutura base
4. ✅ Documentado todos os endpoints existentes e faltantes

**O que precisa ser feito:**
1. ❌ Atualizar TODOS os controllers para usar `ICurrentUserService`
2. ❌ Adicionar validação de acesso por hotel em TODOS os endpoints
3. ❌ Implementar endpoints faltantes (check-in, check-out, pay invoice)
4. ❌ Adicionar endpoints GET globais com filtros
5. ❌ Atualizar JwtService para incluir HotelId no token
6. ❌ Implementar paginação
7. ❌ Criar migration para o novo role

**Observação:** Todos os controllers e services precisam ser atualizados para implementar a lógica de autorização por hotel. Isso é uma mudança extensa que afeta toda a aplicação.

