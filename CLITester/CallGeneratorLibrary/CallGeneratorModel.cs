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


        [System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetChartTotals")]
        [ResultType(typeof(ChartCalls))]
        public IMultipleResults GetChartTotals1([System.Data.Linq.Mapping.ParameterAttribute(Name = "Status", DbType = "Int")] System.Nullable<int> status)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), status);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
