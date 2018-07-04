using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public class VRAutomatedReportDateFileNamePart : VRConcatenatedPartSettings<IVRAutomatedReportFileNamePartConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("A194AAC8-0675-4100-8A8B-1FBE4105FE09"); } }
        public string DateFormat { get; set; }
        public override string GetPartText(IVRAutomatedReportFileNamePartConcatenatedPartContext context)
        {
            var date = DateTime.Now; 
            if (!String.IsNullOrEmpty(this.DateFormat))
            {
                return date.ToString(this.DateFormat);
            }
            return date.ToString();
        }
    }
}
