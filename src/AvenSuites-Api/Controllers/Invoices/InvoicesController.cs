using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Invoices;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IIpmNfseService _ipmNfseService;
    private readonly ICurrentUserService _currentUser;

    public InvoicesController(
        IIpmNfseService ipmNfseService,
        ICurrentUserService currentUser)
    {
        _ipmNfseService = ipmNfseService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Cria uma nota fiscal (NF-e) simplificada com base no quarto.
    /// Requer acesso ao hotel do quarto.
    /// </summary>
    [HttpPost("simple/{roomId}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(InvoiceResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<InvoiceResponse>> CreateSimpleInvoice(
        Guid roomId,
        [FromBody] SimpleInvoiceCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var invoice = await _ipmNfseService.GenerateSimpleInvoiceAsync(roomId, request);
            
            if (!invoice.Success)
                return BadRequest(invoice);

            return Ok(invoice);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cria uma nota fiscal (NF-e) completa.
    /// Requer acesso ao hotel.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IpmNfseCreateResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IpmNfseCreateResponse>> CreateInvoice(Guid hotelId, [FromBody] IpmNfseCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        try
        {
            var invoice = await _ipmNfseService.GenerateInvoiceAsync(hotelId, request);
            
            if (!invoice.Success)
                return BadRequest(invoice);

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancela uma nota fiscal no IPM.
    /// Requer acesso ao hotel.
    /// </summary>
    [HttpPost("{hotelId}/cancel")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IpmNfseCreateResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IpmNfseCreateResponse>> Cancel(Guid hotelId, [FromBody] IpmNfseCancelRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var result = await _ipmNfseService.CancelInvoiceAsync(hotelId, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Busca nota fiscal por código de verificação.
    /// </summary>
    [HttpGet("{hotelId}/verification/{verificationCode}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IpmNfseCreateResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IpmNfseCreateResponse>> GetByVerificationCode(Guid hotelId, string verificationCode)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var result = await _ipmNfseService.GetInvoiceByVerificationCodeAsync(hotelId, verificationCode);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Busca nota fiscal por número e série.
    /// </summary>
    [HttpGet("{hotelId}/number/{nfseNumber}/serie/{serie}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IpmNfseCreateResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IpmNfseCreateResponse>> GetByNumber(Guid hotelId, string nfseNumber, string serie)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var result = await _ipmNfseService.GetInvoiceByNumberAsync(hotelId, nfseNumber, serie);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}

