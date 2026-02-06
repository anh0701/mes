using Microsoft.EntityFrameworkCore;

public class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options)
        : base(options) { }

    public DbSet<Plant> Plant => Set<Plant>();
    public DbSet<Line> Line => Set<Line>();
    public DbSet<Machine> Machine => Set<Machine>();
}
