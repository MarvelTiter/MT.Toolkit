using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.DateTimeExtension
{
	/// <summary>
	/// 
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// 返回一天的零点(yyyyMMdd 000000)
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static DateTime DayStart(this DateTime self)
		{
			return new DateTime(self.Year, self.Month, self.Day, 0, 0, 0);
		}
		/// <summary>
		/// 返回一天的末点(yyyyMMdd 23:59:59)
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static DateTime DayEnd(this DateTime self)
		{
			return self.AddDays(1).DayStart().AddSeconds(-1);
		}

		/// <summary>
		/// 返回一天的零点(yyyyMMdd 000000)
		/// </summary>
		/// <param name="self"></param>
		/// <param name="format">默认 yyyy-MM-dd HH:mm:ss</param>
		/// <returns></returns>
		public static string DayStartStr(this DateTime self, string format = "yyyy-MM-dd HH:mm:ss")
		{
			return self.DayStart().ToString(format);
		}

		/// <summary>
		/// 返回一天的末点(yyyyMMdd 23:59:59)
		/// </summary>
		/// <param name="self"></param>
		/// <param name="format">默认 yyyy-MM-dd HH:mm:ss</param>
		/// <returns></returns>
		public static string DayEndStr(this DateTime self, string format = "yyyy-MM-dd HH:mm:ss")
		{
			return self.DayEnd().ToString(format);
		}
	}
}
