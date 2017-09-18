using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oogi2.Tokens;
using Sushi2;

namespace Oogi2
{
    internal static class Converter
    {        
        private static readonly Dictionary<Type, Func<object, string>> _processors = new Dictionary<Type, Func<object, string>>
                                                                     {
                                                                         { typeof(string), StringProcessor },
                                                                         { typeof(char), StringProcessor },
                                                                         { typeof(bool), BooleanProcessor },
                                                                         { typeof(bool?), BooleanProcessor },
                                                                         { typeof(Stamp), StampProcessor },
                                                                         { typeof(SimpleStamp), StampProcessor }
                                                                     };

        private static string StampProcessor(object arg)
        {
            var stamp = arg as IStamp;

            return stamp == null ? "null" : Process(stamp.Epoch);
        }

        private static string ListProcessor(object items)
        {
            var list = items as IEnumerable;
            
            if (list == null)
                return "(null)";

            var enumerable = list as IList<object> ?? list.Cast<object>().ToList();            
            
            var result = new StringBuilder("(");
            
            var counter = 0;
            var total = enumerable.Count;

            if (total == 0)
                return "(null)";

            foreach (var item in enumerable)
            {
                counter++;
                var v = Process(item);
                
                result.Append(v);

                if (counter < total)
                    result.Append(",");
            }

            result.Append(")");

            return result.ToString();
        }

        public static string Process(object val)
        {
            if (val == null)
                return "null";
            
            var t = val.GetType();

            if (t.IsEnum)
                return EnumProcessor(val);

            if (_processors.ContainsKey(t))
                return _processors[t].Invoke(val);

            var isEnumerableOfT = t.GetInterfaces()
                .Any(ti => ti.IsGenericType
                     && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (isEnumerableOfT)
                return ListProcessor(val);

            return UniversalProcessor(val);
        }

        private static string UniversalProcessor(object val)
        {
            var formattable = val as IFormattable;

            return formattable?.ToString(null, Cultures.English) ?? val.ToString();
        }
        
        private static string StringProcessor(object val)
        {            
            return "'" + val.ToString().ToEscapedString() + "'";
        }

        private static string BooleanProcessor(object val)
        {
            return val.ToString().ToLower();
        }

        private static string EnumProcessor(object val)
        {
            var underlyingType = Enum.GetUnderlyingType(val.GetType());
            var value = Convert.ChangeType(val, underlyingType);

            return Process(value);
        }
    }
}
