using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;

namespace TABS.BusinessEntities
{
    public class PrepaidPostpaidBO
    {
        public static IList<TABS.PrepaidPostpaidOptions> GetPrepaidPostpaidOptions(bool isCustomer, bool isPrepaid, bool HidePrePostInactiveCarriers)
        {
            string hql = string.Format(@"SELECT ppo 
                                     FROM PrepaidPostpaidOptions ppo 
                                     WHERE IsCustomer = :isCustomer
                                        AND IsPrepaid = :isPrepaid
                                     ORDER BY ppo.Amount,ppo.Percentage");
            IList<TABS.PrepaidPostpaidOptions> Options = TABS.ObjectAssembler.CurrentSession.CreateQuery(hql)
                .SetParameter("isCustomer", isCustomer ? "Y" : "N")
                .SetParameter("isPrepaid", isPrepaid ? "Y" : "N")
                .List<TABS.PrepaidPostpaidOptions>().Where(ppo => ((HidePrePostInactiveCarriers) ? ppo.isActiveCarrier : true)).ToList();
            return (Options != null && Options.Count > 0) ? Options.OrderBy(o => o.CarrierName).ToList() : null;
        }

        public static IList<TABS.PostpaidAmount> GetPostpaidAmounts(DateTime? fromDate, DateTime? toDate, string carrierType, bool HidePrePostInactiveCarriers)
        {
            NHibernate.ICriteria criteria = TABS.DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.PostpaidAmount));

            if (fromDate.HasValue)
            {
                criteria = criteria.Add(Expression.Ge("Date", fromDate))
                                   .AddOrder(new Order("Date", false));
            }
            if (toDate.HasValue)
                criteria = criteria.Add(Expression.Le("Date", toDate));
            if (carrierType == "Customer")
                criteria = criteria.Add(Expression.IsNotNull("Customer") || Expression.IsNotNull("CustomerProfile"));
            else criteria = criteria.Add(Expression.IsNotNull("Supplier") || Expression.IsNotNull("SupplierProfile"));
            return criteria.List<TABS.PostpaidAmount>().Where(pa => ((HidePrePostInactiveCarriers) ? pa.isActiveCarrier : true)).ToList();
        }
    }
}
