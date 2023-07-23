using CQRS.Core.Domain;
using CQRS.Core.Infrastructure;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
    builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
    builder.Services.AddScoped<IEventStore, EventStore>();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
