using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class BPAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("17817576-4DE9-4C00-9BEF-0505007B4F53"); }
        }

        public override string ClientActionName
        {
            get { return "BPAction"; }
        }
                
        public ActionBPDefinitionSettings BPDefinitionSettings { get; set; }
    }
}
