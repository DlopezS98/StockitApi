using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StockitApi.Authorization;
using StockitApi.Dtos;
using StockitApi.Interfaces;
using StockitApi.Models;

namespace StockitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    [RequirePermission(Permission.Invoice.Read)]
    [HttpGet]
    [ProducesResponseType<PagedResult<InvoiceResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] InvoiceQueryParameters query)
    {
        PagedResult<InvoiceResponse> result = await _invoiceService.GetAllAsync(query);
        return Ok(result);
    }

    [RequirePermission(Permission.Invoice.Read)]
    [HttpGet("{id:int}")]
    [ProducesResponseType<InvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        InvoiceResponse? result = await _invoiceService.GetByIdAsync(id);
        if (result is null) return NotFound();

        return Ok(result);
    }

    [RequirePermission(Permission.Invoice.Create)]
    [HttpPost]
    [ProducesResponseType<InvoiceResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        InvoiceResponse result = await _invoiceService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [RequirePermission(Permission.Invoice.Void)]
    [HttpPost("{id:int}/void")]
    [ProducesResponseType<InvoiceResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Void(int id)
    {
        InvoiceResponse result = await _invoiceService.VoidAsync(id);
        return Ok(result);
    }
}
