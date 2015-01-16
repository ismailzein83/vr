using System;
using System.Collections.Generic;

namespace TABS.BusinessEntities
{
    public class PricelistBO
    {
        public static IList<PriceListChangeLog> GetPricelistChangeLog(PriceList pricelist)
        {
            return TABS.ObjectAssembler.CurrentSession.CreateQuery("FROM PriceListChangeLog P WHERE P.PriceList=:PriceList")
                       .SetParameter("PriceList", pricelist)
                       .List<TABS.PriceListChangeLog>();
        }

        public static IList<Rate> GetRates(PriceList pricelist, DateTime effectivedate)
        {
            return TABS.DataConfiguration.CurrentSession
                   .CreateQuery(string.Format(@"FROM Rate R 
                          WHERE     
                                R.PriceList.Supplier = :Supplier 
                            AND R.PriceList.Customer = :Customer
                            AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) 
                            OR R.BeginEffectiveDate > :when)"))
                     .SetParameter("Supplier", pricelist.Supplier)
                    .SetParameter("Customer", pricelist.Customer)
                    .SetParameter("when", effectivedate)
                    .List<TABS.Rate>();
        }
    }
}
