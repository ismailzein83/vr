using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEDefinitionViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public List<AccountBEDefinitionViewSetting> Settings { get; set; }

        public string AccountDefinitionSelectorLabel { get; set; }

        public override string GetURL(Vanrise.Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Retail_BusinessEntity/Views/Account/AccountManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }
     
    public class AccountBEDefinitionViewSetting
    {
        public Guid BusinessEntityId { get; set; }
    }
}
