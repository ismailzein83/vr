using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;
namespace Vanrise.Common.Business
{
    public class UISettingsManager
    {

        public UISettings GetUIParameters()
        {
            UISettings uiSettings = new UISettings(){Parameters= new List<UIParameter>()};
            ConfigManager configManager = new ConfigManager();
            GeneralSettingData generalSettingData = configManager.GetGeneralSetting();

            if (generalSettingData!=null && generalSettingData.UIData != null)
            {
                uiSettings.Parameters = new List<UIParameter>();
                if (generalSettingData.UIData.DefaultViewId.HasValue)
                {
                    string defaultURL;
                    View defaultView = BEManagerFactory.GetManager<IViewManager>().GetView(generalSettingData.UIData.DefaultViewId.Value);
                    if (defaultView.Settings != null)
                        defaultURL = defaultView.Settings.GetURL(defaultView);
                    else
                        defaultURL = defaultView.Url;
                    uiSettings.Parameters.Add(new UIParameter()
                    {
                        Name = "DefaultURL",
                        Value = defaultURL
                    });
                }

                if (generalSettingData.UIData.NormalPrecision.HasValue)
                {
                    uiSettings.Parameters.Add(new UIParameter()
                    {
                        Name = "NormalPrecision",
                        Value = generalSettingData.UIData.NormalPrecision.Value.ToString()
                    });
                }


                if (generalSettingData.UIData.LongPrecision.HasValue)
                {

                    uiSettings.Parameters.Add(new UIParameter()
                    {
                        Name = "LongPrecision",
                        Value = generalSettingData.UIData.LongPrecision.Value.ToString()
                    });
                }
                if (generalSettingData.UIData.GridPageSize.HasValue)
                {
                    uiSettings.Parameters.Add(new UIParameter()
                    {
                        Name = "GridPageSize",
                        Value = generalSettingData.UIData.GridPageSize.Value.ToString()
                    });
                }

            }

            return uiSettings;
        }

        public UIParameter GetParameter(string parameterName)
        {
            var uiSettings = GetUIParameters();
            return uiSettings.Parameters.FirstOrDefault(x => x.Name == parameterName);
        }

        public string GetNormalPrecision()
        {
            var parameter = GetParameter("NormalPrecision");
            if (parameter == null)
                return null;
            return parameter.Value;
        }
        public string GetLongPrecision()
        {
            var parameter = GetParameter("LongPrecision");
            if (parameter == null)
                return null;
            return parameter.Value;
        }
    }
    
}
