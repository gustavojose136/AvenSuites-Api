using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Invoices;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IIpmNfseService _ipmNfseService;

    public InvoicesController(IIpmNfseService ipmNfseService)
    {
        _ipmNfseService = ipmNfseService;
    }

    [HttpPost("{hotelId}/nfse/generate")]
    public async Task<ActionResult<IpmNfseCreateResponse>> GenerateNfse(Guid hotelId, [FromBody] IpmNfseCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _ipmNfseService.GenerateInvoiceAsync(hotelId, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{hotelId}/nfse/cancel")]
    public async Task<ActionResult<IpmNfseCreateResponse>> CancelNfse(Guid hotelId, [FromBody] IpmNfseCancelRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _ipmNfseService.CancelInvoiceAsync(hotelId, request);
        
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{hotelId}/nfse/verification/{verificationCode}")]
    public async Task<ActionResult<IpmNfseCreateResponse>> GetByVerificationCode(Guid hotelId, string verificationCode)
    {
        var result = await _ipmNfseService.GetInvoiceByVerificationCodeAsync(hotelId, verificationCode);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{hotelId}/nfse/{nfseNumber}/serie/{serie}")]
    public async Task<ActionResult<IpmNfseCreateResponse>> GetByNumber(Guid hotelId, string nfseNumber, string serie)
    {
        var result = await _ipmNfseService.GetInvoiceByNumberAsync(hotelId, nfseNumber, serie);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}

