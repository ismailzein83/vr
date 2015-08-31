using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Data.SQL;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class DailyReportDataManager : BaseTOneDataManager, IDailyReportDataManager
    {
        private string commonWhereClauseConditions;

        public Vanrise.Entities.BigResult<DailyReportCall> GetFilteredDailyReportCalls(Vanrise.Entities.DataRetrievalInput<DailyReportQuery> input, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("CostRateDescription", "CostRate");
            mapper.Add("SaleRateDescription", "SaleRate");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.SelectedZoneIDs, input.Query.SelectedCustomerIDs, input.Query.SelectedSupplierIDs, assignedCustomerIDs, assignedSupplierIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TargetDate", input.Query.TargetDate));
                });
            };

            return RetrieveData(input, createTempTableAction, DailyReportCallMapper, mapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, List<int> selectedZoneIDs, List<string> selectedCustomerIDs, List<string> selectedSupplierIDs, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                
                WITH CarriersCTE AS
                (
                    SELECT
                        ca.CarrierAccountID,
                        cp.Name AS CarrierName,
                        ca.NameSuffix AS CarrierNameSuffix
                    FROM CarrierAccount ca INNER JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
                ),

                ZonesCTE AS
                (
                    SELECT ZoneID, Name AS ZoneName From Zone WHERE IsEffective = 'Y'
                ),

                TrafficCTE AS
                (
                    SELECT
                        OurZoneID AS ZoneID,
                        SupplierID AS SupplierID,
                        CustomerID AS CustomerID,
                        SUM(Attempts) AS Attempts,
                        SUM(SuccessfulAttempts) AS SuccessfulAttempts,
                        SUM(SuccessfulAttempts) * 100.0 / SUM(Attempts) AS ASR, 
                        CASE WHEN SUM(SuccessfulAttempts) > 0 THEN SUM(DurationsInSeconds) / (60.0 * SUM(SuccessfulAttempts)) ELSE 0 END AS ACD,
                        AVG(PDDinSeconds) AS PDD,
                        SUM(DurationsInSeconds / 60.) AS DurationInMinutes,
                        dbo.dateof(FirstCDRAttempt) AS CallDate

                    FROM TrafficStats WITH(NOLOCK)
                    
                    #TRAFFIC_WHERE_CLAUSE#
                    
                    GROUP BY
                        dbo.dateof(FirstCDRAttempt),
                        SupplierID,
                        CustomerID,
                        OurZoneID
                ),

                BillingCTE AS
                (
                    SELECT
                        SaleZoneID AS ZoneID,
                        SupplierID AS SupplierID,
                        CustomerID AS CustomerID,
                        AVG(Cost_Rate) AS CostRate,
                        AVG(Sale_Rate) AS SaleRate,
                        dbo.DateOf(CallDate) AS CallDate

                    FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
    
                    #BILLING_WHERE_CLAUSE#

                    GROUP BY
                        dbo.dateof(CallDate),
                        SaleZoneID,
                        SupplierID,
                        CustomerID
                )

                SELECT
                    T.ZoneID,
                    Z.ZoneName,
                    T.CustomerID,
                    Customers.CarrierName AS CustomerName,
                    Customers.CarrierNameSuffix AS CustomerNameSuffix,
                    T.SupplierID,
                    Suppliers.CarrierName AS SupplierName,
                    Suppliers.CarrierNameSuffix AS SupplierNameSuffix,
                    T.Attempts,
                    T.SuccessfulAttempts,
                    T.ASR,
                    T.ACD,
                    T.PDD,
                    T.DurationInMinutes,
                    B.CostRate,
                    B.SaleRate

                INTO #TEMP_TABLE_NAME#

                FROM TrafficCTE T
                LEFT JOIN BillingCTE B ON (T.ZoneID IS NULL OR B.ZoneID = T.ZoneID) 
                    AND (T.SupplierID IS NULL OR B.SupplierID = T.SupplierID) 
                    AND (T.CustomerID IS NULL OR B.CustomerID =  T.CustomerID)
                    AND B.CallDate = T.CallDate

                LEFT JOIN ZonesCTE Z ON Z.ZoneID = T.ZoneID
                LEFT JOIN CarriersCTE Customers ON Customers.CarrierAccountID = T.CustomerID
                LEFT JOIN CarriersCTE Suppliers ON Suppliers.CarrierAccountID = T.SupplierID

                ORDER BY T.CallDate, T.Attempts DESC
                    
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);

            commonWhereClauseConditions = GetCommonWhereClauseConditions(selectedCustomerIDs, selectedSupplierIDs, assignedCustomerIDs, assignedSupplierIDs);

            query.Replace("#TRAFFIC_WHERE_CLAUSE#", GetTrafficWhereClause(selectedZoneIDs));

            query.Replace("#BILLING_WHERE_CLAUSE#", GetBillingWhereClause(selectedZoneIDs));

            return query.ToString();
        }

        private DailyReportCall DailyReportCallMapper(IDataReader reader)
        {
            
           
            DailyReportCall call = new DailyReportCall
            {
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                ZoneName = GetReaderValue<string>(reader, "ZoneName"),
                CustomerID = GetReaderValue<string>(reader, "CustomerID"),
                CustomerName = GetCarrierName(GetReaderValue<string>(reader, "CustomerName"), GetReaderValue<string>(reader, "CustomerNameSuffix")),
                SupplierID = GetReaderValue<string>(reader, "SupplierID"),
                SupplierName = GetCarrierName(GetReaderValue<string>(reader, "SupplierName"), GetReaderValue<string>(reader, "SupplierNameSuffix")),
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts"),
                DurationInMinutes = GetReaderValue<Decimal>(reader, "DurationInMinutes"),
                ASR = GetReaderValue<Decimal>(reader, "ASR"),
                ACD = GetReaderValue<Decimal>(reader, "ACD"),
                PDD = GetReaderValue<Decimal>(reader, "PDD"),
                CostRate = GetReaderValue<double>(reader, "CostRate"),
                SaleRate = GetReaderValue<double>(reader, "SaleRate"),
                CostRateDescription = Convert.ToString(GetReaderValue<double>(reader, "CostRate")),
                SaleRateDescription = Convert.ToString(GetReaderValue<double>(reader, "SaleRate"))
            };

            return call;
        }

        private string GetTrafficWhereClause(List<int> selectedZoneIDs)
        {
            StringBuilder whereClause = new StringBuilder(@"WHERE");

            whereClause.Append(" dbo.DateOf(FirstCDRAttempt) = @TargetDate");
            whereClause.Append(" AND CustomerID NOT IN (SELECT grASc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grASc)");

            if (selectedZoneIDs != null)
            {
                string selectedZoneIDsList = GetCommaSeparatedList(selectedZoneIDs);
                whereClause.Append(" AND OurZoneID IN (" + selectedZoneIDsList + ")");
            }

            whereClause.Append(commonWhereClauseConditions);

            return whereClause.ToString();
        }

        private string GetBillingWhereClause(List<int> selectedZoneIDs)
        {
            StringBuilder whereClause = new StringBuilder(@"WHERE");

            whereClause.Append(" dbo.DateOf(CallDate) = @TargetDate");

            if (selectedZoneIDs != null)
            {
                string selectedZoneIDsList = GetCommaSeparatedList(selectedZoneIDs);
                whereClause.Append(" AND SaleZoneID IN (" + selectedZoneIDsList + ")");
            }

            whereClause.Append(commonWhereClauseConditions);

            return whereClause.ToString();
        }

        private string GetCommonWhereClauseConditions(List<string> selectedCustomerIDs, List<string> selectedSupplierIDs, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            StringBuilder whereClause = new StringBuilder();

            if (selectedCustomerIDs != null && selectedSupplierIDs != null)
            {
                whereClause.Append(" AND ((CustomerID IS NULL OR CustomerID IN (#SELECTED_CUSTOMER_IDS#)) OR (SupplierID IS NULL OR SupplierID IN (#SELECTED_SUPPLIER_IDS#)))");
            }
            else if (selectedCustomerIDs == null && selectedSupplierIDs == null)
            {
                if (assignedCustomerIDs != null && assignedSupplierIDs != null)
                {
                    whereClause.Append(" AND ((CustomerID IS NULL OR CustomerID IN (#ASSIGNED_CUSTOMER_IDS#)) OR (SupplierID IS NULL OR SupplierID IN (#ASSIGNED_SUPPLIER_IDS#)))");
                }
                else if (assignedCustomerIDs != null)
                {
                    whereClause.Append(" AND (CustomerID IS NULL OR CustomerID IN (#ASSIGNED_CUSTOMER_IDS#))");
                }
                else if (assignedSupplierIDs != null)
                {
                    whereClause.Append(" AND (SupplierID IS NULL OR SupplierID IN (#ASSIGNED_SUPPLIER_IDS#))");
                }
            }
            else if (selectedCustomerIDs != null)
            {
                if (assignedSupplierIDs != null)
                {
                    whereClause.Append(" AND ((CustomerID IS NULL OR CustomerID IN (#SELECTED_CUSTOMER_IDS#)) OR (SupplierID IS NULL OR SupplierID IN (#ASSIGNED_SUPPLIER_IDS#)))");
                }
                else
                    whereClause.Append(" AND (CustomerID IS NULL OR CustomerID IN (#SELECTED_CUSTOMER_IDS#))");
            }
            else if (selectedSupplierIDs != null)
            {
                if (assignedCustomerIDs != null)
                {
                    whereClause.Append(" AND ((CustomerID IS NULL OR CustomerID IN (#ASSIGNED_CUSTOMER_IDS#)) OR (SupplierID IS NULL OR SupplierID IN (#SELECTED_SUPPLIER_IDS#)))");
                }
                else
                    whereClause.Append(" AND (SupplierID IS NULL OR SupplierID IN (#SELECTED_SUPPLIER_IDS#))");
            }

            whereClause.Replace("#SELECTED_CUSTOMER_IDS#", GetCommaSeparatedList(selectedCustomerIDs));
            whereClause.Replace("#SELECTED_SUPPLIER_IDS#", GetCommaSeparatedList(selectedSupplierIDs));
            whereClause.Replace("#ASSIGNED_CUSTOMER_IDS#", GetCommaSeparatedList(assignedCustomerIDs));
            whereClause.Replace("#ASSIGNED_SUPPLIER_IDS#", GetCommaSeparatedList(assignedSupplierIDs));

            return whereClause.ToString();
        }

        private string GetCommaSeparatedList<T>(IEnumerable<T> items)
        {
            if (items == null) return null;

            if (typeof(T) == typeof(int))
                return string.Join(",", items);
            else
                return "'" + string.Join("', '", items) + "'";
        }

        private string GetCarrierName(string name, string suffix)
        {
            if (suffix != null && suffix != "")
                return name + " (" + suffix + ")";
            else
                return name;
        }
    }
}
