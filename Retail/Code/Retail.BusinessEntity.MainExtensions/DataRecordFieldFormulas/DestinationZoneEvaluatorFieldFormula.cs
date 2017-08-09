using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class DestinationZoneEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("3A88F4C6-5CCE-4A31-A74E-E83BF73A6892"); } }
        public string SubscriberZoneFieldName { get; set; }
        public string OtherPartyZoneFieldName { get; set; }
        public string TrafficDirectionFieldName { get; set; }
        public int TrafficDirectionInputValue { get; set; }
        public int TrafficDirectionOutputValue { get; set; }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            throw new NotImplementedException();
        }

        public override Vanrise.GenericData.Entities.RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
