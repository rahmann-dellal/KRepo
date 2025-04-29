using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Converters
{
    public class KioberConverter
    {
        public static bool ToBool(object value)
        {
            if (value == null)
                return false;

            if (value is bool b)
                return b;

            if (value is string s)
                return !string.IsNullOrWhiteSpace(s);

            if (value is int i)
                return i != 0;

            if (value is long l)
                return l != 0;

            if (value is double d)
                return d != 0;

            if (value is decimal m)
                return m != 0;

            if (value is float f)
                return f != 0;

            if (value is IEnumerable<object> collection)
                return collection.Any();

            return true; // fallback: non-null, non-empty, non-zero = truthy
        }
    }
}
