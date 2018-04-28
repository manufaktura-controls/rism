using Microsoft.EntityFrameworkCore;

namespace Manufaktura.RismCatalogue.Model
{
    public class RismDbContext : DbContext
    {
        public RismDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}