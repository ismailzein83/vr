using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class CompanySettingsManager : ICompanySettingsManager
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
