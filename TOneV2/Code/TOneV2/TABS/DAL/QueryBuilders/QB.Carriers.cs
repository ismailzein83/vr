using System;
using NHibernate;
using NHibernate.Criterion;

namespace TABS.DAL
{
    public partial class QueryBuilder
    {
        public static IQuery GetCarrierAccountsBySuffixQuery(CarrierAccount account)
        {
            var query = TABS.ObjectAssembler.CurrentSession.CreateQuery("From CarrierAccount ca Where ca._CarrierAccountID != :id and ca.CarrierProfile = :profile and ca.NameSuffix = :suffix")
                   .SetParameter("id", account.CarrierAccountID)
                   .SetParameter("suffix", account.NameSuffix)
                   .SetEntity("profile", account.CarrierProfile);
            return query;
        }

        public static string GetCarrierAccountIDsQuery()
        {
            return "SELECT CarrierAccountID FROM CarrierAccount";
        }

        public static IQuery GetZonesQuery(CarrierAccount supplier, DateTime when)
        {
            return TABS.DataConfiguration.CurrentSession
                     .CreateQuery(@"FROM Zone Z
                                             WHERE Z.Supplier = :supplier
                                                AND ((Z.BeginEffectiveDate <= :when AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > :when)))
                                             ORDER BY Z.Name")
                    .SetParameter("when", when)
                    .SetParameter("supplier", supplier);
        }

        public static ICriteria GetCommissionsQuery(CarrierAccount customer, CarrierAccount supplier, Zone zone, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria = TABS.DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.Commission));
            criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            if (zone != null) criteria = criteria.Add(Expression.Eq("Zone", zone));
            if (effectiveDate != null) criteria = criteria.Add(Expression.Le("BeginEffectiveDate", effectiveDate.Value))
                                                                                               .Add(Expression.Or(
                                                                                                      Expression.Gt("EndEffectiveDate", effectiveDate.Value),
                                                                                                      new NullExpression("EndEffectiveDate")))
                                                                                              .AddOrder(Order.Desc("BeginEffectiveDate"));
            return criteria;
        }

        public static ICriteria GetToDsQuery(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria = TABS.DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.ToDConsideration));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            if (zone != null) criteria = criteria.Add(Expression.Eq("Zone", zone));
            if (effectiveDate != null) criteria = criteria.Add(Expression.Le("BeginEffectiveDate", effectiveDate))
                                                                                               .Add(Expression.Or(
                                                Expression.Gt("EndEffectiveDate", effectiveDate),
                                                new NullExpression("EndEffectiveDate")))
                        .AddOrder(Order.Desc("BeginEffectiveDate"));
            return criteria;
        }
    }
}
