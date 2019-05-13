using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerAssignmentOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("ADFE8423-2666-4B45-A881-446D8C368E4C"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();
            var accountManagerAssignmentId = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            var carrierAccountId = context.GenericBusinessEntity.FieldValues.GetRecord("CarrierAccountId");
            carrierAccountId.ThrowIfNull("CarrierAccountId");
            var bed = context.GenericBusinessEntity.FieldValues.GetRecord("BED");
            bed.ThrowIfNull("BED");
            var eed = context.GenericBusinessEntity.FieldValues.GetRecord("EED");

            var allAccountManagerAssignments = accountManagerAssignmentManager.GetAllAccountManagersAssignmentByCarrierAccountId((int)carrierAccountId);
            if (allAccountManagerAssignments != null)
            {
                foreach (var acctManagerAssgn in allAccountManagerAssignments)
                {
                    if (accountManagerAssignmentId != null && acctManagerAssgn.AccountManagerAssignmentId == (int)accountManagerAssignmentId)
                        continue;

                    if (Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                    {
                        context.OutputResult.Result = false;
                        context.OutputResult.Messages.Add(string.Format("This account manager assignment is overlapped with another manager"));
                        return;
                    }
                }
            }
        }
    }
}
