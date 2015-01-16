using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TABS.BusinessEntities
{
    public class ToDConsiderationBO
    {
        public static IList<TABS.ToDConsideration> GetTODs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, DateTime when)
        {
            string hql = @"FROM  ToDConsideration t
                             WHERE (t.EndEffectiveDate > :when OR t.EndEffectiveDate IS NULL)
                                    AND  (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                    AND  (t.Customer = :customer)
                                    AND  (t.Supplier = :supplier)
                             ORDER BY t.BeginEffectiveDate";
            IList<TABS.ToDConsideration> listTODs = TABS.DataConfiguration.CurrentSession.CreateQuery(hql)
                                                    .SetParameter("when", when)
                                                    .SetParameter("customer", customer)
                                                    .SetParameter("supplier", supplier)
                                                    .List<TABS.ToDConsideration>();
            return listTODs;
        }

        public static IDataReader GetTODsADO(CarrierAccount customer, CarrierAccount supplier, Zone zone, DateTime? when, string TableName, int PageSize)
        {
            StringBuilder query = new StringBuilder();

            query.AppendFormat(@"
                                DECLARE @exists bit
	                            SET @exists=dbo.CheckGlobalTableExists ('{0}')

	                            if(@exists=1)
	                            BEGIN
		                            DECLARE @DropTable varchar(100)
		                            SET @DropTable='Drop TABLE  tempdb.dbo.[{0}]'
		                            exec(@DropTable)
	                            END
                                 SELECT td.ToDConsiderationID
                                ,td.ZoneID,td.SupplierID,td.CustomerID,td.BeginTime,td.EndTime,td.WeekDay,td.HolidayDate,td.HolidayName  
                                ,td.RateType,td.BeginEffectiveDate,td.EndEffectiveDate,td.UserID,z.name,c.NameSuffix AS CustomerNameSuffix,cp.Name AS CustomerName
                                ,( CASE RateType
									WHEN 0 THEN 'Normal'
									WHEN 1 THEN 'OffPeak'
									WHEN 2 THEN 'weekend'
									WHEN 4 THEN 'Holiday'
								END
								
								+'('+

								CASE WeekDay
									WHEN NULL THEN 'Everyday'
									WHEN 0 THEN 'Sunday'
									WHEN 1 THEN 'Monday'
									WHEN 2 THEN 'Tuesday'
									WHEN 3 THEN 'Wednesday'
									WHEN 4 THEN 'Thursday'
									WHEN 5 THEN 'Friday'
									ELSE 'Saturday'
									END 
									
									+ ' From ' + beginTime + ' To ' + EndTime +' On ' + z.name +')' 
									)
						 as DefinitionDisplayS
                                 INTO tempdb.dbo.[{0}]
                                 FROM ToDConsideration td 
                                 left join Zone z on z.ZoneID = td.ZoneID
                                 left join CarrierAccount c on c.CarrierAccountID=td.CustomerID
                                 left join CarrierProfile cp on cp.ProfileID=c.ProfileID                                 
                                 where 1=1 ", TableName);
            if (customer != null)
                query.AppendFormat(" AND td.CustomerID = '{0}' ", customer.CarrierAccountID);
            if (supplier != null)
                query.AppendFormat(" AND td.SupplierID = '{0}' ", supplier.CarrierAccountID);
            if (zone != null)
                query.AppendFormat(" AND td.ZoneID = {0}", zone.ZoneID);
            if (when != null)
                query.AppendFormat(" AND ((td.BeginEffectiveDate <= '{0}' AND (td.EndEffectiveDate IS NULL OR td.EndEffectiveDate > '{0}')) OR td.BeginEffectiveDate > '{0}') ", when.Value.ToString("yyyy-MM-dd"));
            query.Append(" ORDER BY td.BeginEffectiveDate Desc");

            query.AppendFormat(@" SELECT COUNT(1) from tempdb.dbo.[{0}]
                                 ;WITH final AS 
                                  (
                                    SELECT 
                                    T.*, ROW_NUMBER() OVER (ORDER BY BeginEffectiveDate Desc) AS RowNumber
                                    FROM tempdb.dbo.[{0}] T
                                  )
                                 SELECT * FROM final WHERE RowNumber BETWEEN 1 AND {1}", TableName, PageSize);
            return DataHelper.ExecuteReader(query.ToString());
        }
    }
}
