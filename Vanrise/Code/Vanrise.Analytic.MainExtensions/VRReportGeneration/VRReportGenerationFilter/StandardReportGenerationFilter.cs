using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions
{
    public class StandardReportGenerationFilter : VRReportGenerationFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("38974659-FB26-415E-82BC-2895E1D09238"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-analytic-reportgeneration-settings-filter-standardfilterruntime"; }
        }
    }
}
