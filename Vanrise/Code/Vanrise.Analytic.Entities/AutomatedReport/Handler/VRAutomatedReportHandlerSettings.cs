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
    }

    public interface IVRAutomatedReportHandlerExecuteContext
    {
        VRAutomatedReportResolvedDataList GetDataList(string queryName, string listName);

        VRAutomatedReportResolvedDataFieldValue GetDataField(string queryName, string fieldName);
    }
}
