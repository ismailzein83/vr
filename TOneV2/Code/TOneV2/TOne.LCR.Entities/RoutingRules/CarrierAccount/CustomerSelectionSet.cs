using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class CustomerSelectionSet : BaseCarrierAccountSet
    {
        public MultipleSelection<string> Customers { get; set; }

        public override bool IsAccountIdIncluded(string accountId)
        {
            switch(this.Customers.SelectionOption)
            {
                case MultipleSelectionOption.AllExceptItems: return this.Customers.SelectedValues == null || !this.Customers.SelectedValues.Contains(accountId);
                case MultipleSelectionOption.OnlyItems: return this.Customers.SelectedValues != null && this.Customers.SelectedValues.Contains(accountId);
            }
            return false;
        }

        public override string Description
        {
            get { return String.Format("Customers: {0}", String.Join(",", this.Customers.SelectedValues)); }
        }
    }
}
