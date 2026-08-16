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
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [RequirePermission(Permission.Product.Read)]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<ProductResponse> result = await _productService.GetAllAsync();
        return Ok(result);
    }

    [RequirePermission(Permission.Product.Read)]
    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        ProductResponse? result = await _productService.GetByIdAsync(id);
        if (result is null) return NotFound();

        return Ok(result);
    }

    // Looks up a single product by barcode or QR code, e.g. for a handheld scanner.
    [RequirePermission(Permission.Product.Read)]
    [HttpGet("lookup")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Lookup([FromQuery] string code)
    {
        ProductResponse? result = await _productService.LookupAsync(code);
        if (result is null) return NotFound(new { message = "No product matches that code." });

        return Ok(result);
    }

    [RequirePermission(Permission.Product.Write)]
    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        ProductResponse result = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [RequirePermission(Permission.Product.Write)]
    [HttpPut("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        ProductResponse result = await _productService.UpdateAsync(id, request);
        return Ok(result);
    }

    [RequirePermission(Permission.Product.Delete)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _productService.DeactivateAsync(id);
        return NoContent();
    }
}
