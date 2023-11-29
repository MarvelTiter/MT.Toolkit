using Microsoft.Extensions.DependencyInjection;

namespace WebServiceTest
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var client = new HttpClient();
            var ws = new SoapService(client, "", "");
            var response = await ws.SendAsync("Login", new()
            {
                ["szUser"] = "admin",
                ["szPass"] = "admin123"
            });
        }

        [Fact]
        public async void ServiceCollectionTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSoapServiceHelper(manager =>
            {
                manager.AddSoapService("Main", config =>
                {
                    config.Url = "";
                    config.RequestNamespace = "";
                    config.ResponseNamespace = "";
                }).AddSoapService("Sec", config =>
                {
                    config.Url = "Sec";
                })
                .SetDefault("Sec");
            });

            var provider = services.BuildServiceProvider();
            var fac = provider.GetService<ISoapServiceFactory>();
            var main = fac!.GetSoapService("Main");
            var response = await main.SendAsync("Login", new()
            {
                ["szUser"] = "admin",
                ["szPass"] = "admin123"
            });
        }
    }
}