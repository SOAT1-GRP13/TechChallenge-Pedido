using API.Setup;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Pedidos.AutoMapper;

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

            services.AddLogging();

            services.AddAutoMapper(typeof(PedidosMappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            DependencyInjection.RegisterServices(services);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
