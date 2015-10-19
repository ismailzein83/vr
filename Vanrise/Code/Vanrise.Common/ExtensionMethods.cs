using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            IEnumerable<Q> filteredResults = dic.Values.ApplyFiltering(filterExpression);
            IEnumerable<R> processedResults = filteredResults.Select(mapper).ApplySortingAndPaging(input);

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

            return dic.Values.Where(predicate);
        }

        public static R MapRecord<T, Q, R>(this Dictionary<T, Q> dic, Func<Q, R> mappingExpression, Func<Q, bool> filterExpression)
        {
            if (dic == null)
                return default(R);

            IEnumerable<Q> filteredResults = dic.Values.Where(filterExpression);

            if (filteredResults == null)
                return default(R);

            return filteredResults.Select(mappingExpression).FirstOrDefault();
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this Dictionary<Q, T> dic, Func<T, M> mappingExpression)
        {
            if (dic == null)
                return null;

            return dic.Values.Select(mappingExpression);
        }

        public static IEnumerable<M> MapRecords<Q, T, M>(this Dictionary<Q,T> dic, Func<T, M> mappingExpression, Func<T, bool> filterExpression)
        {
            if (dic == null)
                return null;

            IEnumerable<T> filteredResults = dic.Values.Where(filterExpression);

            if (filteredResults == null)
                return null;

            return filteredResults.Select(mappingExpression);
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
                TotalCount = filteredResults.Count(),
                Data = processedResults
            };

            return rslt;
        }

        public static Vanrise.Entities.BigResult<Q> ToBigResult<T, Q>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input,
            Func<T, bool> filterExpression, Func<T, Q> mapper)
        {
            IEnumerable<T> filteredResults = list.ApplyFiltering(filterExpression);
            IEnumerable<Q> processedResults = filteredResults.Select(mapper).ApplySortingAndPaging(input);

            Vanrise.Entities.BigResult<Q> rslt = new Vanrise.Entities.BigResult<Q>
            {
                TotalCount = filteredResults.Count(),
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

            return list.Where(predicate);
        }

        public static Q MapRecord<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression, Func<T, bool> filterExpression)
        {
            if (list == null)
                return default(Q);


            IEnumerable<T> filteredResults = list.Where(filterExpression);

            if (filteredResults == null)
                return default(Q);

            return filteredResults.Select(mappingExpression).First();


        }
        
        public static IEnumerable<Q> MapRecords<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression)
        {
            if (list == null)
                return null;

            return list.Select(mappingExpression);
        }

        public static IEnumerable<Q> MapRecords<T, Q>(this IEnumerable<T> list, Func<T, Q> mappingExpression, Func<T, bool> filterExpression)
        {
            if (list == null)
                return null;

            IEnumerable<T> filteredResults = list.Where(filterExpression);

            if (filteredResults == null)
                return null;

            return filteredResults.Select(mappingExpression);
        }

        private static IEnumerable<T> ApplyFiltering<T>(this IEnumerable<T> list, Func<T, bool> filterExpression)
        {
            if (filterExpression == null)
                return list;

            return list.Where(filterExpression);
        }

        private static IEnumerable<T> ApplySortingAndPaging<T>(this IEnumerable<T> list, Vanrise.Entities.DataRetrievalInput input)
        {
            string[] sortPropertyNameParts = input.SortByColumnName.Split('.');
            Func<T, Object> selector = x => GetPropertyValue(x, sortPropertyNameParts, 0);

            if (input.IsSortDescending)
                list = list.OrderByDescending(selector);
            else
                list = list.OrderBy(selector);

            if (input.FromRow.HasValue && input.ToRow.HasValue)
            {
                int pageSize = (input.ToRow.Value - input.FromRow.Value) + 1;
                list = list.ToList().Skip(input.FromRow.Value - 1).Take(pageSize);
            }

            return list; 
        }

        private static Object GetPropertyValue(Object target, string[] propertyNameParts, int currentPropertyIndex)
        {
            if (target == null)
                return null;

            object currentPropertyValue = target.GetType().GetProperty(propertyNameParts[currentPropertyIndex]).GetGetMethod().Invoke(target, null);
            if (currentPropertyIndex == propertyNameParts.Length - 1)
                return currentPropertyValue;
            else
                return GetPropertyValue(currentPropertyValue, propertyNameParts, (currentPropertyIndex + 1));
        }


        #endregion
    }
}
