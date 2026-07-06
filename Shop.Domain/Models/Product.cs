using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("products")]
public class Product:BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column("stock_qty")]
    public int StockQty { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;


    // FK до категорії
    [Required]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    // Navigation properties
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}