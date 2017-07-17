using Retail.Cost.Entities;
using System;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Retail.Cost.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public CDRCostTechnicalSettingData GetCDRCostTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = settingManager.GetSetting<CDRCostTechnicalSettingData>(Constants.CDRCostTechnicalSettings);
            cdrCostTechnicalSettingData.ThrowIfNull("cdrCostTechnicalSettingData");

            return cdrCostTechnicalSettingData;
        }

        public TimeSpan GetDurationMargin()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.DurationMargin;
        }

        public TimeSpan GetAttemptDateTimeMargin()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.AttemptDateTimeMargin;
        }

        public TimeSpan GetAttemptDateTimeOffset()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.AttemptDateTimeOffset;
        }

        public int GetMaxBatchDurationInMinutes()
        {
            CDRCostSettingData cdrCostSettingData = GetCDRCostSettingData();
            return cdrCostSettingData.MaxBatchDurationInMinutes;
        }

        public Guid? GetCostCDRReprocessDefinitionId()
        {
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = GetCDRCostTechnicalSettingData();
            return cdrCostTechnicalSettingData.CostCDRReprocessDefinitionId;
        }

        public Guid? GetDataRecordTypeId()
        {
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = GetCDRCostTechnicalSettingData();
            return cdrCostTechnicalSettingData.DataRecordTypeId;
        }

        public RecordFilterGroup GetRecordFilterGroup()
        {
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = GetCDRCostTechnicalSettingData();
            return cdrCostTechnicalSettingData.FilterGroup;
        }

        public ReprocessChunkTimeEnum? GetReprocessChunkTime()
        {
            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = GetCDRCostTechnicalSettingData();
            return cdrCostTechnicalSettingData.ChunkTime;
        }
        #endregion

        #region private methods

        private CDRCostSettingData GetCDRCostSettingData()
        {
            SettingManager settingManager = new SettingManager();
            CDRCostSettingData cdrCostSettingData = settingManager.GetSetting<CDRCostSettingData>(Constants.CDRCostSettings);
            cdrCostSettingData.ThrowIfNull("costSettingData");

            return cdrCostSettingData;
        }

        #endregion
    }
}
