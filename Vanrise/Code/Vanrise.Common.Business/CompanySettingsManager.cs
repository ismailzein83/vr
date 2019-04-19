using System;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class CompanySettingsManager : BaseBusinessEntityManager, ICompanySettingsManager
    {
        public bool SetFilesUsedAndUpdateSettings(ISettingOnBeforeSaveContext context)
        {
            VRFileManager fileManager = new VRFileManager();
            var currentCompanySettings = context.CurrentSettingData as CompanySettings;
            var newCompanySettings = context.NewSettingData as CompanySettings;

            foreach (var newCompanySetting in newCompanySettings.Settings)
            {
                CompanySetting currentCompanySetting = null;
                if (currentCompanySettings != null && currentCompanySettings.Settings != null)
                    currentCompanySetting = currentCompanySettings.Settings.FirstOrDefault(item => item.CompanySettingId == newCompanySetting.CompanySettingId);

                if (context.SaveOperationType == SaveOperationType.Insert || currentCompanySetting == null || currentCompanySetting.CompanyLogo != newCompanySetting.CompanyLogo)
                {
                    var fileSettings = new VRFileSettings { ExtendedSettings = new CompanySettingsFileSettings { CompanySettingId = newCompanySetting.CompanySettingId } };
                    if (!fileManager.SetFileUsedAndUpdateSettings(newCompanySetting.CompanyLogo, fileSettings))
                        return false;
                }
            }
            return true;
        }

        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            ConfigManager configManager = new ConfigManager();
            var companySettingsInfo = configManager.GetCompanySettingsInfo();
            return companySettingsInfo?.Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            Guid companySettingId = (Guid)context.EntityId;
            ConfigManager configManager = new ConfigManager();
            var companySettingsInfo = configManager.GetCompanySettingsInfo();
            return companySettingsInfo.FirstOrDefault(item => item.CompanySettingId == companySettingId);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var companySettings = context.Entity as CompanySettingsInfo;
            return companySettings.CompanySettingId;
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            Guid companySettingId = (Guid)context.EntityId;
            var configManager = new ConfigManager();
            var companySettingsInfo = configManager.GetCompanySettingsInfo();
            var companySettingInfo = companySettingsInfo.FirstOrDefault(item => item.CompanySettingId == companySettingId);
            return companySettingInfo?.CompanyName;
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class CompanySettingsFileSettings : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AF05B35F-6AC0-4F57-AF1E-C8642E823220"); }
        }

        public Guid CompanySettingId { get; set; }

        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            ISecurityContext securityContext = ContextFactory.GetContext(); ;
            return securityContext.HasPermissionToActions("VRCommon/Settings/GetFilteredSettings");
        }
    }
}
