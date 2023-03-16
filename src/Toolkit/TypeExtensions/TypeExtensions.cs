using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.TypeExtensions
{
	public static class TypeExtensions
	{
		private static readonly string System_Collections_Generic_Dictionary = "System.Collections.Generic.Dictionary";
		private static readonly string System_Collections_Generic_IDictionary = "System.Collections.Generic.IDictionary";
		private static readonly string System_Collections_Generic_ICollection_1 = "System.Collections.Generic.ICollection`1";
		private static readonly string System_Collections_Generic_IEnumerable_1 = "System.Collections.Generic.IEnumerable`1";

		public static bool IsDictionary(this Type type)
		{
			var interfaces = type.GetInterfaces();
			return type.FullName.StartsWith(System_Collections_Generic_Dictionary) ||
				type.FullName.StartsWith(System_Collections_Generic_IDictionary) ||
				type.GetInterfaces().Any(tp => tp.FullName.StartsWith(System_Collections_Generic_IDictionary));
		}
		public static bool IsIEnumerableType(this Type type)
		{
			return type.FullName.StartsWith(System_Collections_Generic_IEnumerable_1) ||
				type.GetInterfaces().Any(tp => tp.FullName.StartsWith(System_Collections_Generic_IEnumerable_1));
		}

		public static bool IsICollectionType(this Type type)
		{
			return type.FullName.StartsWith(System_Collections_Generic_ICollection_1) ||
				type.GetInterfaces().Any(tp => tp.FullName.StartsWith(System_Collections_Generic_ICollection_1));
		}

		public static bool IsNullableType(this Type type)
		{
			return type.FullName.StartsWith("System.Nullable`1[");
		}

		public static Type GetCollectionElementType(this Type type)
		{
			if (type.IsArray) { type.GetElementType(); }
			if (type.IsGenericEnumerableType()) { return type.GetGenericArguments()[0]; }
			var arrayType = Array.Find(type.GetInterfaces(), IsGenericEnumerableType);
			if (arrayType == null) { return typeof(object); }
			return arrayType.GetGenericArguments()[0];
		}

		private static bool IsGenericEnumerableType(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}
	}
}
