using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
namespace CLINumberLibrary
{
    partial class CLINumberModelDataContext
    {
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetRequestCalls")]
        [ResultType(typeof(RequestCallHistory))]
        public IMultipleResults GetRequestCalls1([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "StartDate", DbType = "DateTime")] System.Nullable<System.DateTime> startDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ReleaseDate", DbType = "DateTime")] System.Nullable<System.DateTime> releaseDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "OperatorId", DbType = "Int")] System.Nullable<int> operatorId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayStart", DbType = "Int")] System.Nullable<int> displayStart, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DisplayLength", DbType = "Int")] System.Nullable<int> displayLength)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), startDate, releaseDate, operatorId, displayStart, displayLength);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
