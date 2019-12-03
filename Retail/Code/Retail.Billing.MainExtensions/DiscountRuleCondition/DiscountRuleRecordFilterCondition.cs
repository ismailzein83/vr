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
        public override Guid ConfigID { get { return new Guid("7913C973-C1A7-4EA2-8002-F1540CCBB708"); } }

        public Guid DataRecordTypeID { get; set; }

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public override bool IsMatched(IDiscountRuleConditionIsMatchedContext context)
        {
            return new RecordFilterManager().IsFilterGroupMatch(RecordFilterGroup, new DataRecordDictFilterGenericFieldMatchContext(context.FieldValues, context.DataRecordFields));
        }

        public override bool AreEqual(object comparedDiscountRuleCondition)
        {
            string newRecordFilterGroupExpression = null;
            if (RecordFilterGroup != null)
                newRecordFilterGroupExpression = BuildRecordFilterGroupExpression(RecordFilterGroup);

            string oldRecordFilterGroupExpression = null;
            if (comparedDiscountRuleCondition != null)
            {
                var oldDiscountRuleRecordFilterCondition = comparedDiscountRuleCondition as DiscountRuleRecordFilterCondition;
                if (oldDiscountRuleRecordFilterCondition != null && oldDiscountRuleRecordFilterCondition.RecordFilterGroup != null)
                    oldRecordFilterGroupExpression = BuildRecordFilterGroupExpression(oldDiscountRuleRecordFilterCondition.RecordFilterGroup);
            }

            return newRecordFilterGroupExpression == oldRecordFilterGroupExpression;
        }

        public override string GetDescription()
        {
            if (RecordFilterGroup == null)
                return null;

            return BuildRecordFilterGroupExpression(RecordFilterGroup);
        }

        private string BuildRecordFilterGroupExpression(RecordFilterGroup recordFilterGroup)
        {
            Dictionary<string, RecordFilterFieldInfo> recordFilterFieldsInfo = null;

            var dataRecordTypeFields = new DataRecordTypeManager().GetDataRecordTypeFields(this.DataRecordTypeID);
            if (dataRecordTypeFields != null)
            {
                recordFilterFieldsInfo = dataRecordTypeFields.ToDictionary(itm => itm.Value.Name, itm => new RecordFilterFieldInfo()
                {
                    Name = itm.Value.Name,
                    Title = itm.Value.Title,
                    Type = itm.Value.Type
                });
            }

            return new RecordFilterManager().BuildRecordFilterGroupExpression(recordFilterGroup, recordFilterFieldsInfo);
        }
    }
}