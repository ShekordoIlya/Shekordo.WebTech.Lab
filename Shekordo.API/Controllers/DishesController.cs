using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shekordo.API.Data;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DishesController(AppDbContext context, IWebHostEnvironment env) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ResponseData<ListModel<Dish>>>> GetDishes(
        string? category,
        int pageNo = 1,
        int pageSize = 3)
    {
        var result = new ResponseData<ListModel<Dish>>();

        var data = context.Dishes
            .Include(d => d.Category)
            .Where(d => string.IsNullOrEmpty(category) ||
                        d.Category!.NormalizedName == category);

        var totalCount = await data.CountAsync();
        int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

        if (pageNo > totalPages)
        {
            pageNo = totalPages;
        }

        var listData = new ListModel<Dish>
        {
            Items = await data
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            CurrentPage = pageNo,
            TotalPages = totalPages
        };

        result.Data = listData;

        if (totalCount == 0)
        {
            result.Success = false;
            result.ErrorMessage = "Нет объектов в выбранной категории";
        }

        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Dish>> GetDish(int id)
    {
        var dish = await context.Dishes
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);

        return dish == null ? NotFound() : dish;
    }

    [HttpPost]
    public async Task<ActionResult<Dish>> PostDish(Dish dish)
    {
        context.Dishes.Add(dish);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutDish(int id, Dish dish)
    {
        if (id != dish.Id)
        {
            return BadRequest();
        }

        context.Entry(dish).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await context.Dishes.AnyAsync(d => d.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDish(int id)
    {
        var dish = await context.Dishes.FindAsync(id);
        if (dish == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(dish.Image))
        {
            DeleteImageFile(dish.Image);
        }

        context.Dishes.Remove(dish);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> SaveImage(int id, IFormFile image)
    {
        var dish = await context.Dishes.FindAsync(id);
        if (dish == null)
        {
            return NotFound();
        }

        var imagesPath = Path.Combine(env.WebRootPath, "Images");
        Directory.CreateDirectory(imagesPath);

        var randomName = Path.GetRandomFileName();
        var extension = Path.GetExtension(image.FileName);
        var fileName = Path.ChangeExtension(randomName, extension);
        var filePath = Path.Combine(imagesPath, fileName);

        await using (var stream = System.IO.File.OpenWrite(filePath))
        {
            await image.CopyToAsync(stream);
        }

        if (!string.IsNullOrEmpty(dish.Image))
        {
            DeleteImageFile(dish.Image);
        }

        var host = $"{Request.Scheme}://{Request.Host}";
        dish.Image = $"{host}/Images/{fileName}";
        await context.SaveChangesAsync();

        return Ok();
    }

    private void DeleteImageFile(string imageUrl)
    {
        var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
        var filePath = Path.Combine(env.WebRootPath, "Images", fileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}
