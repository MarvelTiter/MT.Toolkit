#if !NET40_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Mapper.Extensions
{
    [Obsolete("使用AutoGenMapperGenerator代替")]
	public static class MapperServiceExtension
    {
		public static IServiceCollection AddSimpleMapper(this IServiceCollection services)
		{
			var mapper = new SimpleMapper();
			services.AddSingleton<ISimpleMapper>(mapper);
			return services;
		}

		public static IServiceCollection AddSimpleMapper(this IServiceCollection services, Action<MapperConfig> config)
		{
			var mapper = new SimpleMapper();
			config.Invoke(mapper.Configuration);
			services.AddSingleton<ISimpleMapper>(mapper);
			return services;
		}
	}
}
#endif