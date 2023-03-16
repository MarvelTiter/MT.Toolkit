using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.EnumExtensions
{
	public static class EnumHelper
	{
		public static string GetDisplayName<T>(this T @enum) where T : Enum
		{
			var name = Enum.GetName(typeof(T), @enum);
			var member = typeof(T).GetMember(name)[0];
			var attr = Attribute.GetCustomAttribute(member, typeof(DisplayAttribute));
			if (attr is DisplayAttribute display)
			{
				return display.Name;
			}
			return member.Name;
		}

		private static ConcurrentDictionary<string, Type> enumCache = new ConcurrentDictionary<string, Type>();

		private static ConstructorInfo DisplayAttributeCtor = typeof(DisplayAttribute).GetConstructor(Type.EmptyTypes);
		private static PropertyInfo NameProperty = typeof(DisplayAttribute).GetProperty("Name");
		[Obsolete]
		public static Type GenerateEnumType<T>(this IEnumerable<T> sources, Func<T, object> display, Func<T, object> value)
		{
			var name = typeof(T).FullName ?? typeof(T).Name;
			AssemblyName asmName = new AssemblyName("DynamicEnumAssembly");
			AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(
				asmName, AssemblyBuilderAccess.RunAndCollect);

			ModuleBuilder mb = ab.DefineDynamicModule(asmName.Name + ".dll");

			// Define a public enumeration with the name "Elevation" and an
			// underlying type of Integer.
			EnumBuilder eb = mb.DefineEnum(name + "DynamicEnum", TypeAttributes.Public, typeof(int));

			var index = 0;
			foreach (var item in sources)
			{
				var displayName = display.Invoke(item).ToString();
				var enumValue = value.Invoke(item).ToString();
				var field = eb.DefineLiteral(value.Invoke(item).ToString(), index);
				var attr = new CustomAttributeBuilder(DisplayAttributeCtor, null, new[] { NameProperty }, new object[] { displayName });
				field.SetCustomAttribute(attr);
				index++;
			}

			// Create the type and save the assembly.
			Type finished = eb.CreateTypeInfo();

			return finished;
		}
	}
}
