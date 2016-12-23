using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class Actions : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0FF1E64B-15D4-45B8-B616-DDC9B0B78F74"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-actions-view";
            }
            set
            {

            }
        }
    }
}
