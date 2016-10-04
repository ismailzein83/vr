using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public static class ExtensionMethods
    {
        #region Hashset Extensions

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        #endregion

        #region Dictionary Extension

        public static Q GetOrCreateItem<T, Q>(this IDictionary<T, Q> dictionary, T itemKey)
        {
            return GetOrCreateItem(dictionary, itemKey, () => Activator.CreateInstance<Q>());
        }

        public static Q GetOrCreateItem<T, Q>(this IDictionary<T, Q> dictionary, T itemKey, Func<Q> createInstance)
        {
            Q value;
            if (!dictionary.TryGetValue(itemKey, out value))
            {
                value = createInstance();
                dictionary.Add(itemKey, value);
            }
            return value;
        }

        public static Vanrise.Entities.BigResult<Q> ToBigResult<T, Q>(this Dictionary<T, Q> dic, Vanrise.Entities.DataRetrievalInput input, Func<Q, bool> filterExpression)
        {
            if (dic == null)
                return null;

            IEnumerable<Q> filteredResults = dic.Values.ApplyFiltering<Q>(filterExpression);
            IEnumerable<Q> processedResults = filteredResults.ApplySortingAndPaging(input);

            Vanrise.Entities.BigResult<Q> rslt = new Vanrise.Entities.BigResult<Q>
            {
                TotalCount = filteredResults.Count(),
                Data = processedResults
            };

            return rslt;
        }

        public static Vanrise.Entities.BigResult<R> ToBigResult<T, Q, R>(this Dictionary<T, Q> dic, Vanrise.Entities.DataRetrievalInput input,
            Func<Q, bool> filterExpression, Func<Q, R> mapper)
        {
            if (dic == null)
                return null;

            IEnumerable<Q> filteredResults = dic.Values.ApplyFiltering(filterExpression);
            IEnumerable<R> processedResults = filteredResults.Select(mapper).ApplySortingAndPaging(input).ToList();

            Vanrise.Entities.BigResult<R> rslt = new Vanrise.Entities.BigResult<R>
            {
                TotalCount = filteredResults.Count(),
                Data = processedResults
            };

            return rslt;
        }

        public static T GetRecord<Q, T>(this Dictionary<Q, T> dic, Q key)
        {
            T entity;

            if (dic != null && dic.TryGetValue(key, out entity))
                return entity;

            return default(T);
        }

        public static T FindRecord<Q, T>(this Dictionary<Q, T> dic, Func<T, bool> predicate)
        {
            if (dic == null)
                return default(T);

            return dic.Values.FirstOrDefault(predicate);
        }

        public static IEnumerable<T> FindAllRecords<Q, T>(this Dictionary<Q, T> dic, Func<T, bool> predicate)
        {
            if (dic == null)
                return null;

            return dic.Values.Where(predicate).ToList();
        }

        public static R MapRecord<T, Q, R>(this Dictionary<T, Q> dic, Func<Q, R> mappingExpression, Func<Q, bool> filterExpression)
        {
            if (dic == null)
                return default(R);

            IEnumerable<Q> filteredResults = dic.Values.ApplyFiltering(filterExpression);

            if (filteredResults == null)
                return default(R);

            return filteredResults.Select(mappingExpression).FirstOrDefault();
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this Dictionary<Q, T> dic, Func<T, M> mappingExpression)
        {
            if (dic == null)
                return null;

            return dic.Values.Select(mappingExpression).ToList();
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this Dictionary<Q, T> dic, Func<T, M> mappingExpression, Func<T, bool> filterExpression)
        {
            if (dic == null)
                return null;

            IEnumerable<T> filteredResults = dic.Values.ApplyFiltering(filterExpression);

            if (filteredResults == null)
                return null;

            return filteredResults.Select(mappingExpression).ToList();
        }

        #endregion

        #region IEnumerable Extenstions

        public static Vanrise.Entities.BigResult<T> ToBigResult<T>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input, Func<T, bool> filterExpression)
        {
            if (list == null)
                return null;

            IEnumerable<T> filteredResults = list.ApplyFiltering<T>(filterExpression);
            IEnumerable<T> processedResults = filteredResults.ApplySortingAndPaging(input);

            Vanrise.Entities.BigResult<T> rslt = new Vanrise.Entities.BigResult<T>
            {
                ResultKey = input.ResultKey,
                TotalCount = filteredResults.Count(),
                Data = processedResults
            };

            return rslt;
        }

        public static Vanrise.Entities.BigResult<Q> ToBigResult<T, Q>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input,
            Func<T, bool> filterExpression, Func<T, Q> mapper)
        {
            IEnumerable<T> filteredResults = list != null ? list.ApplyFiltering(filterExpression) : null;
            IEnumerable<Q> processedResults = filteredResults != null ? filteredResults.Select(mapper).ApplySortingAndPaging(input).ToList() : null;

            Vanrise.Entities.BigResult<Q> rslt = new Vanrise.Entities.BigResult<Q>
            {
                ResultKey = input.ResultKey,
                TotalCount = filteredResults != null ? filteredResults.Count() : 0,
                Data = processedResults
            };

            return rslt;
        }

        public static T FindRecord<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                return default(T);

            return list.FirstOrDefault(predicate);
        }

        public static IEnumerable<T> FindAllRecords<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            if (list == null)
                return null;

            return list.Where(predicate).ToList();
        }

        public static Q MapRecord<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression, Func<T, bool> filterExpression)
        {
            if (list == null)
                return default(Q);


            IEnumerable<T> filteredResults = list.ApplyFiltering(filterExpression);

            if (filteredResults == null)
                return default(Q);

            return filteredResults.Select(mappingExpression).FirstOrDefault();


        }

        public static IEnumerable<Q> MapRecords<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression)
        {
            if (list == null)
                return null;

            return list.Select(mappingExpression).ToList();
        }

        public static IEnumerable<Q> MapRecords<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression, Func<T, bool> filterExpression)
        {
            if (list == null)
                return null;

            IEnumerable<T> filteredResults = list.ApplyFiltering(filterExpression);

            if (filteredResults == null)
                return null;

            return filteredResults.Select(mappingExpression).ToList();
        }

        private static IEnumerable<T> ApplyFiltering<T>(this IEnumerable<T> list, Func<T, bool> filterExpression)
        {
            if (filterExpression == null)
                return list;

            return list.Where(filterExpression).ToList();
        }

        private static IEnumerable<T> ApplySortingAndPaging<T>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input)
        {
            list = list.VROrderList<T>(input);

            list = VRGetPage<T>(list, input);

            return list;
        }

        public static IOrderedEnumerable<T> VROrderList<T>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input)
        {
            if (list == null)
                return null;

            IPropValueReader propValueReader = Utilities.GetPropValueReader(input.SortByColumnName);
            Func<T, Object> selector = x => propValueReader.GetPropertyValue(x);

            IOrderedEnumerable<T> orderedList;
            if (input.IsSortDescending)
                orderedList = list.OrderByDescending(selector);
            else
                orderedList = list.OrderBy(selector);
            return orderedList;
        }

        public static IEnumerable<T> VRGetPage<T>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input)
        {
            IEnumerable<T> pagedList = list;
            if (input.FromRow.HasValue && input.ToRow.HasValue)
            {
                int pageSize = (input.ToRow.Value - input.FromRow.Value) + 1;
                if (list != null)
                    pagedList = list.ToList().Skip(input.FromRow.Value - 1).Take(pageSize);
            }
            return pagedList.ToList();
        }


        public static IEnumerable<Q> VRCast<Q>(this IEnumerable list)
        {
            if (list == null)
                return null;
            return list.Cast<Q>();
        }

        #endregion

        #region DateTime

        public static bool VRGreaterThan(this DateTime? date, DateTime? targetDate)
        {
            if (date.HasValue)
            {
                if (targetDate.HasValue)
                    return date.Value > targetDate.Value;
                else
                    return false;
            }
            else
            {
                if (targetDate.HasValue)
                    return true;
                else
                    return false;
            }
        }

        public static bool VRLessThan(this DateTime? date, DateTime? targetDate)
        {
            return !date.VRGreaterThan(targetDate) && (date != targetDate);
        }


        public static bool VRLessThanOrEqual(this DateTime? date, DateTime? targetDate)
        {
            return !date.VRGreaterThan(targetDate);
        }

        public static DateTime? VRMinimumDate(this IEnumerable<DateTime?> dates)
        {
            if (dates == null)
                return null;

            return dates.OrderBy(itm => itm.HasValue ? itm.Value : DateTime.MaxValue).First();
        }

        public static DateTime? VRMaximumDate(this IEnumerable<DateTime?> dates)
        {
            if (dates == null)
                return null;

            return dates.OrderBy(itm => itm.HasValue ? itm.Value : DateTime.MaxValue).Last();
        }

        #endregion

        #region Date Effective Settings

        public static bool IsEffective(this IDateEffectiveSettings entity, DateTime? date, bool futureEntities)
        {
            if (date.HasValue)
            {
                if (entity.BED <= date.Value && entity.EED.VRGreaterThan(date))
                    return true;
                else
                    return false;
            }
            else
                if (futureEntities)
                    if (!entity.EED.HasValue || entity.BED > DateTime.Now)
                        return true;

            return false;
        }

        public static bool IsEffective(this IDateEffectiveSettings entity, DateTime? date)
        {
            return IsEffective(entity, date, false);
        }

        public static bool IsEffectiveOrFuture(this IDateEffectiveSettings entity, DateTime date)
        {
            return (!entity.EED.HasValue || entity.EED.Value > date);
        }

        public static bool IsOverlappedWith(this IDateEffectiveSettings entity, IDateEffectiveSettings target)
        {
            return entity.EED.VRGreaterThan(target.BED) && target.EED.VRGreaterThan(entity.BED);
        }


        #endregion

        #region Assembly

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        #endregion
    }
}
