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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("786EDDD6-1EC7-4C44-889C-E7246B51AED0"); } }
        public string FieldValue { get; set; }
        public override dynamic Evaluate(IRDLCReportParameterValueContext context)
        {
            return this.FieldValue;
        }
    }
}
