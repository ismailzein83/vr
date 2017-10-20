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
            return this.AccountIds;
        }
    }
}
