using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportHandlerValidateContext : IVRAutomatedReportHandlerValidateContext
    {
        public List<VRAutomatedReportQuery> Queries { get; set; }

        public QueryHandlerValidatorResult Result { get; set; }

        public string ErrorMessage { get; set; }
    }
}
