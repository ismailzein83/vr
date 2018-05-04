using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRAutomatedReportFileGeneratorSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context);
    }

    public interface IVRAutomatedReportFileGeneratorGenerateFileContext
    {
        IVRAutomatedReportHandlerExecuteContext HandlerContext { get; }
    }
}
