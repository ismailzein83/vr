using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
namespace Vanrise.Common.Business
{
    public class UISettingsManager
    {

        public UISettings GetUIParameters()
        {
            UISettings uiSettings = new UISettings(){
                Parameters= new List<UIParameter>(),
            };
            uiSettings.Parameters.Add(new UIParameter() { 
                Name="DefaultURL",
                Value = "/view/Security/Views/User/UserManagement"
            });
            return uiSettings;
        }
    }
}
