using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using EventHandler = System.EventHandler;

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

    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
    builder.Services.AddScoped<IEventConsumer, EventConsumer>();
    builder.Services.AddHostedService<ConsumerHostedService>();
    builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
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
