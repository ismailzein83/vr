using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class AccountInfo : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("30064FB0-193D-4C41-A4B9-BFB7E236656B"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountinfo-view";
            }
            set
            {
                
            }
        }
    }
}
