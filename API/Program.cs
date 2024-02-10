using API.Setup;
using Infra.Pedidos;
using Infra.RabbitMQ;
using Domain.RabbitMQ;
using System.Reflection;
using Domain.Configuration;
using Infra.RabbitMQ.Consumers;
using Microsoft.EntityFrameworkCore;
using Application.Pedidos.AutoMapper;
using Swashbuckle.AspNetCore.Filters;
using Application.Catalogo.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Configurando LoggerFactory e criando uma instância de ILogger
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();


// Add services to the container.
builder.Services.AddControllers();

string connectionString = "";
string secret = "";

if (builder.Environment.IsProduction())
{
    logger.LogInformation("Ambiente de Producao detectado.");

    builder.Configuration.AddAmazonSecretsManager("us-west-2", "pedido-secret");

    connectionString = builder.Configuration.GetSection("ConnectionString").Value ?? string.Empty;

    secret = builder.Configuration.GetSection("ClientSecret").Value ?? string.Empty;
}
else
{
    logger.LogInformation("Ambiente de Desenvolvimento/Local detectado.");

    connectionString = builder.Configuration.GetSection("ConnectionString").Value ?? string.Empty;

    secret = builder.Configuration.GetSection("ClientSecret").Value ?? string.Empty;

    var rabbitMQOptions = new RabbitMQOptions();
    builder.Configuration.GetSection("RabbitMQ").Bind(rabbitMQOptions);
    builder.Services.AddSingleton(rabbitMQOptions);
}

builder.Services.Configure<Secrets>(builder.Configuration);

builder.Services.AddDbContext<PedidosContext>(options =>
        options.UseNpgsql(connectionString));

builder.Services.AddAuthenticationJWT(secret);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfig();

builder.Services.AddAutoMapper(typeof(ProdutosMappingProfile));
builder.Services.AddAutoMapper(typeof(PedidosMappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddHostedService<PedidoPagoSubscriber>();
builder.Services.AddHostedService<PedidoPreparandoSubscriber>();
builder.Services.AddHostedService<PedidoPrrontoSubscriber>();

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UsePathBase(new PathString("/pedido"));
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseReDoc(c =>
{
    c.DocumentTitle = "REDOC API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await using var scope = app.Services.CreateAsyncScope();
using var dbApplication = scope.ServiceProvider.GetService<PedidosContext>();

await dbApplication!.Database.MigrateAsync();

app.Run();
