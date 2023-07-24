using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess;

public class DataBaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;

    public DataBaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        _configureDbContext = configureDbContext;
    }

    public DatabaseContext CreateDbContext()
    {
        DbContextOptionsBuilder<DatabaseContext> options = new();

        _configureDbContext(options);

        return new DatabaseContext(options.Options);
    }
}