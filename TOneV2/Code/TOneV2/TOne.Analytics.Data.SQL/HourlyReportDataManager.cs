using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class HourlyReportDataManager : BaseTOneDataManager, IHourlyReportDataManager
    {
        public Vanrise.Entities.BigResult<string> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            //Dictionary<string, string> mapper = new Dictionary<string, string>();
            //string columnId;
            //GetColumnNames(input.Query.GroupKeys[0], out columnId);
            //mapper.Add("GroupKeyValues[0].Name", columnId);
            //mapper.Add("Zone", "OurZoneID");
            //mapper.Add("Customer", "CustomerID");
            //mapper.Add("Supplier", "SupplierID");
            //mapper.Add("Code Group", "CodeGroupID");
            //mapper.Add("Switch", "SwitchID");
            //mapper.Add("GateWay In", "GateWayInName");
            //mapper.Add("GateWay Out", "GateWayOutName");
            //mapper.Add("Port In", "Port_IN");
            //mapper.Add("Port Out", "Port_OUT");
            //mapper.Add("Code Sales", "OurCode");
            //mapper.Add("Code Buy", "SupplierCode");

            //string tempTable = null;
            //Action<string> createTempTableAction = (tempTableName) =>
            //{
            //    tempTable = tempTableName;
            //    ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.GroupKeys), (cmd) =>
            //    {
            //        cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
            //        cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
            //    });
            //};
            //TrafficStatisticSummaryBigResult rslt = RetrieveData(input, createTempTableAction, (reader) =>
            //{
            //    var obj = new TrafficStatisticGroupSummary
            //    {
            //        GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()]
            //    };
            //    FillTrafficStatisticFromReader(obj, reader);

            //    for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
            //    {
            //        string idColumn;

            //        string nameColumn = null;
            //        if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
            //            nameColumn = CodeGroupNameColumnName;
            //        else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
            //            nameColumn = GateWayInIDColumnName;
            //        else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
            //            nameColumn = GateWayOutIDColumnName;

            //        GetColumnNames(input.Query.GroupKeys[i], out idColumn);
            //        object id = reader[idColumn];
            //        obj.GroupKeyValues[i] = new KeyColumn
            //        {
            //            Id = id != DBNull.Value ? id.ToString() : "N/A",
            //            Name = nameColumn != null && reader[nameColumn] as string != null ? reader[nameColumn] as string : "N/A"
            //        };
            //    }
            //    return obj;
            //}, mapper, new TrafficStatisticSummaryBigResult()) as TrafficStatisticSummaryBigResult;

            //FillBEProperties(rslt, input.Query.GroupKeys);
            //if (input.Query.WithSummary)
            //    rslt.Summary = GetSummary(tempTable);
            Vanrise.Entities.BigResult<string> rslt = new Vanrise.Entities.BigResult<string>();
            return rslt;

        }
    }
}
