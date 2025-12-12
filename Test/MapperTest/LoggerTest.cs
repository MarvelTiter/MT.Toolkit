using LoggerProviderExtensions;
using LoggerProviderExtensions.DbLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MapperTest
{
    public class DbLogger : IDbLogger
    {
        public Task LogAsync(DbLogItem item, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public async Task ConfiguraTest()
        {
            var services = new ServiceCollection();
            services.AddScoped<IDbLogger, DbLogger>();
            services.AddLogging(builder =>
            {
                builder.AddDbLogger();

            });
            var provider = services.BuildServiceProvider();
            var logger = provider.GetService<ILogger<LoggerTest>>()!;
            int row = 0;
            while (row < 100)
            {
                logger.LogInformation("{row},Hello", row);
                row++;
            }
            await Task.Delay(2000);
        }
    }
}