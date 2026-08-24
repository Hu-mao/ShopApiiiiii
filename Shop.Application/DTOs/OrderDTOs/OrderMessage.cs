namespace Shop.Application.DTOs.OrderDTOs;

public class OrderMessage
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public List<OrderProductDTO> Products { get; set; } = new();
}