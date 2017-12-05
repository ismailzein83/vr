using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class CompanySettingObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("12D896BB-24FE-42FE-A2C7-CF8C06ED030A"); }
        }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            ConfigManager configManager = new ConfigManager();
            CompanySetting companySetting = configManager.GetCompanySettingById((Guid)context.ObjectId);

            if (companySetting == null)
                throw new DataIntegrityValidationException(string.Format("Company setting not found for ID: '{0}'", context.ObjectId));

            return companySetting;
        }
    }
}
