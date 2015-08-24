using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class ReleaseCodeDataManager : BaseTOneDataManager
    {
        private TrafficStatisticCommon _trafficStatisticCommon = new TrafficStatisticCommon();


        private string BuildFilter(GenericFilter filter)
        {
            StringBuilder whereBuilder = new StringBuilder();
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SwitchIds, "SwitchID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.ZoneIds, "OurZoneID");

            if (filter.SwitchIds == null || filter.SwitchIds.Count == 0)
                whereBuilder.Append("CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)");

            return whereBuilder.ToString();
        }

        private void BuildSelect(IEnumerable<TrafficStatisticGroupKeys> groupKeys, StringBuilder groupKeysSelectPart, StringBuilder groupKeysGroupByPart)
        {
            foreach (var groupKey in groupKeys)
            {
                string columnName;
                string groupByStatement;
                _trafficStatisticCommon.GetColumnStatements(groupKey, out columnName, null, out groupByStatement);
                if (groupByStatement == null)
                {
                    groupByStatement = columnName;
                }
                groupKeysSelectPart.Append(columnName);
                groupKeysSelectPart.Append(",");
                if (groupKeysGroupByPart.Length > 0)
                    groupKeysGroupByPart.Append(",");
                groupKeysGroupByPart.Append(groupByStatement);
            }
        }

        private string CreateTempTableIfNotExists(string tempTableName, GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            StringBuilder whereBuilder = new StringBuilder();
            String zoneCte = String.Empty;
            String joinZone = String.Empty;

            if (filter.CodeGroups != null && filter.CodeGroups.Count > 0)
            {
                zoneCte = String.Format("with Zones AS (SELECT ZoneID FROM Zone z WITH (NOLOCK) WHERE CodeGroup IN  {0} )",String.Join("', '", filter.CodeGroups));
                joinZone = " LEFT JOIN  Zones z ON BT.OurZoneID = z.ZoneID ";
            }

            StringBuilder groupKeysSelectPart = new StringBuilder();
            StringBuilder groupKeysGroupByPart = new StringBuilder();
            BuildSelect(groupKeys, groupKeysSelectPart, groupKeysGroupByPart);
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat(@"
                            IF NOT OBJECT_ID('{2}', N'U') IS NOT NULL
                                BEGIN 
                                
                                {0}
                                
                                SELECT  SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
                                Into #BillingTemp
                                FROM Billing_CDR_INVALID WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
                                WHERE Attempt  BETWEEN @FromDate AND @ToDate {3}


                                INSERT INTO  #BillingTemp
                                SELECT SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
                                FROM Billing_CDR_Main WITH(NOLOCK)
                                WHERE Attempt  BETWEEN @FromDate AND @ToDate {3}
                                
                                Declare @TotalAttempts bigint

                                Select @TotalAttempts=COUNT(*) from #BillingTemp
            
                                DECLARE @ShowNameSuffix nvarchar(1)
                                SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')                                    

                                With PrimaryResult AS (
	
	                                SELECT  {4}
	                                FROM #BillingTemp BT WITH(NOLOCK)  {1}
	                                Group By {5}

	                             ),Suppliers AS
                                 (
                                    Select (CASE WHEN  @ShowNameSuffix ='Y' 
                                            THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
			                            ,A.CarrierAccountID as SupplierID  from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                                 ),CTEResult As(
	                                SELECT  
	                                        SwitchID,
			                                z.ZoneID, 
			                                pr.SupplierID, 
			                                z.[Name],
			                                S.SupplierName,
			                                ReleaseCode,
			                                ReleaseSource,
			                                Sum(DurationsInMinutes) DurationsInMinutes,
			                                Sum(Attempts) Attempts,
			                                Sum(SuccessfulAttempts) SuccessfulAttempts,
			                                Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			                                Min(FirstAttempt) FirstAttempt,	
			                                Max(LastAttempt) LastAttempt,
			                                0 Percentage,
			                                pr.Port_out,
			                                pr.Port_in
	                                From PrimaryResult pr LEFT JOIN Suppliers S on pr.SupplierID = S.SupplierID LEFT JOIN Zone z ON pr.OurZoneID = z.ZoneID
                                    GROUP BY {5}
                                )

                            SELECT * INTO {2}  FROM CTEResult

                            END", zoneCte, joinZone, tempTableName, BuildFilter(filter), groupKeysSelectPart, groupKeysGroupByPart);
            return queryBuilder.ToString();
        }

        public GenericSummaryBigResult<TrafficStatistic> GetReleaseCodeStatistic(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>
            {
                {"Data.Attempts", "Attempts"},
                {"Zone", "OurZoneID"},
                {"Customer", "CustomerID"},
                {"Supplier", "SupplierID"},
                {"Code Group", "CodeGroupID"},
                {"Switch", "SwitchID"},
                {"GateWay In", "GateWayInName"},
                {"GateWay Out", "GateWayOutName"},
                {"Port In", "Port_IN"},
                {"Port Out", "Port_OUT"},
                {"Code Sales", "OurCode"},
                {"Code Buy", "SupplierCode"}
            };

            string columnId;
            _trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[0], out columnId);

            
            mapper.Add("GroupKeyValues[0].Name", columnId);
            

            string tempTable = null;
            Action<string> createTempTableAction = (tempTableName) =>
            {
                tempTable = tempTableName;
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter,input.Query.GroupKeys), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                });
            };
            GenericSummaryBigResult<TrafficStatistic> rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new GroupSummary<TrafficStatistic>
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()],
                    Data = FillTrafficStatisticFromReader(reader)
                };


                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn = null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayIn);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayOut);

                    _trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[i], out idColumn);
                    object id = reader[idColumn];
                    obj.GroupKeyValues[i] = new KeyColumn
                    {
                        Id = id != DBNull.Value ? id.ToString() : "N/A",
                        Name = nameColumn != null && reader[nameColumn] is string ? reader[nameColumn] as string : "N/A"
                    };
                }
                return obj;
            }, mapper, new GenericSummaryBigResult<TrafficStatistic>()) as GenericSummaryBigResult<TrafficStatistic>;

            _trafficStatisticCommon.FillBEProperties<TrafficStatistic>(rslt, input.Query.GroupKeys);
            return rslt;

        }

        TrafficStatistic FillTrafficStatisticFromReader(IDataReader reader)
        {
            TrafficStatistic trafficStatistics = new TrafficStatistic
            {
                FirstCDRAttempt = GetReaderValue<DateTime>(reader, "FirstCDRAttempt"),
                LastCDRAttempt = GetReaderValue<DateTime>(reader, "LastCDRAttempt"),
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                FailedAttempts = GetReaderValue<int>(reader, "FailedAttempts"),
                DeliveredAttempts = GetReaderValue<int>(reader, "DeliveredAttempts"),
                SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts"),
                DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInSeconds")/60,
                MaxDurationInMinutes = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds")/60,
                CeiledDuration = GetReaderValue<long>(reader, "CeiledDuration"),
                ACD = GetReaderValue<Decimal>(reader, "ACD"),
                PDDInSeconds = GetReaderValue<Decimal>(reader, "PDDInSeconds"),
                UtilizationInSeconds = GetReaderValue<Decimal>(reader, "UtilizationInSeconds"),
                NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                DeliveredNumberOfCalls = GetReaderValue<int>(reader, "DeliveredNumberOfCalls"),
                PGAD = GetReaderValue<Decimal>(reader, "PGAD"),
                ABR = GetReaderValue<Decimal>(reader, "ABR"),
                ASR = GetReaderValue<Decimal>(reader, "ASR"),
                NER = GetReaderValue<Decimal>(reader, "NER")
            };
            return trafficStatistics;
        }

    }
}
