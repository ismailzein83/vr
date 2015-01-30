using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace CallGeneratorLibrary
{
    partial class CallGeneratorModelDataContext
    {
        [System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetScheduleNumbers")]
        [ResultType(typeof(ScheduleNumbers))]
        public IMultipleResults GetScheduleNumbers1([System.Data.Linq.Mapping.ParameterAttribute(Name = "ScheduleId", DbType = "Int")] System.Nullable<int> ScheduleId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), ScheduleId);
            return ((IMultipleResults)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetCDR")]
        [ResultType(typeof(CDRHistory))]
        public IMultipleResults GetCDR1([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "StartDate", DbType = "DateTime")] System.Nullable<System.DateTime> startDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "EndDate", DbType = "DateTime")] System.Nullable<System.DateTime> endDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "Number", DbType = "NVarChar(500)")] string number, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ClientId", DbType = "Int")] System.Nullable<int> clientId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayStart", DbType = "Int")] System.Nullable<int> displayStart, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayLength", DbType = "Int")] System.Nullable<int> displayLength)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), startDate, endDate, number, clientId, displayStart, displayLength);
            return ((IMultipleResults)(result.ReturnValue));
        }


        [System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetChartTotals")]
        [ResultType(typeof(ChartCalls))]
        public IMultipleResults GetChartTotals1([System.Data.Linq.Mapping.ParameterAttribute(Name = "Status", DbType = "Int")] System.Nullable<int> status)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), status);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
