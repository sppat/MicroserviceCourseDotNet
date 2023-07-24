using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

var builder = WebApplication.CreateBuilder(args);

{
    void ConfigureDbContext(DbContextOptionsBuilder options) =>
        options.UseLazyLoadingProxies()
            .UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));

    builder.Services.AddDbContext<DatabaseContext>((Action<DbContextOptionsBuilder>)ConfigureDbContext);
    builder.Services.AddSingleton(new DataBaseContextFactory(ConfigureDbContext));

    var dataContext = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<DatabaseContext>();

    dataContext.Database.EnsureCreated();
    
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
}

var app = builder.Build();

{
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
