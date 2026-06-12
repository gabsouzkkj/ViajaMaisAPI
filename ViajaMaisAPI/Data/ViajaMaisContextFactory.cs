using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ViajaMaisAPI.Data;

public class ViajaMaisContextFactory : IDesignTimeDbContextFactory<ViajaMaisContext>
{
    public ViajaMaisContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ViajaMaisContext>();

        optionsBuilder.UseSqlite("Data Source=viajamais.db");

        return new ViajaMaisContext(optionsBuilder.Options);
    }
}