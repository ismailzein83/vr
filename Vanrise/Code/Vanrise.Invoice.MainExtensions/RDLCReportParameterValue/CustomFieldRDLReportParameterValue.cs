using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class CustomFieldRDLReportParameterValue : RDLCReportParameterValue
    {
        public override Guid ConfigId { get { return  new Guid("786EDDD6-1EC7-4C44-889C-E7246B51AED0"); } }
        public string FieldValue { get; set; }
        public override dynamic Evaluate(IRDLCReportParameterValueContext context)
        {
            return this.FieldValue;
        }
    }
}
