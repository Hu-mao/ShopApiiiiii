using MediatR;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Queries.GetProductById;

public record GetProductByIdQuery(int id) : IRequest<ProductReadDTO?>;