using DiscountManager.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountManager.Server;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DiscountCode> DiscountCodes { get; set; }
}
