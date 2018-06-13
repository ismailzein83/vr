using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportFileGeneratorGenerateFileContext : IVRAutomatedReportFileGeneratorGenerateFileContext
    {
        public IVRAutomatedReportHandlerExecuteContext HandlerContext { get; set; }
    }
}
