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
        public string FieldValue { get; set; }
        public override dynamic Evaluate(IRDLCReportParameterValueContext context)
        {
            return this.FieldValue;
        }
    }
}
