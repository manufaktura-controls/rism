using Microsoft.EntityFrameworkCore;

namespace Manufaktura.RismCatalogue.Model
{
    public class RismDbContext : DbContext
    {
        public RismDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Incipit> Incipits { get; set; }
        public DbSet<MusicalSource> MusicalSources { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<UniformTitle> UniformTitles { get; set; }

        public DbSet<SpatialHash> SpatialHashes { get; set; }

        //public DbSet<SpatialHashIncipit> SpatialHashIncipits { get; set; }

        public DbSet<Plane> Planes { get; set; }
    }
}