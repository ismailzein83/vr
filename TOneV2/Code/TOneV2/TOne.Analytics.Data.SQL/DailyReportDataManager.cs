using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class DailyReportDataManager : BaseTOneDataManager, IDailyReportDataManager
    {
        private string commonWhereClauseConditions;

        public Vanrise.Entities.BigResult<DailyReportCall> GetFilteredDailyReportCalls(Vanrise.Entities.DataRetrievalInput<DailyReportQuery> input, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("ZoneName", "ZoneID");
            mapper.Add("CustomerName", "CustomerID");
            mapper.Add("SupplierName", "SupplierID");
            mapper.Add("Attempts", "Attempts");
            mapper.Add("SuccessfulAttempts", "SuccessfulAttempts");
            mapper.Add("DurationInMinutes", "DurationInMinutes");
            mapper.Add("ASR", "ASR");
            mapper.Add("ACD", "ACD");
            mapper.Add("PDD", "PDD");
            mapper.Add("CostRateDescription", "CostRate");
            mapper.Add("SaleRateDescription", "SaleRate");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.selectedZoneIDs, input.Query.selectedCustomerIDs, input.Query.selectedSupplierIDs, assignedCustomerIDs, assignedSupplierIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TargetDate", input.Query.targetDate));
                });
            };

            Vanrise.Entities.BigResult<DailyReportCall> bigResult = RetrieveData(input, createTempTableAction, DailyReportCallMapper, mapper);

            GetNames(bigResult);

            return bigResult;
        }

        private string CreateTempTableIfNotExists(string tempTableName, List<int> selectedZoneIDs, List<string> selectedCustomerIDs, List<string> selectedSupplierIDs, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                    
                WITH Traffic AS
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

                Billing AS
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
                    T.CustomerID,
                    T.SupplierID,
                    T.Attempts,
                    T.SuccessfulAttempts,
                    T.ASR,
                    T.ACD,
                    T.PDD,
                    T.DurationInMinutes,
                    B.CostRate,
                    B.SaleRate

                INTO #TEMP_TABLE_NAME#

                FROM Traffic T
                LEFT JOIN Billing B ON (T.ZoneID IS NULL OR B.ZoneID = T.ZoneID) 
                    AND (T.SupplierID IS NULL OR B.SupplierID = T.SupplierID) 
                    AND (T.CustomerID IS NULL OR B.CustomerID =  T.CustomerID)
                    AND B.CallDate = T.CallDate
    
                ORDER BY T.CallDate, T.Attempts DESC
                    
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);

            commonWhereClauseConditions = GetCommonWhereClauseConditions(selectedCustomerIDs, selectedSupplierIDs, assignedCustomerIDs, assignedSupplierIDs);

            query.Replace("#TRAFFIC_WHERE_CLAUSE#", GetTrafficWhereClause(selectedZoneIDs));

            query.Replace("#BILLING_WHERE_CLAUSE#", GetBillingWhereClause(selectedZoneIDs));

            return query.ToString();
        }

        private void GetNames(Vanrise.Entities.BigResult<DailyReportCall> calls)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

            foreach (DailyReportCall call in calls.Data)
            {
                if (call.ZoneID != 0)
                    call.ZoneName = manager.GetZoneName(call.ZoneID);
                if (call.CustomerID != null)
                    call.CustomerName = manager.GetCarrirAccountName(call.CustomerID);
                if (call.SupplierID != null)
                    call.SupplierName = manager.GetCarrirAccountName(call.SupplierID);
            }
        }

        private DailyReportCall DailyReportCallMapper(IDataReader reader)
        {
            DailyReportCall call = new DailyReportCall
            {
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                CustomerID = GetReaderValue<string>(reader, "CustomerID"),
                SupplierID = GetReaderValue<string>(reader, "SupplierID"),
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
            string whereClause = "WHERE";

            whereClause += " dbo.DateOf(FirstCDRAttempt) = @TargetDate";
            whereClause += " AND CustomerID NOT IN (SELECT grASc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grASc)";

            if (selectedZoneIDs.Count > 0)
            {
                string selectedZoneIDsList = GetCommaSeparatedList(selectedZoneIDs);
                whereClause += " AND OurZoneID IN (" + selectedZoneIDsList + ")";
            }

            whereClause += commonWhereClauseConditions;

            return whereClause;
        }

        private string GetBillingWhereClause(List<int> selectedZoneIDs)
        {
            string whereClause = "WHERE";

            whereClause += " dbo.DateOf(CallDate) = @TargetDate";

            if (selectedZoneIDs.Count > 0)
            {
                string selectedZoneIDsList = GetCommaSeparatedList(selectedZoneIDs);
                whereClause += " AND SaleZoneID IN (" + selectedZoneIDsList + ")";
            }

            whereClause += commonWhereClauseConditions;

            return whereClause;
        }

        private string GetCommonWhereClauseConditions(List<string> selectedCustomerIDs, List<string> selectedSupplierIDs, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs)
        {
            string whereClause = null;

            if (selectedCustomerIDs.Count > 0 && selectedSupplierIDs.Count > 0)
            {
                string selectedCustomerIDsList = GetCommaSeparatedList(selectedCustomerIDs);
                string selectedSupplierIDsList = GetCommaSeparatedList(selectedSupplierIDs);

                whereClause = " AND ((CustomerID IS NULL OR CustomerID IN (" + selectedCustomerIDsList + ")) OR (SupplierID IS NULL OR SupplierID IN (" + selectedSupplierIDsList + ")))";
            }
            else if (selectedCustomerIDs.Count == 0 && selectedSupplierIDs.Count == 0)
            {
                if (assignedCustomerIDs.Count > 0 && assignedSupplierIDs.Count > 0)
                {
                    string assignedCustomerIDsList = GetCommaSeparatedList(assignedCustomerIDs);
                    string assignedSupplierIDsList = GetCommaSeparatedList(assignedSupplierIDs);

                    whereClause = " AND ((CustomerID IS NULL OR CustomerID IN (" + assignedCustomerIDsList + ")) OR (SupplierID IS NULL OR SupplierID IN (" + assignedSupplierIDsList + ")))";
                }
                else if (assignedCustomerIDs.Count > 0)
                {
                    string assignedCustomerIDsList = GetCommaSeparatedList(assignedCustomerIDs);
                    whereClause = " AND (CustomerID IS NULL OR CustomerID IN (" + assignedCustomerIDsList + "))";
                }
                else if (assignedSupplierIDs.Count > 0)
                {
                    string assignedSupplierIDsList = GetCommaSeparatedList(assignedSupplierIDs);
                    whereClause = " AND (SupplierID IS NULL OR SupplierID IN (" + assignedSupplierIDsList + "))";
                }
            }
            else if (selectedCustomerIDs.Count > 0)
            {
                string selectedCustomerIDsList = GetCommaSeparatedList(selectedCustomerIDs);

                if (assignedSupplierIDs.Count > 0)
                {
                    string assignedSupplierIDsList = GetCommaSeparatedList(assignedSupplierIDs);
                    whereClause = " AND ((CustomerID IS NULL OR CustomerID IN (" + selectedCustomerIDsList + ")) OR (SupplierID IS NULL OR SupplierID IN (" + assignedSupplierIDsList + ")))";
                }
                else
                    whereClause = " AND (CustomerID IS NULL OR CustomerID IN (" + selectedCustomerIDsList + "))";
            }
            else if (selectedSupplierIDs.Count > 0)
            {
                string selectedSupplierIDsList = GetCommaSeparatedList(selectedSupplierIDs);

                if (assignedCustomerIDs.Count > 0)
                {
                    string assignedCustomerIDsList = GetCommaSeparatedList(assignedCustomerIDs);
                    whereClause = " AND ((CustomerID IS NULL OR CustomerID IN (" + assignedCustomerIDsList + ")) OR (SupplierID IS NULL OR SupplierID IN (" + selectedSupplierIDsList + ")))";
                }
                else
                    whereClause = " AND (SupplierID IS NULL OR SupplierID IN (" + selectedSupplierIDsList + "))";
            }

            return whereClause;
        }

        private string GetCommaSeparatedList(List<string> items)
        {
            string list = "";

            for (int i = 0; i < items.Count; i++)
            {
                list += (i > 0) ? (", '" + items[i] + "'") : "'" + items[i] + "'";
            }

            return list;
        }

        private string GetCommaSeparatedList(List<int> items)
        {
            string list = "";

            for (int i = 0; i < items.Count; i++)
            {
                list += (i > 0) ? (", " + items[i].ToString()) : items[i].ToString();
            }

            return list;
        }
    }
}
