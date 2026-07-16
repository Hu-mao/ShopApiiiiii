using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;
//URL - Uniform Resource Locator - текстовий рядок, який вказує
//на місце розташування ресурса

[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductController(IProductService _productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDTO dto)
    {
        var id = await _productService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
}
