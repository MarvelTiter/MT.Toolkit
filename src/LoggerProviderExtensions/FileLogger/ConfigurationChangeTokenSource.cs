using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.FileLogger;

internal class ConfigurationChangeTokenSource<TOptions> : IOptionsChangeTokenSource<TOptions>
{
    private readonly IConfiguration config;

    public ConfigurationChangeTokenSource(IConfiguration config) : this(Options.DefaultName, config)
    {
    }
    public ConfigurationChangeTokenSource(string? name, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        Name = name ?? Options.DefaultName;
        this.config = config;
    }
    public string? Name { get; }

    public IChangeToken GetChangeToken()
    {
        return config.GetReloadToken();
    }
}
