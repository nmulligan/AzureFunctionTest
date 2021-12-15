using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
//using Isolated.Business.Services;

namespace Function.MSABackEnd
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddEnvironmentVariables();
                    var config = configurationBuilder.Build();
                })
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureOpenApi()
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    //actual code should reside in a separate project
                    //services.AddSingleton<ITimeService, TimeService>();
                })
                .Build();

            host.Run();
        }
    }
}