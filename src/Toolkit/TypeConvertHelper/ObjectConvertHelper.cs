using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.TypeConvertHelper
{
    public static class ObjectConvertHelper
    {
        public static T? ChangeType<T>(this object self)
        {
            return (T?)ChangeType(self, typeof(T));
        }

        public static object? ChangeType(this object self, Type type)
        {
            if (self is null)
            {
                return default;
            }
             type = Nullable.GetUnderlyingType(type) ?? type;
            return Convert.ChangeType(self, type);
        }
    }
}
