using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class GeneralSettingsManager : IGeneralSettingsManager
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
            UISettings uiSettings = new UISettings() { Parameters = new Dictionary<string, object>() };
            var generalSettingData = GetGeneralSettingData();
            var generalTechnicalSettingData = GetGeneralTechnicalSettingData();

            var gATechnicalSettingData = GetGoogleAnalyticsSetting();

            var masterLayoutSetting = generalSettingData.MasterLayoutData;

            TimeSpan serverUtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            uiSettings.Parameters.Add("ServerUtcOffsetInMinutes",serverUtcOffset.TotalMinutes);
            uiSettings.Parameters.Add("MasterLayoutSetting", masterLayoutSetting);
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

                    uiSettings.Parameters.Add("DefaultURL", defaultURL);
                    uiSettings.Parameters.Add("DefaultURLTitle", defaultView.Title);
                }

                if (generalSettingData.UIData.NormalPrecision.HasValue)
                {
                    uiSettings.Parameters.Add("NormalPrecision", generalSettingData.UIData.NormalPrecision.Value.ToString());
                }


                if (generalSettingData.UIData.LongPrecision.HasValue)
                {
                    uiSettings.Parameters.Add("LongPrecision", generalSettingData.UIData.LongPrecision.Value.ToString());
                }
                if (generalSettingData.UIData.GridPageSize.HasValue)
                {
                    uiSettings.Parameters.Add("GridPageSize", generalSettingData.UIData.GridPageSize.Value.ToString());
                }

                uiSettings.Parameters.Add("MaxSearchRecordCount", generalSettingData.UIData.MaxSearchRecordCount > 0 ? generalSettingData.UIData.MaxSearchRecordCount : 1000);

                uiSettings.Parameters.Add("HorizontalLine", generalSettingData.UIData.HorizontalLine);
                uiSettings.Parameters.Add("AlternativeColor", generalSettingData.UIData.AlternativeColor);
                uiSettings.Parameters.Add("VerticalLine", generalSettingData.UIData.VerticalLine);

            }

            if (gATechnicalSettingData != null)
            {
                uiSettings.Parameters.Add("GoogleAnalyticsEnabled", gATechnicalSettingData.IsEnabled);

                if (gATechnicalSettingData.Account != null)
                {
                    uiSettings.Parameters.Add("GoogleAnalyticsAccount", gATechnicalSettingData.Account);
                }

            }

            var uiExtendedSettingsImplementations = Utilities.GetAllImplementations<UIExtendedSettings>();
            if(uiExtendedSettingsImplementations != null)
            {
                foreach(var item in uiExtendedSettingsImplementations)
                {
                    var convertor = Activator.CreateInstance(item).CastWithValidate<UIExtendedSettings>("uiExtendedSettings");


                    var parameters = convertor.GetUIParameters();
                    if(parameters != null)
                    {
                        foreach(var parameter in parameters)
                        {
                            uiSettings.Parameters.Add(parameter.Key, parameter.Value);
                        }
                    }
                }
            }


            return uiSettings;
        }
       
        public UIParameter GetParameter(string parameterName)
        {
            var uiSettings = GetUIParameters();
            var uiParameterValue =  uiSettings.Parameters.GetRecord(parameterName);
            if (uiParameterValue == null)
                return null;
            return new UIParameter
            {
                Name = parameterName,
                Value = uiParameterValue
            };
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