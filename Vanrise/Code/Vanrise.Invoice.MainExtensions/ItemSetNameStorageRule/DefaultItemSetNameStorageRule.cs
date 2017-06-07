using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.ItemSetNameStorageRule
{
    public class DefaultItemSetNameStorageRule : ItemSetNameStorageRuleSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("72314BA6-93BE-4662-9AE7-AACC0E965F3B"); }
        }
        public string ItemSetName { get; set; }
        public string StorageConnectionStringKey { get; set; }
        public Vanrise.Entities.TextFilterType Condition { get; set; }
        public override bool IsApplicable(IItemSetNameStorageRuleContext context)
        {
            if (Utilities.IsTextMatched(context.ItemSetName, this.ItemSetName, this.Condition))
            {
                context.StorageConnectionString = this.StorageConnectionStringKey;
                return true;
            }
            return false;
        }
    }
}
