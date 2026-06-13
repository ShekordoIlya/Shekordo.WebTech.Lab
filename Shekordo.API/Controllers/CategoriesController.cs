using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shekordo.API.Data;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ResponseData<List<Category>>>> GetCategories()
    {
        return new ResponseData<List<Category>>
        {
            Data = await context.Categories.ToListAsync()
        };
    }
}
