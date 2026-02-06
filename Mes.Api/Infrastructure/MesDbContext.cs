using Microsoft.EntityFrameworkCore;

public class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options)
        : base(options) { }

    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Line> Lines => Set<Line>();
    public DbSet<Machine> Machines => Set<Machine>();
}
