using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.PartnerGroup
{
    public class SpecifcAccounts : Vanrise.Invoice.Entities.PartnerGroup
    {
        public override Guid ConfigId { get { return new Guid("CC4DFF84-25C3-4BF7-93BF-48E2F7438476"); } }
        public List<string> AccountIds { get; set; }

        public override List<string> GetPartnerIds(IPartnerGroupContext context)
        {
            if (this.AccountIds == null || this.AccountIds.Count == 0)
                return null;

            DateTime now = DateTime.Now;
            List<string> matchingAccounts = new List<string>();
            foreach (string accountId in this.AccountIds)
            {
                PartnerStatusFilterMatchingContext partnerStatusFilterMatchingContext = new PartnerStatusFilterMatchingContext()
                {
                    AccountId = accountId,
                    EffectiveDate = context.EffectiveDate,
                    InvoiceTypeId = context.InvoiceTypeId,
                    IsEffectiveInFuture = context.IsEffectiveInFuture,
                    Status = context.Status,
                    CurrentDate = now
                };
                if (context.IsStatusFilterMatching(partnerStatusFilterMatchingContext))
                    matchingAccounts.Add(accountId);
            }

            return matchingAccounts != null && matchingAccounts.Count > 0 ? matchingAccounts : null;
        }
    }
}
