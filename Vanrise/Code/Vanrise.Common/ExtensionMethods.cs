using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class ExtensionMethods
    {
        public static Q GetOrCreateItem<T, Q>(this Dictionary<T, Q> dictionary, T itemKey)
        {
            Q value;
            if(!dictionary.TryGetValue(itemKey, out value))
            {
                value = Activator.CreateInstance<Q>();
                dictionary.Add(itemKey, value);
            }
            return value;
        }
    }
}
