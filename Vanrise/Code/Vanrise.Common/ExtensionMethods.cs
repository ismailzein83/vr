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
            return GetOrCreateItem(dictionary, itemKey, () => Activator.CreateInstance<Q>());
        }

        public static Q GetOrCreateItem<T, Q>(this Dictionary<T, Q> dictionary, T itemKey, Func<Q> createInstance)
        {
            Q value;
            if (!dictionary.TryGetValue(itemKey, out value))
            {
                value = createInstance();
                dictionary.Add(itemKey, value);
            }
            return value;
        }

        public static Vanrise.Entities.BigResult<T> ToBigResult<T>(this IEnumerable<T> allRecords, Vanrise.Entities.DataRetrievalInput input, Func<IEnumerable<T>, IEnumerable<T>> filterResult)
        {
            if (allRecords == null)
                return null;

            allRecords = filterResult(allRecords);

            Vanrise.Entities.BigResult<T> rslt = new Vanrise.Entities.BigResult<T>
            {
                TotalCount = allRecords.Count()
            };

            if (input.FromRow.HasValue && input.ToRow.HasValue)
            {
                int pageSize = (input.ToRow.Value - input.FromRow.Value) + 1;
                rslt.Data = allRecords.ToList().Skip(input.FromRow.Value - 1).Take(pageSize);
            }
            else
            {
                rslt.Data = allRecords;
            }

            Func<T, Object> selector = x => typeof(T).InvokeMember(input.SortByColumnName, System.Reflection.BindingFlags.GetProperty, null, x, null);

            if (input.IsSortDescending)
                rslt.Data = rslt.Data.OrderByDescending(selector);
            else
                rslt.Data = rslt.Data.OrderBy(selector);

            return rslt;
        }

    }
}
