using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class RouteRulesByCriteria<T> where T : IRouteCriteria
    {
        public abstract bool IsEmpty();

        public abstract void SetSource(List<T> rules);

        public abstract T GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId);

        public RouteRulesByCriteria<T> NextRuleSet { get; set; }

        protected R GetOrCreateDictionaryItem<Q, R>(Q itemKey, Dictionary<Q, R> dictionary)
        {
            R value;
            if(!dictionary.TryGetValue(itemKey, out value))
            {
                value = Activator.CreateInstance<R>();
                dictionary.Add(itemKey, value);
            }
            return value;
        }
    }    
}
