using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRAutomatedReportHandlerSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IVRAutomatedReportHandlerExecuteContext context);

        public virtual void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            context.Result = QueryHandlerValidatorResult.Successful;
        }

    }

    public interface IVRAutomatedReportHandlerExecuteContext
    {
        Guid? TaskId { get; set; }

        VRAutomatedReportResolvedDataList GetDataList(Guid vrAutomatedReportQueryId, string listName);

        VRAutomatedReportResolvedDataFieldValue GetDataField(Guid vrAutomatedReportQueryId, string fieldName);
    }
    public enum QueryHandlerValidatorResult { Successful = 0, Failed = 1}
    public interface IVRAutomatedReportHandlerValidateContext
    {
        List<VRAutomatedReportQuery> Queries { get;}

        QueryHandlerValidatorResult Result { get; set; }

        string ErrorMessage { set; }
    }
}
