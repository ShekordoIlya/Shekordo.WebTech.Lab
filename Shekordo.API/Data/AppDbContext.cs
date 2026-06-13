using Microsoft.EntityFrameworkCore;
using Shekordo.Domain.Entities;

namespace Shekordo.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Dish> Dishes => Set<Dish>();
}
