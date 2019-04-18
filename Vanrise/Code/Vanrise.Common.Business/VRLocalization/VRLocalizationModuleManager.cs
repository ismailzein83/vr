using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRLocalizationModuleManager
	{
		static Guid businessEntityDefinitionId = new Guid("07faa80a-a482-41b2-a7db-b0fe0bf32e0f");


        public VRLocalizationModule GetVRLocalizationModule(Guid vrLocalizationModuleId)
        {
            var vrLocalizationModules = GetCachedVRLocalizationModules();
            if (vrLocalizationModules == null)
                return null;
            return vrLocalizationModules.GetRecord(vrLocalizationModuleId);
        }


		public string GetVRModuleName(Guid ModuleId)
		{
			var module = GetVRLocalizationModule(ModuleId);
			if (module == null)
				return null;
			return module.Name;
		}


        public Dictionary<Guid, VRLocalizationModule> GetAllModules()
        {
            return GetCachedVRLocalizationModules();
        }

        private Dictionary<Guid, VRLocalizationModule> GetCachedVRLocalizationModules()
        {
			IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
			return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRLocalizationModule", businessEntityDefinitionId, () =>
			{
				Dictionary<Guid, VRLocalizationModule> result = new Dictionary<Guid, VRLocalizationModule>();
				IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
				if (genericBusinessEntities != null)
				{
					foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
					{
						VRLocalizationModule vrLocalizationModule = new VRLocalizationModule()
						{
							VRLocalizationModuleId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
							Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
							CreatedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
							LastModifiedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
							CreatedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
							LastModifiedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
						};
						result.Add(vrLocalizationModule.VRLocalizationModuleId, vrLocalizationModule);
					}
				}
				return result;
			});
		}

        

        public IEnumerable<VRLocalizationModuleInfo> GetVRLocalizationModulesInfo(VRLocalizationModuleInfoFilter filter)
        {
            Func<VRLocalizationModule, bool> filterExpression = (item) =>
            {
                return true;
            };
            return this.GetCachedVRLocalizationModules().MapRecords(VRLocalizationModuleInfoMapper, filterExpression).OrderBy(item => item.Name);
        }

        public VRLocalizationModuleInfo VRLocalizationModuleInfoMapper(VRLocalizationModule vrLocalizationModule)
        {
            VRLocalizationModuleInfo vrLocalizationModuleInfo = new VRLocalizationModuleInfo()
            {
                LocalizationModuleId = vrLocalizationModule.VRLocalizationModuleId,
                Name = vrLocalizationModule.Name
            };
            return vrLocalizationModuleInfo;
        }
    }
}
