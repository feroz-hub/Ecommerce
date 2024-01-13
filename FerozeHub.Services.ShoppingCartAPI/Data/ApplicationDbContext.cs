using FerozeHub.Services.ShoppingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FerozeHub.Services.ShoppingAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
{
    public DbSet<CartHeader> CartHeaders{get; set; }
    public DbSet<CartDetails> CartDetails{get; set; }
}