using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.AdminDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(
    IAdminService _adminService) : ControllerBase
{
    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(
        CreateStaffDTO dto)
    {
        await _adminService.CreateStaffAsync(dto);

        return Ok(new
        {
            message = "Staff member created successfully"
        });
    }
}