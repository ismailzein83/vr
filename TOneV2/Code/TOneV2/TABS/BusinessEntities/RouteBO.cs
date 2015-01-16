using System.Data;
using System.Text;
using System;

namespace TABS.BusinessEntities
{
    public class RouteBO
    {
        public static IDataReader GetMinMaxUpdatedRoutes()
        {
            return TABS.DataHelper.ExecuteReader("SELECT MIN(Updated), MAX(Updated) FROM [Route] WITH (NOLOCK, INDEX=IX_Route_Updated)");
        }

        public static IDataReader GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime? when, bool forRouteManager)
        {
            StringBuilder query = new StringBuilder();

            query.Append(@"SELECT sr.SpecialRequestID,sr.CustomerID,sr.SupplierID,sr.ZoneID,sr.Code,sr.[Priority],sr.NumberOfTries,
                                  sr.SpecialRequestType,sr.BeginEffectiveDate,sr.EndEffectiveDate,sr.UserID,sr.Percentage,
                                  sr.Reason,sr.RouteChangeHeaderID,sr.IncludeSubCodes,sr.ExcludedCodes,zone.name,
                                  c.NameSuffix,s.NameSuffix,cp.Name,sp.Name,u.Name 
                             FROM SpecialRequest sr 
                             left join Zone on zone.ZoneID = sr.ZoneID
                             left join CarrierAccount c on c.CarrierAccountID=sr.CustomerID
                             left join CarrierAccount s on s.CarrierAccountID=sr.SupplierID
                             left join CarrierProfile cp on cp.ProfileID=c.ProfileID
                             left join CarrierProfile sp on sp.ProfileID=s.ProfileID
                             left join [dbo].[User] u on u.ID =sr.UserID
                            WHERE 1=1");

            if (customer != null) query.AppendFormat(" AND sr.CustomerID = '{0}' ", customer.CarrierAccountID);
            if (supplier != null) query.AppendFormat(" AND sr.SupplierID = '{0}' ", supplier.CarrierAccountID);
            if (!string.IsNullOrEmpty(code)) query.AppendFormat(" AND sr.Code = '{0}' ", code);
            if (zone != null) query.AppendFormat(" AND sr.ZoneID = '{0}' ", zone.ZoneID.ToString());
            if (specialRequestType != null) query.AppendFormat(" AND sr.SpecialRequestType = {0} ", (int)specialRequestType.Value);

            if (when != null)
                query.AppendFormat(" AND ((sr.BeginEffectiveDate <= '{0}' AND (sr.EndEffectiveDate IS NULL OR sr.EndEffectiveDate > '{0}')) OR sr.BeginEffectiveDate > '{0}') ", when.Value.ToString("yyyy-MM-dd"));
            query.Append(" ORDER BY sr.BeginEffectiveDate Desc");

            return TABS.DataHelper.ExecuteReader(query.ToString());
        }
        public static IDataReader GetSpecialRequestsCustomPaging(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime? when, bool forRouteManager, string TableName, int PageSize)
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
                                SELECT sr.SpecialRequestID,sr.CustomerID,sr.SupplierID,sr.ZoneID,sr.Code,sr.[Priority],sr.NumberOfTries,
                                  sr.SpecialRequestType,sr.BeginEffectiveDate,sr.EndEffectiveDate,sr.UserID,sr.Percentage,
                                  sr.Reason,sr.RouteChangeHeaderID,sr.IncludeSubCodes,sr.ExcludedCodes,zone.name,
                                 c.NameSuffix AS CustomerNameSuffix,s.NameSuffix AS SupplierNameSuffix,cp.Name AS CustomerName,sp.Name as SupplierName,u.Name  AS UserName 
                                 INTO tempdb.dbo.[{0}]
                                 FROM SpecialRequest sr 
                                 left join Zone on zone.ZoneID = sr.ZoneID
                                 left join CarrierAccount c on c.CarrierAccountID=sr.CustomerID
                                 left join CarrierAccount s on s.CarrierAccountID=sr.SupplierID
                                 left join CarrierProfile cp on cp.ProfileID=c.ProfileID
                                 left join CarrierProfile sp on sp.ProfileID=s.ProfileID
                                 left join [dbo].[User] u on u.ID =sr.UserID
                                WHERE 1=1", TableName);

            if (customer != null) query.AppendFormat(" AND sr.CustomerID = '{0}' ", customer.CarrierAccountID);
            if (supplier != null) query.AppendFormat(" AND sr.SupplierID = '{0}' ", supplier.CarrierAccountID);
            if (!string.IsNullOrEmpty(code)) query.AppendFormat(" AND sr.Code = '{0}' ", code);
            if (zone != null) query.AppendFormat(" AND sr.ZoneID = '{0}' ", zone.ZoneID.ToString());
            if (specialRequestType != null) query.AppendFormat(" AND sr.SpecialRequestType = {0} ", (int)specialRequestType.Value);

            if (when != null)
                query.AppendFormat(" AND ((sr.BeginEffectiveDate <= '{0}' AND (sr.EndEffectiveDate IS NULL OR sr.EndEffectiveDate > '{0}')) OR sr.BeginEffectiveDate > '{0}') ", when.Value.ToString("yyyy-MM-dd"));
            query.Append(" ORDER BY sr.BeginEffectiveDate Desc");
            query.AppendFormat(@" SELECT COUNT(1) from tempdb.dbo.[{0}]
                                 ;WITH final AS 
                                  (
                                    SELECT 
                                    T.*, ROW_NUMBER() OVER (ORDER BY BeginEffectiveDate Desc) AS RowNumber
                                    FROM tempdb.dbo.[{0}] T
                                  )
                                 SELECT * FROM final WHERE RowNumber BETWEEN 1 AND {1}", TableName, PageSize);

            return TABS.DataHelper.ExecuteReader(query.ToString());
        }

        public static IDataReader GetSupplierZoneBlockADO(TABS.CarrierAccount customer, string zone
                                    , TABS.CarrierAccount supplier, TABS.RouteBlockType? routeBlockType, DateTime? when
                                    , string TableName, int pageIndex, int pageSize)
        {
            StringBuilder query = new StringBuilder();

            query.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

            query.AppendFormat(@"
                        ;with myCarriers as (select CarrierAccountid,IsDeleted,NameSuffix,ProfileID,ActivationStatus from CarrierAccount where IsDeleted='N')
                            ,RouteBlocks AS (SELECT 
                            R.RouteBlockID
                            ,R.CustomerID
                            ,R.SupplierID
                            ,R.ZoneID
                            ,R.code
                            ,R.UserID
                            ,R.UpdateDate
                            ,R.BeginEffectiveDate
                            ,R.EndEffectiveDate
                            ,R.BlockType
                            ,R.IsEffective
                            ,R.Reason
                            ,R.RouteChangeHeaderID
                            ,R.IncludeSubCodes
                            ,R.ExcludedCodes
                            FROM routeblock R WHERE R.CustomerID IS NULL OR R.CustomerID IN (SELECT CarrierAccountid from myCarriers))

                        SELECT 
                            R.RouteBlockID
                            ,R.CustomerID
                            ,R.SupplierID
                            ,R.ZoneID
                            ,R.code
                            ,R.UserID
                            ,R.UpdateDate
                            ,R.BeginEffectiveDate
                            ,R.EndEffectiveDate
                            ,R.BlockType
                            ,R.IsEffective
                            ,R.Reason
                            ,R.RouteChangeHeaderID
                            ,R.IncludeSubCodes
                            ,R.ExcludedCodes
                            ,zone.name as ZoneName
                            ,c.NameSuffix as CustomerNameSuffix
                            ,s.NameSuffix as SupplierNameSuffix
                            ,cp.Name as CustomerProfileName
                            ,sp.Name as SupplierProfileName
                            ,u.Name as UserName
                            ,cp.Name + ' (' + c.NameSuffix + ')' as CustomerName
                            ,sp.Name + ' (' + s.NameSuffix + ')' as SupplierName
                            
                        INTO TempDB.dbo.{0}
                        FROM RouteBlocks R
                        JOIN Zone on zone.ZoneID=r.ZoneID      
                        LEFT JOIN myCarriers c ON (r.CustomerID = C.CarrierAccountID)
                        JOIN myCarriers s ON (R.SupplierID = s.CarrierAccountID)
                        LEFT JOIN CarrierProfile cp on cp.ProfileID=c.ProfileID
                        LEFT JOIN CarrierProfile sp on sp.ProfileID=s.ProfileID
                        LEFT JOIN [dbo].[User] u on u.ID = R.UserID
                        WHERE 1 = 1"
            , TableName);
            if (customer != null) query.AppendFormat("AND ( R.CustomerID = '{0}' OR R.CUSTOMERID IS NULL )", customer.CarrierAccountID);
            if (supplier != null) query.AppendFormat(" AND R.SupplierID = '{0}' ", supplier.CarrierAccountID);
            if (!string.IsNullOrEmpty(zone)) query.AppendFormat(" AND zone.Name like '%{0}%' ", zone);
            if (routeBlockType != null) query.AppendFormat(" AND R.BlockType = {0} ", (int)routeBlockType.Value);

            query.AppendFormat(" AND ((R.BeginEffectiveDate <= '{0}' AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > '{0}')) OR R.BeginEffectiveDate > '{0}') ", when.Value.ToString("yyyy-MM-dd"));
            query.Append(" ORDER BY R.BeginEffectiveDate Desc");

            query.AppendFormat(@"
                        ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
            , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);


            return TABS.DataHelper.ExecuteReader(query.ToString());
        }

        public static DataTable GetCodeRoutes(string query)
        {

            return TABS.DataHelper.GetDataTable(query);
        }

        public static IDataReader GetRouteOverridesADO(CarrierAccount customer, CarrierAccount supplier, string userOptionOrder, string filterUser, string filterZoneCode, bool customerChecked
                                                        , string TableName, int pageIndex, int pageSize)
        {
            StringBuilder query = new StringBuilder();

            query.AppendFormat(@"
                        DECLARE @exists bit
                        SET @exists=dbo.CheckGlobalTableExists('{0}')                      
                        IF(@Exists = 1)
	                    BEGIN
		                    DROP TABLE TempDB.dbo.[{0}] 
	                    END  ", TableName);

            query.AppendFormat(@"SELECT 
                                    ro.CustomerID
                                    ,ro.Code
                                    ,ro.IncludeSubCodes
                                    ,ro.OurZoneID
                                    ,ro.RouteOptions
                                    ,ro.BlockedSuppliers
                                    ,ro.BeginEffectiveDate
                                    ,ro.EndEffectiveDate
                                    ,ro.[Weight]
                                    ,ro.UserID
                                    ,ro.ExcludedCodes
                                    ,ro.Reason
                                    ,z.[Name]
                                    ,cp.Name as ProfileName
                                    ,u.Name as UserName
                                    ,u.ID
                             INTO TempDB.dbo.{0}
                             FROM RouteOverride ro
                             LEFT JOIN Zone z ON z.ZoneID = ro.OurZoneID
                             left join CarrierAccount c on c.CarrierAccountID=ro.CustomerID
                             left join CarrierProfile cp on cp.ProfileID=c.ProfileID
                             left join [dbo].[User] u on u.ID = ro.UserID
                             where ro.IsEffective = 'Y' AND z.IsEffective = 'Y' ", TableName);
            if (customer != null)
                query.AppendFormat(" AND ro.CustomerID = '{0}'", customer.CarrierAccountID);
            if (supplier != null)
                query.AppendFormat(" AND ro.RouteOptions like '%{0}%'", supplier.CarrierAccountID);
            if (!string.IsNullOrEmpty(filterUser))
                query.AppendFormat(" AND u.Name like '%{0}%'", filterUser);
            if (!string.IsNullOrEmpty(filterZoneCode))
                query.AppendFormat(" AND (ro.Code like '%{0}%'  or z.[Name] like '%{0}%' )", filterZoneCode);
            if (customerChecked)
            {
                if (userOptionOrder == "Code")
                    query.AppendFormat(" Order by ro.Code");
                else
                    query.AppendFormat(" Order by z.Name");
            }
            else
            {
                if (userOptionOrder == "Code")
                    query.AppendFormat(" Order by cp.Name,ro.Code");
                else
                    query.AppendFormat(" Order by cp.Name,z.Name");
            }

            query.AppendFormat(@"
                        ;WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN {1} AND {2}
                          SELECT COUNT(1) FROM TempDB.dbo.[{0}];"
           , TableName, (pageIndex - 1) * pageSize + 1, (((pageIndex - 1) * pageSize + 1) + pageSize) - 1);

            return DataHelper.ExecuteReader(query.ToString());
        }
    }
}
