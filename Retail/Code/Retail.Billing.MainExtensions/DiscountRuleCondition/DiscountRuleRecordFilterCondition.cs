using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.MainExtensions.DiscountRuleCondition
{
    public class DiscountRuleRecordFilterCondition : Retail.Billing.Entities.DiscountRuleCondition
    {
        public override Guid ConfigID => new Guid("7913C973-C1A7-4EA2-8002-F1540CCBB708");

        public Guid DataRecordTypeID { get; set; }

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public override bool IsMatched(IDiscountRuleConditionIsMatchedContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            if (this.RecordFilterGroup == null)
                return null;

            return BuildRecordFilterGroupExpression(this.RecordFilterGroup);
        }

        private string BuildRecordFilterGroupExpression(RecordFilterGroup recordFilterGroup)
        {
            Dictionary<string, RecordFilterFieldInfo> recordFilterFieldsInfo = null;

            var dataRecordTypeFields = new DataRecordTypeManager().GetDataRecordTypeFields(this.DataRecordTypeID);
            if (dataRecordTypeFields != null)
            {
                recordFilterFieldsInfo = dataRecordTypeFields.ToDictionary(x => x.Value.Name, x => new RecordFilterFieldInfo()
                {
                    Name = x.Value.Name,
                    Title = x.Value.Title,
                    Type = x.Value.Type
                });
            }

            return new RecordFilterManager().BuildRecordFilterGroupExpression(recordFilterGroup, recordFilterFieldsInfo);
        }
    }
}