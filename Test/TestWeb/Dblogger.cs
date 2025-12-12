using LoggerProviderExtensions;
using LoggerProviderExtensions.DbLogger;

namespace TestWeb
{
    public class Dblogger : IDbLogger
    {
        public Task LogAsync(DbLogItem item, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Logged to DB: {item.Content}, IsException: {item.IsException}");
            return Task.CompletedTask;
        }
    }
}
