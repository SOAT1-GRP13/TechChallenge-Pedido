using API.Setup;
using System.Text;
using Infra.Pedidos;
using Infra.RabbitMQ;
using Domain.RabbitMQ;
using System.Reflection;
using Infra.RabbitMQ.Consumers;
using Microsoft.EntityFrameworkCore;
using Application.Pedidos.AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace API.Tests
{
    public class TestStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var jsonString = @"{""ConnectionString"": ""teste""}";

            var configuration = new ConfigurationBuilder()
                    .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                    .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<PedidosContext>(options =>
                options.UseNpgsql("User ID=fiap;Password=S3nh@L0c@lP3d1d0;Host=localhost;Port=15433;Database=techChallengeProduto;Pooling=true;"));

            var rabbitMQServiceMock = new Mock<IRabbitMQService>();
            services.AddSingleton(rabbitMQServiceMock.Object);
            services.AddHostedService<PedidoPagoSubscriber>();
            services.AddHostedService<PedidoPreparandoSubscriber>();
            services.AddHostedService<PedidoProntoSubscriber>();

            services.AddLogging();

            services.AddAutoMapper(typeof(PedidosMappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            DependencyInjection.RegisterServices(services);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
