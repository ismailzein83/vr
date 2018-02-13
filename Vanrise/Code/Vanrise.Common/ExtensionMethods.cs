using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        public static Vanrise.Entities.BigResult<Q> ToBigResult<T, Q>(this IDictionary<T, Q> dic, Vanrise.Entities.DataRetrievalInput input, Func<Q, bool> filterExpression)
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

        public static Vanrise.Entities.BigResult<R> ToBigResult<T, Q, R>(this IDictionary<T, Q> dic, Vanrise.Entities.DataRetrievalInput input,
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

        public static T GetRecord<Q, T>(this IDictionary<Q, T> dic, Q key)
        {
            T entity;

            if (dic != null && dic.TryGetValue(key, out entity))
                return entity;

            return default(T);
        }

        public static T FindRecord<Q, T>(this IDictionary<Q, T> dic, Func<T, bool> predicate)
        {
            if (dic == null)
                return default(T);

            return dic.Values.FirstOrDefault(predicate);
        }

        public static IEnumerable<T> FindAllRecords<Q, T>(this IDictionary<Q, T> dic, Func<T, bool> predicate)
        {
            if (dic == null)
                return null;

            return dic.Values.Where(predicate).ToList();
        }

        public static R MapRecord<T, Q, R>(this IDictionary<T, Q> dic, Func<Q, R> mappingExpression, Func<Q, bool> filterExpression)
        {
            if (dic == null)
                return default(R);

            IEnumerable<Q> filteredResults = dic.Values.ApplyFiltering(filterExpression);

            if (filteredResults == null)
                return default(R);

            return filteredResults.Select(mappingExpression).FirstOrDefault();
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this IDictionary<Q, T> dic, Func<T, M> mappingExpression)
        {
            if (dic == null)
                return null;

            return dic.Values.Select(mappingExpression).ToList();
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this IDictionary<Q, T> dic, Func<T, M> mappingExpression, Func<T, bool> filterExpression)
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
        public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
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

        public static DateTime? MaxDate(this DateTime? date1, DateTime? date2)
        {
            if (date1 == null)
                return date1;

            if (date2 == null)
                return date2;

            return new DateTime(Math.Max(date1.Value.Ticks, date2.Value.Ticks));
        }

        public static DateTime? MinDate(this DateTime? date1, DateTime? date2)
        {
            if (date1 == null)
                return date2;

            if (date2 == null)
                return date1;

            return new DateTime(Math.Min(date1.Value.Ticks, date2.Value.Ticks));
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
            return Utilities.AreTimePeriodsOverlapped(entity.BED, entity.EED, target.BED, target.EED);
        }

        public static bool IsInTimeRange(this IDateEffectiveSettings entity, DateTime date)
        {
            return entity.BED <= date && entity.EED.VRGreaterThan(date);
        }

        public static bool IsEffective(this IDateEffectiveSettings entity, EntityFilterEffectiveMode effectiveMode, DateTime date)
        {
            switch (effectiveMode)
            {
                case EntityFilterEffectiveMode.Current:
                    return IsEffective(entity, date);
                case EntityFilterEffectiveMode.CurrentAndFuture:
                    return IsEffectiveOrFuture(entity, date);
                case EntityFilterEffectiveMode.All:
                    return true;
                default:
                    throw new ArgumentException(string.Format("EntityFilterEffectiveMode '{0}' is not valid", effectiveMode));
            }
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

        #region Miscellaneous

        public delegate bool TryParseHandler<T>(string value, out T result);
        public static T TryParseWithValidate<T>(this string valueToBeConverted, TryParseHandler<T> handler)
        {
            return TryParseWithValidate(valueToBeConverted, handler, null);
        }

        public static T TryParseWithValidate<T>(this string valueToBeConverted, TryParseHandler<T> handler, string objectName)
        {
            if (String.IsNullOrEmpty(valueToBeConverted))
                throw new ArgumentNullException("valueToBeConverted");

            T result;
            if (handler(valueToBeConverted, out result))
                return result;

            throw new Exception(String.Format("Unable to convert value '{0}' to type '{1}'. objectName '{2}'", valueToBeConverted, typeof(T), objectName));
        }

        /// <summary>
        /// Check if object with reference type is null and throw a null reference excpetion with formated string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objectName">Defined Object Name</param>
        /// <param name="id">Object Id</param>
        public static void ThrowIfNull<T>(this T obj, string objectName, object id = null) where T : class
        {
            if (obj == null)
                throw new NullReferenceException(string.Format("{0} '{1}'", objectName, id));
        }

        public static T CastWithValidate<T>(this Object obj, string objectName, object id = null) where T : class
        {
            obj.ThrowIfNull(objectName, id);
            T castedObject = obj as T;
            if (castedObject == null)
                throw new Exception(String.Format("{0} '{1}' is not of type {2}. it is of type {3}", objectName, id, typeof(T), obj.GetType()));
            return castedObject;
        }

        public static T CastWithValidateStruct<T>(this Object obj, string objectName) where T : struct
        {
            if (!(obj is T))
                throw new Exception(String.Format("{0} '{1}' is not of type {2}. it is of type {3}", objectName, obj, typeof(T), obj.GetType()));
            T castedObject = (T)obj;
            return castedObject;
        }

        public static void SkipBytes(this Stream stream, int numberOfBytes)
        {
            stream.Seek(numberOfBytes, SeekOrigin.Current);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        public static string ConvertByteToString(this byte[] source)
        {
            return source != null ? System.Text.Encoding.UTF8.GetString(source) : null;
        }
        #endregion

        #region Object Extensions

        public static T VRDeepCopy<T>(this T obj)
        {
            if (obj == null)
                return obj;
            return Serializer.Deserialize<T>(Serializer.Serialize(obj));
        }

        #endregion
        
    }
}