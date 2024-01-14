using FerozeHub.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FerozeHub.Services.EmailAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):DbContext(options)
{
    public DbSet<EmailLogger> EmailLoggers { get;set; }
    
}