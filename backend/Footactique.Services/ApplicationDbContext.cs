using Microsoft.EntityFrameworkCore;

namespace Footactique.Services
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
    }
}
