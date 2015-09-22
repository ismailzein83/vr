using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class RouteRulesByCriteria
    {
        public abstract bool IsEmpty();

        public abstract void SetSource(List<RouteRule> rules);

        public abstract RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId);

        protected Q GetOrCreateDictionaryItem<T, Q>(T itemKey, Dictionary<T, Q> dictionary)
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
