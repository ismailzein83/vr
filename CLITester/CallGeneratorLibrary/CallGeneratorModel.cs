using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
namespace CallGeneratorLibrary
{
    partial class CallGeneratorModelDataContext
    {
        [System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetData")]
        [ResultType(typeof(DataCalls))]
        public IMultipleResults GetData1()
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
            return ((IMultipleResults)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetTestOperators")]
        [ResultType(typeof(TestOperatorHistory))]
        public IMultipleResults GetTestOperators1([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "StartDate", DbType = "DateTime")] System.Nullable<System.DateTime> startDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "EndDate", DbType = "DateTime")] System.Nullable<System.DateTime> endDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "OperatorId", DbType = "Int")] System.Nullable<int> operatorId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayStart", DbType = "Int")] System.Nullable<int> displayStart, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayLength", DbType = "Int")] System.Nullable<int> displayLength)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), startDate, endDate, operatorId, displayStart, displayLength);
            return ((IMultipleResults)(result.ReturnValue));
        }


        [System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetChartTotals")]
        [ResultType(typeof(ChartCalls))]
        public IMultipleResults GetChartTotals1([System.Data.Linq.Mapping.ParameterAttribute(Name = "Status", DbType = "Int")] System.Nullable<int> status, [System.Data.Linq.Mapping.ParameterAttribute(Name = "UserId", DbType = "Int")] System.Nullable<int> userId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), status, userId);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
