using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Huawei.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Mediation.Huawei.Business
{
    public class ConfigManager
    {
        public HuaweiTechnicalSettingData GetHuaweiTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            HuaweiTechnicalSettingData huaweiTechnicalSettingData = settingManager.GetSetting<HuaweiTechnicalSettingData>(Constants.HuaweiTechnicalSettings);
            huaweiTechnicalSettingData.ThrowIfNull("huaweiTechnicalSettingData");

            return huaweiTechnicalSettingData;
        }
        public RecordFilterGroup GetCDRFilterGroup()
        {
            HuaweiTechnicalSettingData huaweiTechnicalSettingData = GetHuaweiTechnicalSettingData();
            return huaweiTechnicalSettingData.CDR_FilterGroup;
        }
        public RecordFilterGroup GetSMSFilterGroup()
        {
            HuaweiTechnicalSettingData huaweiTechnicalSettingData = GetHuaweiTechnicalSettingData();
            return huaweiTechnicalSettingData.SMS_FilterGroup;
        }
    }
}
