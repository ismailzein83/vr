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

            List<int> overlappingManagerAssignmentsIds = new List<int>();
            AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            var accountManagerAssignmentId = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            var supplierAssigned = (bool)context.GenericBusinessEntity.FieldValues.GetRecord("SupplierAssigned");
            var customerAssigned = (bool)context.GenericBusinessEntity.FieldValues.GetRecord("CustomerAssigned");
            var carrierAccountId = context.GenericBusinessEntity.FieldValues.GetRecord("CarrierAccountId");
            carrierAccountId.ThrowIfNull("CarrierAccountId");
            var bed = context.GenericBusinessEntity.FieldValues.GetRecord("BED");
            bed.ThrowIfNull("BED");
            var eed = context.GenericBusinessEntity.FieldValues.GetRecord("EED");

            var allAccountManagerAssignments = accountManagerAssignmentManager.GetAllAccountManagersAssignmentByCarrierAccountId((int)carrierAccountId);
            var carrierAccount = carrierAccountManager.GetCarrierAccount((int)carrierAccountId);

            if (allAccountManagerAssignments != null)
            {
                foreach (var acctManagerAssgn in allAccountManagerAssignments)
                {
                    if (carrierAccount.AccountType == CarrierAccountType.Customer)
                    {
                        if (acctManagerAssgn.CustomerAssigned && customerAssigned && Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                        {
                            overlappingManagerAssignmentsIds.Add(acctManagerAssgn.AccountManagerAssignmentId);
                            continue;
                        }
                    }
                    else if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                    {
                        if (acctManagerAssgn.SupplierAssigned && supplierAssigned && Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                        {
                            overlappingManagerAssignmentsIds.Add(acctManagerAssgn.AccountManagerAssignmentId);
                            continue;
                        }
                    }

                    else
                    {
                        if (acctManagerAssgn.CustomerAssigned && customerAssigned && Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                        {
                            overlappingManagerAssignmentsIds.Add(acctManagerAssgn.AccountManagerAssignmentId);
                            continue;
                        }

                        if (acctManagerAssgn.SupplierAssigned && supplierAssigned && Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                        {
                            overlappingManagerAssignmentsIds.Add(acctManagerAssgn.AccountManagerAssignmentId);
                            continue;
                        }

                        if (acctManagerAssgn.SupplierAssigned && acctManagerAssgn.CustomerAssigned && customerAssigned && supplierAssigned && Utilities.AreTimePeriodsOverlapped((DateTime)bed, (DateTime?)eed, acctManagerAssgn.BED, acctManagerAssgn.EED))
                        {
                            overlappingManagerAssignmentsIds.Add(acctManagerAssgn.AccountManagerAssignmentId);
                            continue;
                        }
                    }
                }
            }


            if (overlappingManagerAssignmentsIds.Count() > 0)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("The Account Manager Assignment Added is overlapping with the following manager(s) Id(s) : '{0}'", string.Join(",", overlappingManagerAssignmentsIds)));
            }
        }
    }
}
