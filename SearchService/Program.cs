using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SearchService.Configuration;
using SearchService.Documents;
using SearchService.Infrastructure;
using SearchService.Infrastructure.ErrorHandling;
using Serilog;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
);
var compositeTextMapPropagator = new CompositeTextMapPropagator(new TextMapPropagator[]
{
    new TraceContextPropagator(),
    new BaggagePropagator()
});
Sdk.SetDefaultTextMapPropagator(compositeTextMapPropagator);
var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpExporter:Endpoint"];

builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("SearchService"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRabbitMQInstrumentation()
            .AddOtlpExporter(o =>
            {
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    o.Endpoint = new Uri(otlpEndpoint);
            });
    });
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSearchServiceDependencies();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
    var options = new ConfigurationOptions
    {
        EndPoints = { $"{redisSettings!.Host}:{redisSettings.Port}" },
        Password = redisSettings.Password,
        AbortOnConnectFail = false
    };

    return ConnectionMultiplexer.Connect(options);
});

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseMiddleware<VisitorTrackingMiddleware>();
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseCors("AllowOrigins");

app.MapControllers();
app.MapMetrics();
app.Run();
