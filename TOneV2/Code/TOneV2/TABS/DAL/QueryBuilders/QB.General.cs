using System;
using NHibernate.Criterion;

namespace TABS.DAL
{
    public partial class QueryBuilder
    {
        public static string GetRoutOverridesQuery()
        {
            return @"SELECT * FROM [RouteOverride] WITH(NOLOCK)";
        }

        public static string UpdateZoneCodeGroupQuery(int zoneid, string code)
        {
            return string.Format(@"UPDATE Zone
                                   SET CodeGroup = '{1}'
                                   FROM zone WITH(NOLOCK)
                                   WHERE zoneID ={0}", zoneid, code);
        }

        public static string GetZonesAndCodesQuery(DateTime effectiveDate)
        {
            string sql = string.Format(@"
                                        DECLARE @date DATETIME 
                                        SET @date = '{0:yyyy-MM-dd HH:mm:ss}';
            
                                        WITH Zones AS 
                                        (
                                            SELECT z.zoneID
                                            FROM   Zone z WITH(NOLOCK)
                                            WHERE  (ISNULL(Z.EndEffectiveDate, '2050-01-01') > @date)
            
                                        ),
                                          Codes AS 
                                        (
                                            SELECT c.ZoneID,
                                                   MIN(c.Code) AS Code
                                            FROM   Code c WITH(NOLOCK)
                                            WHERE  (ISNULL(c.EndEffectiveDate, '2050-01-01') > @date)
                                            GROUP BY c.ZoneID
                                        )
                                        SELECT Z.ZoneID,
                                               c.Code
                                        FROM   zones Z
                                               LEFT JOIN Codes C
                                                    ON  z.ZoneID = c.ZoneID
                                        ORDER BY
                                               Z.zoneId,
                                               c.Code
                                                  ", DateTime.Now.Date);
            return sql;
        }

        public static string InsertSwitchCarrierMappingsQuery()
        {
            return "INSERT INTO SwitchCarrierMapping(SwitchID, CarrierAccountID, Identifier, IsIn, IsOut) VALUES (@P1, @P2, @P3, @P4, @P5)";
        }

        public static string FindIdInTrafficStatsQuery()
        {
            return "SELECT ID FROM TrafficStats WITH(NOLOCK) WHERE ID = @P1";
        }

        public static string DropAndCreateSwitchCarrierMappingQuery()
        {
            return "DROP TABLE SwitchCarrierMapping; CREATE TABLE SwitchCarrierMapping(SwitchID TINYINT, CarrierAccountID VARCHAR(10), Identifier varchar(100), IsIn char(1), IsOut char(1))";
        }

        public static string GetSwitchCarrierMappingsQuery()
        {
            return "SELECT * FROM SwitchCarrierMapping ORDER BY CarrierAccountID, IsIn DESC, IsOut DESC";
        }

        public static NHibernate.IQuery GetCurrencyExchangeQuery(TABS.Currency currency, DateTime rateUpdateDate)
        {
            return TABS.ObjectAssembler.CurrentSession
                        .CreateQuery("FROM TABS.CurrencyExchangeRate x WHERE x.ExchangeDate = :date and x.Currency = :currency")
                        .SetEntity("currency", currency)
                        .SetDateTime("date", rateUpdateDate);
        }

        public static NHibernate.IQuery GetCurrencyExchangeHistoryQuery(TABS.Currency currency)
        {
            return TABS.ObjectAssembler.CurrentSession
                .CreateQuery("SELECT R FROM CurrencyExchangeRate R WHERE R.Currency = :currency ORDER BY ExchangeDate DESC")
                .SetParameter("currency", currency);
        }

        public static NHibernate.ICriteria GetCommissionsQuery(CarrierAccount customer, CarrierAccount supplier, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria =DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.Commission));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            if (effectiveDate != null)
            {
                criteria = criteria.Add(Expression.Le("BeginEffectiveDate", effectiveDate))
                                                .Add(Expression.Or(
                                                Expression.Ge("EndEffectiveDate", effectiveDate),
                                                new NullExpression("EndEffectiveDate")));
            }
            return criteria;
        }

        public static string DeleteSwitchReleaseCodesQuery()
        {
            return "DELETE FROM SwitchReleaseCode WHERE SwitchID=@P1";
        }

        public static NHibernate.ICriteria GetTariffsQuery(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime? effectiveDate, TABS.Zone zone)
        {
            NHibernate.ICriteria criteria = DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.Tariff));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            if (zone != null) criteria = criteria.Add(Expression.Eq("Zone", zone));
            if (effectiveDate != null)
            {
                criteria = criteria.Add(Expression.Le("BeginEffectiveDate", effectiveDate))
                                                .Add(Expression.Or(
                                                Expression.Gt("EndEffectiveDate", effectiveDate),
                                               new NullExpression("EndEffectiveDate")));
            }
            return criteria;
        }

        public static NHibernate.ICriteria GetTODsQuery(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria = DataConfiguration.CurrentSession.CreateCriteria(typeof(TABS.ToDConsideration));
            if (customer != null) criteria = criteria.Add(Expression.Eq("Customer", customer));
            if (supplier != null) criteria = criteria.Add(Expression.Eq("Supplier", supplier));
            if (effectiveDate != null)
            {
                criteria = criteria.Add(Expression.Le("BeginEffectiveDate", effectiveDate))
                                                .Add(Expression.Or(
                                                Expression.Gt("EndEffectiveDate", effectiveDate),
                                                new NullExpression("EndEffectiveDate")));
            }
            return criteria;
        }

        public static NHibernate.IQuery GetZoneCodesQuery(int zoneid)
        {
            NHibernate.IQuery query = DataConfiguration.CurrentSession.CreateQuery("SELECT C From Code C WHERE 1=1 AND C.Zone._ZoneID = :zoneid")
                .SetParameter("zoneid", zoneid);
            return query;
        }

        public static NHibernate.IQuery GetRatesQuery(int? zoneid, TABS.CarrierAccount supplier, TABS.CarrierAccount customer)
        {
            string HQL = "SELECT R From Rate R WHERE 1=1";
            if (zoneid != null)
                HQL += " AND R.Zone._ZoneID = :zoneid";

            if (supplier != null)
                HQL += " AND R.PriceList.Supplier = :supplier";
            else if (customer != null)
                HQL += " AND R.PriceList.Customer = :customer";

            HQL += " ORDER BY R.Zone.Name, R.BeginEffectiveDate DESC";

            NHibernate.IQuery Query = TABS.DataConfiguration.CurrentSession.CreateQuery(HQL);

            if (customer != null) Query.SetParameter("customer", customer);
            if (supplier != null) Query.SetParameter("supplier", supplier);
            if (zoneid != null)
                Query.SetParameter("zoneid", zoneid);
            return Query;
        }

        public static NHibernate.IQuery GetOwnZonesForCustomerQuery(TABS.CarrierAccount supplier, TABS.CarrierAccount customer)
        {
            return TABS.DataConfiguration.CurrentSession.CreateQuery(
                    @"SELECT Z FROM TABS.Zone Z 
                            WHERE 
                                    Z.Supplier = :Supplier
                                AND EXISTS (SELECT R FROM Rate R WHERE R.Zone = Z AND R.PriceList.Customer = :Customer)                                
                            ORDER BY Z.Name, Z.BeginEffectiveDate DESC")
                    .SetParameter("Supplier", supplier)
                    .SetParameter("Customer", customer);
        }

        public static NHibernate.IQuery GetAllSupplierZonesQuery(TABS.CarrierAccount supplier)
        {
            return TABS.DataConfiguration.CurrentSession.CreateQuery(
                        @"SELECT Z FROM Zone Z
                            WHERE                                     
                                Z.Supplier = :supplier                                
                                ORDER BY Z.Name, Z.BeginEffectiveDate DESC")
                   .SetParameter("supplier", supplier);
        }

        public static NHibernate.IQuery GetPricelistChangeLogQuery(TABS.PriceList pricelist)
        {
            return TABS.ObjectAssembler.CurrentSession.CreateQuery("FROM PriceListChangeLog P WHERE P.PriceList=:PriceList")
                      .SetParameter("PriceList", pricelist);
        }

        public static NHibernate.IQuery GetRatesQuery(PriceList pricelist, DateTime effectivedate)
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
                   .SetParameter("when", effectivedate);
        }

        public static NHibernate.IQuery GetSupplierCodesQuery(CarrierAccount supplier, DateTime effectivedate)
        {
            return TABS.DataConfiguration.CurrentSession
                   .CreateQuery(string.Format(@"FROM Code C 
                            WHERE 
                            (C.Zone.Supplier = :supplier)  
                            AND ((C.BeginEffectiveDate <= :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= :when)) OR C.BeginEffectiveDate >= :when)"))
                    .SetParameter("when", effectivedate)
                    .SetParameter("supplier", supplier);
        }

        public static NHibernate.IQuery GetPricingTemplatePlanQuery(int pricingtemplateid)
        {
            return TABS.ObjectAssembler.CurrentSession.CreateQuery(
                                                            @"FROM PricingTemplatePlan P 
                                                            WHERE P.PricingTemplate.PricingTemplateId = :id 
                                                            ORDER BY P.Priority DESC")
                                                            .SetParameter("id", pricingtemplateid);
        }

    }
}
