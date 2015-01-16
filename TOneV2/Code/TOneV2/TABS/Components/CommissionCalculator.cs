using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;

namespace TABS.Components
{
    public class CommissionCalculator
    {

        public static void ClearFromCache()
        {
            _FutureEffectiveCommissions = null;
            _EffectiveCommissions = null;
        }

        internal static IList<Commission> _FutureEffectiveCommissions;
        public static IList<Commission> FutureEffectiveCommissions
        {
            get
            {
                if (_FutureEffectiveCommissions == null)
                {
                    var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                    var date = DateTime.Today.AddDays(noticeDays);

                    NHibernate.ICriteria criteria = DataConfiguration.CurrentSession.CreateCriteria(typeof(Commission))
                            .Add(Expression.Le("BeginEffectiveDate", date))
                            .Add(Expression.Or(
                                Expression.Gt("EndEffectiveDate", date),
                                new NullExpression("EndEffectiveDate"))
                                )
                            .AddOrder(new Order("BeginEffectiveDate", false));

                    _FutureEffectiveCommissions = criteria.List<TABS.Commission>();

                }
                return _FutureEffectiveCommissions;
            }
        }

        internal static IList<Commission> _EffectiveCommissions;
        public static IList<Commission> EffectiveCommissions
        {
            get
            {
                if (_EffectiveCommissions == null)
                {

                    NHibernate.ICriteria criteria = DataConfiguration.CurrentSession.CreateCriteria(typeof(Commission))
                            .Add(Expression.Le("BeginEffectiveDate", DateTime.Now))
                            .Add(Expression.Or(
                                Expression.Gt("EndEffectiveDate", DateTime.Now),
                                new NullExpression("EndEffectiveDate"))
                                )
                            .AddOrder(new Order("BeginEffectiveDate", false));

                    _EffectiveCommissions = criteria.List<TABS.Commission>();

                }
                return _EffectiveCommissions;
            }
        }


        public static Dictionary<string, Commission> GetCommissions(bool isExtracharge, bool isCurrent)
        {
            Dictionary<string, Commission> result = new Dictionary<string, Commission>();

            var commissions = isCurrent ? EffectiveCommissions : FutureEffectiveCommissions;
            var filteredCommissions = commissions.Where(c => c.IsExtraCharge == isExtracharge);

            foreach (var item in filteredCommissions)
            {
                var key = string.Concat(item.Supplier.CarrierAccountID, item.Customer.CarrierAccountID, item.Zone.ZoneID);
                result[key] = item;
            }

            return result;
        }
    }
}
