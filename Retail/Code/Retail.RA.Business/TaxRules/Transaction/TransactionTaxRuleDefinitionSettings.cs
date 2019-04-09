using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class TransactionTaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("C2E37C1D-8D58-48B7-B42C-BFB9CEA4703B"); } }
        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("TotalAmount");
            fieldNames.Add("TotalTaxValue");
            return fieldNames;
        }
    }
}
