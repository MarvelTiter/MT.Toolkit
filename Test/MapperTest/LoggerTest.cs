using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MT.Toolkit.LogTool;
using MT.Toolkit.LogTool.DbLogger;

namespace MapperTest
{
    public class DbLogger : IDbLogger
    {
        public async Task LogAsync(LogInfo data, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            Console.WriteLine(data.FormatLogMessage());
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
                builder.AddDbLogger(s =>
                {
                    s.SetDbWriteLevel(LogLevel.Debug);
                });

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