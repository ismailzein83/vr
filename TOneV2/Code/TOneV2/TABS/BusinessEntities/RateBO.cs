using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
namespace TABS.BusinessEntities
{
    public class RateBO
    {
        public static NHibernate.IQuery GetCurrentAndFutureOwnRates()
        {
            string currentFutureCondition = "r.BeginEffectiveDate <= :when and (r.EndEffectiveDate is null or r.EndEffectiveDate >= :when)";
            var data = TABS.ObjectAssembler.CurrentSession
                    .CreateQuery("from Rate r where r.PriceList.Supplier = :sys and " + currentFutureCondition)
                    .SetParameter("sys", TABS.CarrierAccount.SYSTEM)
                    .SetParameter("when", DateTime.Now);
            return data;
        }

        public static IList<TABS.Rate> GetRates(int? zoneid, TABS.CarrierAccount supplier, TABS.CarrierAccount customer)
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
            return Query
                .List<TABS.Rate>();
        }

        public static object GetPendingCodeRateList(CarrierAccount customer, CarrierAccount supplier)
        {
            

            return TABS.ObjectAssembler.CurrentSession.CreateQuery(
                @"FROM Code c, Rate r 
                    WHERE 
                            r.PriceList.Supplier = :supplier 
                        AND r.PriceList.Customer = :customer
                        AND r.Zone = c.Zone
                        AND r.BeginEffectiveDate > :when
                        AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > :when)
                        AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > r.BeginEffectiveDate)
                    ORDER BY r.Zone.Name, c.Value
                    ")
                     .SetParameter("supplier", supplier)
                     .SetParameter("customer", customer)
                     .SetParameter("when", DateTime.Today)
                     .List();
        }

        public static object GetEffectiveCodeRateList(CarrierAccount customer, CarrierAccount supplier, DateTime effectivedate)
        {
            return TABS.ObjectAssembler.CurrentSession.CreateQuery(
                    @"FROM Code c, Rate r 
                        WHERE 
                                r.PriceList.Supplier = :supplier 
                            AND r.PriceList.Customer = :customer
                            AND r.Zone = c.Zone
                            AND c.BeginEffectiveDate <= :when
                            AND r.BeginEffectiveDate <= :when
                            AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > :when)
                            AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > :when)
                        ORDER BY r.Zone.Name, c.Value
                        ")
                         .SetParameter("supplier", supplier)
                         .SetParameter("customer", customer)
                         .SetParameter("when", effectivedate)
                         .List();
        }
        #region ADO Funtionality
        
        public static System.Data.IDbConnection GetOpenedConnection()
        {
            System.Data.IDbConnection connection = new System.Data.SqlClient.SqlConnection(TABS.DataConfiguration.Default.Properties["connection.connection_string"].ToString());
            connection.Open();
            return connection;
        }
        public static System.Data.IDataReader ExecuteReader(System.Data.IDbConnection connection,string sql)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            System.Data.IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }
       
        public static System.Data.IDataReader GetEffectiveCodeRateListADO(CarrierAccount customer, CarrierAccount supplier, DateTime effectivedate)
        {
            string sql = string.Format(@"select z.ZoneID,z.CodeGroup,z.Name,c.Code,r.RateID,r.ServicesFlag,r.Rate,p.CurrencyID,r.OffPeakRate,r.WeekendRate,r.Change,
                    r.BeginEffectiveDate,r.EndEffectiveDate ,c.BeginEffectiveDate,c.EndEffectiveDate,p.PriceListID,cu.LastRate,cu.IsMainCurrency,p.BeginEffectiveDate
                    from  
                     code c inner join rate r on c.ZoneID=r.ZoneID
                     inner join Zone z on z.ZoneID=r.ZoneID
                     inner join PriceList p on p.PriceListID=r.PriceListID
                     inner join Currency cu on cu.CurrencyID=p.CurrencyID
                     where 
                     p.CustomerID='{0}' and p.SupplierID='{1}' AND
                     c.BeginEffectiveDate <= '{2}'
                     AND r.BeginEffectiveDate <= '{2}'
                     AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >'{2}')
                     AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > '{2}')
                    ORDER BY z.Name, c.Code", customer.CarrierAccountID,supplier.CarrierAccountID,effectivedate.ToString("yyyy-MM-dd"));
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)GetOpenedConnection();
            System.Data.IDataReader reader = ExecuteReader(connection, sql);
            return reader;
        }
        public static System.Data.IDataReader GetPendingCodeRateListADO(CarrierAccount customer, CarrierAccount supplier)
        {

            string sql = string.Format(@"select z.ZoneID,z.CodeGroup,z.Name,c.Code,r.RateID,r.ServicesFlag,r.Rate,p.CurrencyID,r.OffPeakRate,r.WeekendRate,r.Change,
                    r.BeginEffectiveDate,r.EndEffectiveDate,c.BeginEffectiveDate,c.EndEffectiveDate,p.PriceListID,cu.LastRate,cu.IsMainCurrency,p.BeginEffectiveDate
                    from  
                     code c inner join rate r on c.ZoneID=r.ZoneID
                     inner join Zone z on z.ZoneID=r.ZoneID
                     inner join PriceList p on p.PriceListID=r.PriceListID
                     inner join Currency cu on cu.CurrencyID=p.CurrencyID
                     where 
                     p.CustomerID='{0}' and p.SupplierID='{1}'
                     AND r.BeginEffectiveDate > '{2}' 
                     AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >'{2}')
                     AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > r.BeginEffectiveDate)
                    ORDER BY z.Name, c.Code", customer.CarrierAccountID, supplier.CarrierAccountID, DateTime.Today.ToString("yyyy-MM-dd"));
            System.Data.SqlClient.SqlConnection connection = (System.Data.SqlClient.SqlConnection)GetOpenedConnection();
            System.Data.IDataReader reader = ExecuteReader(connection, sql);
            return reader;
        }
        #endregion


    }
}
