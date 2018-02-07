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
    public class GeneralSettingsManager
    {
        public GeneralSettingData GetGeneralSettingData()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetGeneralSetting();
        }

        public GeneralTechnicalSettingData GetGeneralTechnicalSettingData()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetGeneralTechnicalSetting();
        }


        public GoogleAnalyticsData GetGoogleAnalyticsSetting()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetGoogleAnalyticsSetting();
        }

        public bool GetGoogleAnalyticsEnabled()
        {
            var gATechnicalSettingData = GetGoogleAnalyticsSetting();
            return gATechnicalSettingData != null && gATechnicalSettingData.IsEnabled;
        }

        public CacheSettingData GetCacheSettingData()
        {
            var generalSettingData = GetGeneralSettingData();
            if (generalSettingData != null && generalSettingData.CacheData != null)
                return generalSettingData.CacheData;
            return null;
        }

        public UISettings GetUIParameters()
        {
            UISettings uiSettings = new UISettings() { Parameters = new List<UIParameter>() };
            var generalSettingData = GetGeneralSettingData();
            var generalTechnicalSettingData = GetGeneralTechnicalSettingData();

            var gATechnicalSettingData = GetGoogleAnalyticsSetting();


            TimeSpan serverUtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            uiSettings.Parameters.Add(new UIParameter()
            {
                Name = "ServerUtcOffsetInMinutes",
                Value = serverUtcOffset.TotalMinutes
            });

            if (generalSettingData != null && generalSettingData.UIData != null)
            {
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

              
                uiSettings.Parameters.Add(new UIParameter()
                {
                    Name = "MaxSearchRecordCount",
                    Value = generalSettingData.UIData.MaxSearchRecordCount > 0 ? generalSettingData.UIData.MaxSearchRecordCount : 1000
                });


                uiSettings.Parameters.Add(new UIParameter()
                {
                    Name = "HorizontalLine",
                    Value = generalSettingData.UIData.HorizontalLine
                });

                uiSettings.Parameters.Add(new UIParameter()
                {
                    Name = "AlternativeColor",
                    Value = generalSettingData.UIData.AlternativeColor
                });

                uiSettings.Parameters.Add(new UIParameter()
                {
                    Name = "VerticalLine",
                    Value = generalSettingData.UIData.VerticalLine
                });

            }

            if (gATechnicalSettingData != null)
            {
                uiSettings.Parameters.Add(new UIParameter()
                {
                    Name = "GoogleAnalyticsEnabled",
                    Value = gATechnicalSettingData.IsEnabled
                });

                if (gATechnicalSettingData.Account != null)
                {
                    uiSettings.Parameters.Add(new UIParameter()
                    {
                        Name = "GoogleAnalyticsAccount",
                        Value = gATechnicalSettingData.Account
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
            return parameter.Value.ToString();
        }

        public int GetNormalPrecisionValue()
        {
            var generalSettings = GetGeneralSettingData();
            if (generalSettings == null)
                throw new NullReferenceException("generalSettings");
            if (generalSettings.UIData == null)
                throw new NullReferenceException("eneralSettings.UIData");
            if (generalSettings.UIData.NormalPrecision == null)
                throw new NullReferenceException("generalSettings.UIData.NormalPrecision");
            return generalSettings.UIData.NormalPrecision.Value;
        }

        public string GetLongPrecision()
        {
            var parameter = GetParameter("LongPrecision");
            if (parameter == null)
                return null;
            return parameter.Value.ToString();
        }

        public int GetLongPrecisionValue()
        {
            var generalSettings = GetGeneralSettingData();
            if (generalSettings == null)
                throw new NullReferenceException("generalSettings");
            if (generalSettings.UIData == null)
                throw new NullReferenceException("eneralSettings.UIData");
            if (generalSettings.UIData.LongPrecision == null)
                throw new NullReferenceException("generalSettings.UIData.LongPrecision");
            return generalSettings.UIData.LongPrecision.Value;
        }

        public string GetLongDateTimeFormat()
        {
            return "yyyy-MM-dd HH:mm:ss";
        }

        public string GetDateTimeFormat()
        {
            return "yyyy-MM-dd HH:mm";
        }

        public string GetDateFormat()
        {
            return "yyyy-MM-dd";
        }
    }

}
