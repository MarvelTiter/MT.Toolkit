using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Mapper
{
	public class MapperConfigProvider
	{

		private static MapperConfig Config = new MapperConfig();
		public static MapperConfig GetMapperConfig()
		{
			return Config;
		}

	}
}
