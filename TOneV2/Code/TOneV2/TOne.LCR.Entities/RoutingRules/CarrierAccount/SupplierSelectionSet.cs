using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class SupplierSelectionSet : BaseCarrierAccountSet
    {
        public string SupplierId { get; set; }

        public override bool IsAccountIdIncluded(string accountId)
        {
            return this.SupplierId == accountId;
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }
    }
}
