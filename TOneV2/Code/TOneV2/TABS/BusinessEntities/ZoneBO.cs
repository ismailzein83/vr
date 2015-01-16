using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using NHibernate.Criterion;
using System.Text;

namespace TABS.BusinessEntities
{
    public class ZoneBO
    {
        public static System.Data.IDbConnection GetOpenedConnection()
        {
            System.Data.IDbConnection connection = new System.Data.SqlClient.SqlConnection(TABS.DataConfiguration.Default.Properties["connection.connection_string"].ToString());
            connection.Open();
            return connection;
        }

        public static System.Data.IDataReader ExecuteReader(System.Data.IDbConnection connection, string sql)
        {
            if (connection.State == ConnectionState.Closed) connection.Open();
            System.Data.IDbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        public static IList<TABS.Zone> GetZonesList(TABS.CarrierAccount supplier, DateTime when)
        {
            IList<TABS.Zone> list = TABS.DataConfiguration.CurrentSession
                     .CreateQuery(@"FROM Zone Z
                                             WHERE Z.Supplier = :supplier
                                                AND ((Z.BeginEffectiveDate <= :when AND (Z.EndEffectiveDate IS NULL OR Z.EndEffectiveDate > :when)))
                                             ORDER BY Z.Name")
                    .SetParameter("when", when)
                    .SetParameter("supplier", supplier)
                    .List<TABS.Zone>();
            return list;
        }

        public static IList<TABS.ToDConsideration> GetToDs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime? effectiveDate, int pageIndex, int pageSize, out int RecordsCount)
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

            RecordsCount = criteria.List<TABS.ToDConsideration>().Where(t => !t.Supplier.IsDeleted).ToList().Count;
            return criteria.SetFirstResult(pageSize * (pageIndex - 1)).SetMaxResults(pageSize).List<TABS.ToDConsideration>();
            //     criteria.List<TABS.ToDConsideration>();
        }

        public static void UpdateZoneCodeGroup(int zoneid, string code)
        {
            string sql = string.Format(@"UPDATE Zone
                                        SET
	                                        CodeGroup = '{1}'
                                        FROM zone WITH(NOLOCK)
                                        WHERE 
                                             zoneID ={0}", zoneid, code);

            TABS.DataHelper.ExecuteNonQuery(sql);
        }

        public static IDataReader GetZonesAndCodes(DateTime effectiveDate)
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
            return TABS.DataHelper.ExecuteReader(sql);
        }

        public static IList<TABS.Code> GetZoneCodes(int zoneid)
        {
            string HQL = "SELECT C From Code C WHERE 1=1 AND C.Zone._ZoneID = :zoneid";

            NHibernate.IQuery Query = TABS.DataConfiguration.CurrentSession.CreateQuery(HQL);

            Query.SetParameter("zoneid", zoneid);
            return Query
                .List<TABS.Code>();
        }

        public static IList<Code> GetSupplierCodes(CarrierAccount supplier, DateTime effectivedate)
        {
            return TABS.DataConfiguration.CurrentSession
                    .CreateQuery(string.Format(@"FROM Code C 
                            WHERE 
                            (C.Zone.Supplier = :supplier)  
                            AND ((C.BeginEffectiveDate <= :when AND (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate >= :when)) OR C.BeginEffectiveDate >= :when)"))
                     .SetParameter("when", effectivedate)
                     .SetParameter("supplier", supplier)
                     .List<TABS.Code>();
        }

        public static IList<TABS.Tariff> GetTariffs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime? from)
        {
            string hql = string.Format(@"FROM  Tariff t
                                                 
                                                    WHERE (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                                    {0}
                                                    {1}
                                                    {2}
                                                    {3}
                                                 ORDER BY t.BeginEffectiveDate"
                                                , (from != null) ? " AND (t.EndEffectiveDate > :when OR t.EndEffectiveDate IS NULL) " : ""
                                                , (zone != null) ? " AND t.Zone = :zone" : ""
                                                , (customer != null) ? " AND t.Customer = :customer" : ""
                                                , (supplier != null) ? " AND t.Supplier = :supplier" : "");
            NHibernate.IQuery query = TABS.DataConfiguration.CurrentSession.CreateQuery(hql);
            if (zone != null) query.SetParameter("zone", zone);
            if (customer != null) query.SetParameter("customer", customer);
            if (supplier != null) query.SetParameter("supplier", supplier);
            if (from != null) query.SetParameter("when", from);
            IList<TABS.Tariff> listTariffs = query.List<TABS.Tariff>();
            return listTariffs;
        }

        public static IList<TABS.Tariff> GetTariffs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime? from, int PageIndex, int PageSize, out int RecordCount)
        {
            RecordCount = 0;
            var MultiQuery = TABS.DataConfiguration.CurrentSession.CreateMultiQuery();

            string hql = string.Format(@"FROM  Tariff t                                             
                                                    WHERE (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                                    {0}
                                                    {1}
                                                    {2}
                                                    {3}
                                                 ORDER BY t.BeginEffectiveDate"
                                                , (from != null) ? " AND (t.EndEffectiveDate > :when OR t.EndEffectiveDate IS NULL) " : ""
                                                , (zone != null) ? " AND t.Zone = :zone" : ""
                                                , (customer != null) ? " AND t.Customer = :customer" : ""
                                                , (supplier != null) ? " AND t.Supplier = :supplier" : "");
            NHibernate.IQuery query = TABS.DataConfiguration.CurrentSession.CreateQuery(hql);
            if (zone != null) query.SetParameter("zone", zone);
            if (customer != null) query.SetParameter("customer", customer);
            if (supplier != null) query.SetParameter("supplier", supplier);
            if (from != null) query.SetParameter("when", from);

            MultiQuery.Add(query.SetFirstResult(PageSize * (PageIndex - 1)).SetMaxResults(PageSize));

            string CountQuery = string.Format(@"Select Count(*) FROM  Tariff t                                             
                                                    WHERE (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                                    {0}
                                                    {1}
                                                    {2}
                                                    {3}"
                                                , (from != null) ? " AND (t.EndEffectiveDate > :whenZ OR t.EndEffectiveDate IS NULL) " : ""
                                                , (zone != null) ? " AND t.Zone = :zoneZ" : ""
                                                , (customer != null) ? " AND t.Customer = :customerZ" : ""
                                                , (supplier != null) ? " AND t.Supplier = :supplierZ" : "");

            var countQuery = DataConfiguration.CurrentSession.CreateQuery(CountQuery);
            if (zone != null) countQuery.SetParameter("zoneZ", zone);
            if (customer != null) countQuery.SetParameter("customerZ", customer);
            if (supplier != null) countQuery.SetParameter("supplierZ", supplier);
            if (from != null) countQuery.SetParameter("whenZ", from);

            MultiQuery.Add(countQuery);
            var results = MultiQuery.List();

            System.Collections.IList listTariffs = (System.Collections.IList)results[0];
            int.TryParse((((System.Collections.IList)results[1])[0].ToString()), out RecordCount);
            List<TABS.Tariff> Tarrifsz = new List<Tariff>();
            foreach (var tarrifItem in listTariffs)
            {
                Tarrifsz.Add((TABS.Tariff)tarrifItem);
            }

            return Tarrifsz;
        }

        public static Dictionary<TABS.Zone, List<TABS.Code>> GetZonesAndCodesOfNullEEDs()
        {
            Dictionary<TABS.Zone, List<TABS.Code>> result = new Dictionary<TABS.Zone, List<TABS.Code>>();
            using (var session = TABS.DataConfiguration.OpenSession())
            {
                var data = session.CreateQuery(@"FROM Zone Z, Code C 
                WHERE
                    Z.Supplier = :Supplier
                AND C.Zone = Z
                AND (Z.EndEffectiveDate IS NULL)
                AND (C.EndEffectiveDate is NULL)
                ORDER BY Z.Name, C.Value")
                                             .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM).List<Object[]>();
                foreach (object[] zoneCode in data)
                {
                    var zone = (TABS.Zone)zoneCode[0];

                    //if (zone != null && zone.Name != "")
                    //zone.Name = TABS.Zone.CleanName(zone.Name);

                    var code = (TABS.Code)zoneCode[1];
                    if (!result.Keys.Any(z => z.ZoneID == zone.ZoneID)) //Could use contains key, but for assurance checking by ZoneID
                    {
                        zone.EffectiveCodes = new List<TABS.Code>();
                        result.Add(zone, new List<TABS.Code>());
                        result[zone].Add(code);
                    }
                    zone.EffectiveCodes.Add(code);
                    result[zone].Add(code);

                }
            }
            result.OrderBy(z => z.Key.Name);
            return result;
        }

        public static Dictionary<TABS.Zone, List<TABS.Code>> GetZonesAndCodesWithFutureEED()
        {
            Dictionary<TABS.Zone, List<TABS.Code>> result = new Dictionary<TABS.Zone, List<TABS.Code>>();
            using (var session = TABS.DataConfiguration.OpenSession())
            {
                var data = session.CreateQuery(@"FROM Zone Z, Code C 
                WHERE
                    Z.Supplier = :Supplier
                AND C.Zone = Z
                AND (Z.EndEffectiveDate > :when)
                AND (C.EndEffectiveDate > :when)
                ORDER BY Z.Name, C.Value")
                                             .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                                             .SetParameter("when", DateTime.Now).List<Object[]>();
                foreach (object[] zoneCode in data)
                {
                    var zone = (TABS.Zone)zoneCode[0];

                    //if (zone != null && zone.Name != "")
                    //zone.Name = TABS.Zone.CleanName(zone.Name);

                    var code = (TABS.Code)zoneCode[1];
                    if (!result.Keys.Any(z => z.ZoneID == zone.ZoneID)) //Could use contains key, but for assurance checking by ZoneID
                    {
                        result.Add(zone, new List<TABS.Code>());
                        result[zone].Add(code);
                    }
                    else
                    {
                        result[zone].Add(code);
                    }
                }
            }
            result.OrderBy(z => z.Key.Name);
            return result;
        }

        public static void Exec_bp_FixErroneousEffectiveCodes()
        {
            TABS.DataHelper.ExecuteNonQuery("EXEC bp_FixErroneousEffectiveCodes");
        }

        public static DataSet Exec_bp_IdentifyErrors(string CodeZoneErrors, string RateZoneErrors, string CodeCodeErrors, string RateRateErrors, DateTime? FromDate)
        {
            string sql = @"EXEC bp_IdentifyErrors
                            @CodeZoneErrors=@P1, 
                            @RateZoneErrors=@P2, 
                            @CodeCodeErrors=@P3,
                            @RateRateErrors=@P4,
                            @FromDate=@P5";
            DataSet GetData = TABS.DataHelper.GetDataSet(sql,
                                CodeZoneErrors,
                                RateZoneErrors,
                                CodeCodeErrors,
                                RateRateErrors,
                                FromDate);

            return GetData;
        }

        public static IDataReader GetEffectiveSuppliedZonesADO(CarrierAccount supplier, DateTime whenEffective, string TableName, int pageIndex, int pageSize, int afterZero)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

//            sql.AppendFormat(@"
//                        select   z.ZoneID as ZoneID
//                                ,z.CodeGroup as CodeGroup
//                                ,z.Name as DisplayName
//                                ,z.BeginEffectiveDate as BeginEffectiveDate
//                                ,z.EndEffectiveDate as EndEffectiveDate
//                                ,min(c.Code) as FirstEffectiveCodeValue
//                                
//                                ,r.RateID as RateID
//                                ,r.ServicesFlag as ServiceFlag
//                                ,r.Rate as Rate
//                                ,p.CurrencyID as CurrencyID
//                                ,r.OffPeakRate as OffPeakRate
//                                ,r.WeekendRate as WeekendDate
//                                ,r.Change as Change
//                                ,r.BeginEffectiveDate as RateBeginEffectiveDate
//                                ,r.EndEffectiveDate as RateEndEffectiveDate
//                                ,c.BeginEffectiveDate as CodeBeginEffectiveDate
//                                ,c.EndEffectiveDate as CodeEndEffectiveDate
//                                ,p.PriceListID as PricelistID
//                                ,cu.LastRate as LastRate
//                                ,cu.IsMainCurrency as IsMainCurrency
//                                ,z.Name + ', ' + cast(CAST(r.Rate AS decimal(9,{3})) as nvarchar) + ' ' + cu.CurrencyID + ', ' + 
//                                        CONVERT(varchar(10), cast(r.BeginEffectiveDate as smalldatetime), 103)  as DisplayName2
//                        INTO TempDB.dbo.{0}
//                        from  
//                            code c inner join rate r on c.ZoneID=r.ZoneID
//                            inner join Zone z on z.ZoneID=r.ZoneID
//                            inner join PriceList p on p.PriceListID=r.PriceListID
//                            inner join Currency cu on cu.CurrencyID=p.CurrencyID
//                         where 
//                            p.SupplierID='{1}'
//                            AND r.BeginEffectiveDate < '{2}'
//                            AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >'{2}')
//                            AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > r.BeginEffectiveDate)
//                        Group by z.ZoneID
//                                ,z.CodeGroup
//                                ,z.Name
//                                ,z.BeginEffectiveDate
//                                ,z.EndEffectiveDate
//                                ,z.ServicesFlag
//                        ORDER BY z.Name, c.Code", TableName, supplier.CarrierAccountID, whenEffective.ToString("yyyy-MM-dd hh:mm:ss"),afterZero);
            sql.AppendFormat(@"
                        select   z.ZoneID as ZoneID
                                ,z.CodeGroup as CodeGroup
                                ,z.Name as Name
                                ,z.BeginEffectiveDate as BeginEffectiveDate
                                ,z.EndEffectiveDate as EndEffectiveDate
                                ,min(c.Code) as FirstEffectiveCodeValue
                                ,z.ServicesFlag as ServiceFlag
                        INTO TempDB.dbo.{0}
                        from  
                            code c inner join rate r on c.ZoneID=r.ZoneID
                            inner join Zone z on z.ZoneID=r.ZoneID
                            inner join PriceList p on p.PriceListID=r.PriceListID
                            inner join Currency cu on cu.CurrencyID=p.CurrencyID
                         where 
                            p.SupplierID='{1}'
                            AND r.BeginEffectiveDate < '{2}'
                            AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate >'{2}')
                            AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > r.BeginEffectiveDate)
                        Group by z.ZoneID
                                ,z.CodeGroup
                                ,z.Name
                                ,z.BeginEffectiveDate
                                ,z.EndEffectiveDate
                                ,z.ServicesFlag
                        ORDER BY z.Name, min(c.Code)", TableName, supplier.CarrierAccountID, whenEffective.ToString("yyyy-MM-dd hh:mm:ss"));

            sql.AppendFormat(@"
                    ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
            , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);

            return TABS.DataHelper.ExecuteReader(sql.ToString());
        }

        public static IDataReader GetFutureSuppliedZonesCodesADO(CarrierAccount supplier, DateTime whenEffective, string TableName, int pageIndex, int pageSize)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

            sql.AppendFormat(@"SELECT    z.ZoneID
                                        ,z.CodeGroup
                                        ,z.Name
                                        ,z.BeginEffectiveDate
                                        ,z.EndEffectiveDate
                                        ,c.Code  as FirstEffectiveCodeValue
                                        ,r.RateID
                                        ,r.ServicesFlag
                                        ,r.Rate
                                        ,p.CurrencyID
                                        ,r.OffPeakRate
                                        ,r.WeekendRate
                                        ,r.Change
                                        ,r.BeginEffectiveDate AS RateBeginEffectiveDate
                                        ,r.EndEffectiveDate AS RateEndEffectiveDate
                                        ,c.BeginEffectiveDate AS CodeBeginEffectiveDate
                                        ,c.EndEffectiveDate AS CodeEndEffectiveDate
                                        ,p.PriceListID
                                        ,cu.LastRate
                                        ,cu.IsMainCurrency
                               INTO TempDB.dbo.{0}
                               FROM  
                                        code c 
                                        INNER JOIN rate r on c.ZoneID=r.ZoneID
                                        INNER JOIN Zone z on z.ZoneID=r.ZoneID
                                        INNER JOIN PriceList p on p.PriceListID=r.PriceListID
                                        INNER JOIN Currency cu on cu.CurrencyID=p.CurrencyID
                               WHERE 
                                         p.SupplierID='{1}'
                                         AND z.BeginEffectiveDate > '{2}'
                                         AND ( c.BeginEffectiveDate >'{2}')
                               ORDER BY z.Name, c.Code", TableName, supplier.CarrierAccountID, whenEffective.ToString("yyyy-MM-dd hh:mm:ss"));

            sql.AppendFormat(@"
                    ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
           , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);

            return TABS.DataHelper.ExecuteReader(sql.ToString());
        }




        public static IDataReader GetEndedLaterZonesAndCodesADO(CarrierAccount supplier, DateTime whenEffective, string TableName, int pageIndex, int pageSize)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

            sql.AppendFormat(@"select z.ZoneID
                                    ,z.CodeGroup
                                    ,z.Name
                                    ,z.BeginEffectiveDate
                                    ,z.EndEffectiveDate
                                    ,c.Code as FirstEffectiveCodeValue
                                    ,r.RateID
                                    ,r.ServicesFlag
                                    ,r.Rate
                                    ,p.CurrencyID
                                    ,r.OffPeakRate
                                    ,r.WeekendRate
                                    ,r.Change
                                    ,r.BeginEffectiveDate AS RateBeginEffectiveDate
                                    ,r.EndEffectiveDate AS RateEndEffectiveDate
                                    ,c.BeginEffectiveDate AS CodeBeginEffectiveDate
                                    ,c.EndEffectiveDate AS CodeEndEffectiveDate
                                    ,p.PriceListID
                                    ,cu.LastRate
                                    ,cu.IsMainCurrency
                                INTO TempDB.dbo.{0}
                                from  
                                     code c inner join rate r on c.ZoneID=r.ZoneID
                                     inner join Zone z on z.ZoneID=r.ZoneID
                                     inner join PriceList p on p.PriceListID=r.PriceListID
                                     inner join Currency cu on cu.CurrencyID=p.CurrencyID
                                where 
                                     p.SupplierID='{1}'
                                     AND z.EndEffectiveDate > '{2}'
                                     AND ( c.EndEffectiveDate >'{2}')
                     ORDER BY z.Name, c.Code", TableName, supplier.CarrierAccountID, whenEffective.ToString("yyyy-MM-dd hh:mm:ss"));

            sql.AppendFormat(@"
                    ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
           , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);

            return TABS.DataHelper.ExecuteReader(sql.ToString());


        }

        public static IDataReader GetSupplierZonesHistoryADO(CarrierAccount supplier, string TableName, int pageIndex, int pageSize)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

            sql.AppendFormat(@"select    z.ZoneID
                                        ,z.CodeGroup
                                        ,z.Name
                                        ,z.BeginEffectiveDate as ZoneBeginEffectiveDate
                                        ,z.EndEffectiveDate as ZoneEndEffectiveDate
                                        ,c.Code as FirstEffectiveCodeValue
                                        
                                        ,c.ID
                                        ,c.Code as Code
                                        ,r.RateID
                                        ,r.ServicesFlag
                                        ,r.Rate
                                        ,p.CurrencyID
                                        ,r.OffPeakRate
                                        ,r.WeekendRate
                                        ,r.Change
                                        ,r.BeginEffectiveDate as RateBeginEffectiveDate
                                        ,r.EndEffectiveDate as RateEndEffectiveDate
                                        ,c.BeginEffectiveDate as CodeBeginEffectiveDate
                                        ,c.EndEffectiveDate as CodeEndEffectiveDate
                                        ,p.PriceListID
                                        ,cu.LastRate
                                        ,cu.IsMainCurrency
                                        ,z.Name + ', ' + cast(CAST(r.Rate AS decimal(9,{2})) as nvarchar) + ' ' + cu.CurrencyID + ', ' + CONVERT(varchar(10), cast(r.BeginEffectiveDate as smalldatetime), 103)  as DisplayName 
                                into TempDB.dbo.[{0}]
                                from  
                                     code c 
                                     inner join rate r on c.ZoneID=r.ZoneID
                                     inner join Zone z on z.ZoneID=r.ZoneID
                                     inner join PriceList p on p.PriceListID=r.PriceListID
                                     inner join Currency cu on cu.CurrencyID=p.CurrencyID
                                where 
                                        p.SupplierID='{1}'
                                ORDER BY z.Name, c.Code", TableName, supplier.CarrierAccountID, (int)TABS.SystemParameter.RatesDigitsAfterDot.NumericValue.Value);

            sql.AppendFormat(@"
                    ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
           , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);

            return TABS.DataHelper.ExecuteReader(sql.ToString());


        }
    }

}
