using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.Billing.Business
{
    public class BiilingContractServiceChargeableConditionOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("AE5DEB49-0804-4B3D-8D3C-6254E357ABE0"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            var chargeableConditions = new BiilingContractServiceChargeableConditionManager().GetCachedContractServiceChargeableCondition();

            if (chargeableConditions.Count > 0)
            {
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                foreach (var chargeableCondition in chargeableConditions)
                {
                    if (chargeableCondition.Condition != null)
                    {
                        var recordFitlerGroupContext = new DataRecordDictFilterGenericFieldMatchContext(context.GenericBusinessEntity.FieldValues, context.DefinitionSettings.DataRecordTypeId);
                        if (recordFilterManager.IsFilterGroupMatch(chargeableCondition.Condition, recordFitlerGroupContext))
                        {
                            context.GenericBusinessEntity.FieldValues["ChargeableConditionID"] = chargeableCondition.ID;
                            break;
                        }
                    }
                }
            }
        }
    }
}
