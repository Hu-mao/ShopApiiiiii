using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Infrastructure.RabbitMQ;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly RabbitMQProducer _rabbitMQProducer;

    public OrderController(RabbitMQProducer rabbitMQProducer)
    {
        _rabbitMQProducer = rabbitMQProducer;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO dto)
    {
        if (dto.Products == null || dto.Products.Count == 0)
        {
            return BadRequest("Замовлення повинно містити хоча б один продукт.");
        }

        var order = new OrderMessage
        {
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            TotalPrice = dto.TotalPrice,
            Products = dto.Products
        };

        await _rabbitMQProducer.SendOrderAsync(order);

        return Ok(new
        {
            message = "Замовлення додано в чергу Orders"
        });
    }
}