using System;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.RA.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public string GetDisputeSerialNumberPattern()
        {
            DisputeSettingsData disputeSettingsData = GetDisputeSettingsData();
            disputeSettingsData.SerialNumberPattern.ThrowIfNull("disputeSettingsData.SerialNumberPattern");
            return disputeSettingsData.SerialNumberPattern;
        }

        public long GetDisputeSerialNumberInitialSequence()
        {
            DisputeSettingsData disputeSettingsData = GetDisputeSettingsData();
            return disputeSettingsData.InitialSequence;
        }

        public Guid GetDisputeOpenMailTemplateId()
        {
            return new Guid("682b517d-aabd-42cd-8758-94c3c458e360");
        }

        public Guid GetDisputePendingMailTemplateId()
        {
            return new Guid("e6d9e6d7-86c8-41db-ab94-f1b6cfb3aa0e");
        }
        public Guid GetDisputeClosedMailTemplateId()
        {
            return new Guid("e4894262-2201-4e0b-9539-361e7ae1fdaa");
        }
        #endregion

        #region Private Methods

        private DisputeSettingsData GetDisputeSettingsData()
        {
            SettingManager settingManager = new SettingManager();
            DisputeSettingsData disputeSettingsData = settingManager.GetSetting<DisputeSettingsData>(Constants.DisputeSettings);

            if (disputeSettingsData == null)
                throw new NullReferenceException("disputeSettingsData");

            return disputeSettingsData;
        }

        #endregion
    }
}
