using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class AccountIdentificationRules : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A8098DDE-51C2-4922-B346-32AFF202A4C1"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountidentificationrules-view";
            }
            set
            {

            }
        }
    }
}
