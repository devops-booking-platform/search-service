using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Configuration;
using SearchService.Documents;
using SearchService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSearchServiceDependencies();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opt.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(opt.Database);
});

builder.Services.AddSingleton<IMongoCollection<AccommodationDocument>>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<AccommodationDocument>(opt.AccommodationsCollection);
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowOrigins");
app.MapControllers();
app.Run();
