using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shekordo.API.Data;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<ResponseData<List<Category>>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            var response = new ResponseData<List<Category>>
            {
                Data = categories
            };

            return response;
        }
    }
}