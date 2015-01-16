using System;
using System.Collections.Generic;

namespace TABS.BusinessEntities
{
    public class TariffBO
    {
        public static IList<TABS.Tariff> GetTariffs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, DateTime when)
        {
            string hql = @"FROM Tariff t
                             WHERE (t.EndEffectiveDate > :when OR t.EndEffectiveDate IS NULL)
                                    AND  (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                    AND  (t.Customer = :customer)
                                    AND  (t.Supplier = :supplier)
                             ORDER BY t.BeginEffectiveDate";
            IList<TABS.Tariff> listTariffs = TABS.DataConfiguration.CurrentSession.CreateQuery(hql)
                                                    .SetParameter("when", when)
                                                    .SetParameter("customer", customer)
                                                    .SetParameter("supplier", supplier)
                                                    .List<TABS.Tariff>();
            return listTariffs;
        }
    }
}
