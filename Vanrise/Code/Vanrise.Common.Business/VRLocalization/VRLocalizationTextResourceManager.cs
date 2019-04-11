﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
	public class VRLocalizationTextResourceManager
	{
		static Guid businessEntityDefinitionId = new Guid("abe7cdc6-90cc-4b09-a4dc-f52fa2323781");

		#region public methods
		//public IDataRetrievalResult<VRLocalizationTextResourceDetail> GetFilteredVRLocalizationTextResources(DataRetrievalInput<VRLocalizationTextResourceQuery> input)
		//{
		//	var allVRLocalizationTextResources = GetCachedVRLocalizationTextResources();
		//	Func<VRLocalizationTextResource, bool> filterExpression = (item) =>
		//	{
		//		if (input.Query.ResourceKey != null && !item.ResourceKey.ToLower().Contains(input.Query.ResourceKey.ToLower()))
		//			return false;
		//		if (input.Query.ModuleIds != null && !input.Query.ModuleIds.Contains(item.ModuleId))
		//			return false;
		//		return true;
		//	};
		//	return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResources.ToBigResult(input, filterExpression, VRLocalizationTextResourceDetailMapper));
		//}

		public VRLocalizationTextResource GetVRLocalizationTextResource(Guid vrLocalizationTextResourceId)
		{
			var vrLocalizationTextResources = GetCachedVRLocalizationTextResources();
			if (vrLocalizationTextResources == null)
				return null;
			return vrLocalizationTextResources.GetRecord(vrLocalizationTextResourceId);
		}
		public string GetVRLocalizationResourceKey(Guid VRLocalizationTextResourceId)
		{
			var vrLocalizationTextResource = GetVRLocalizationTextResource(VRLocalizationTextResourceId);
			if (vrLocalizationTextResource == null)
				return null;
			return vrLocalizationTextResource.ResourceKey;
		}
		//public InsertOperationOutput<VRLocalizationTextResourceDetail> AddVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResource)
		//{
		//	var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationTextResourceDetail>();

		//	insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
		//	insertOperationOutput.InsertedObject = null;

		//	IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();

		//	vrLocalizationTextResource.VRLocalizationTextResourceId = Guid.NewGuid();

		//	if (dataManager.Insert(vrLocalizationTextResource))
		//	{
		//		Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
		//		insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
		//		insertOperationOutput.InsertedObject = VRLocalizationTextResourceDetailMapper(vrLocalizationTextResource);
		//	}
		//	else
		//	{
		//		insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
		//	}

		//	return insertOperationOutput;
		//}

		//public UpdateOperationOutput<VRLocalizationTextResourceDetail> UpdateVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResource)
		//{
		//	var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationTextResourceDetail>();

		//	updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
		//	updateOperationOutput.UpdatedObject = null;

		//	IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();

		//	if (dataManager.Update(vrLocalizationTextResource))
		//	{
		//		Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
		//		updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
		//		updateOperationOutput.UpdatedObject = VRLocalizationTextResourceDetailMapper(this.GetVRLocalizationTextResource(vrLocalizationTextResource.VRLocalizationTextResourceId));
		//	}
		//	else
		//	{
		//		updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
		//	}

		//	return updateOperationOutput;
		//}
		public IEnumerable<VRLocalizationTextResourceInfo> GetVRLocalizationTextResourceInfo(VRLocalizationTextResourceInfoFilter filter)
		{
			Func<VRLocalizationTextResource, bool> filterExpression = (x) =>
			{
				if (filter != null)
				{
					if (filter.VRLocalizationTextResourceIds != null && filter.VRLocalizationTextResourceIds.Contains(x.VRLocalizationTextResourceId))
						return false;
				}
				return true;
			};
			return this.GetCachedVRLocalizationTextResources().MapRecords(VRLocalizationTextResourceInfoMapper, filterExpression).OrderBy(x => x.ResourceKey);
		}

		public Dictionary<Guid, VRLocalizationTextResource> GetAllResources()
		{
			return GetCachedVRLocalizationTextResources();
		}

		#endregion

		#region private methods
		private Dictionary<Guid, VRLocalizationTextResource> GetCachedVRLocalizationTextResources()
		{
			IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
			return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRLocalizationModule", businessEntityDefinitionId, () =>
			{
				Dictionary<Guid, VRLocalizationTextResource> result = new Dictionary<Guid, VRLocalizationTextResource>();
				IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
				if (genericBusinessEntities != null)
				{
					foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
					{
						VRLocalizationTextResourceSettings VRLocalizationTextResourceSettings = new VRLocalizationTextResourceSettings();
						VRLocalizationTextResourceSettings.DefaultValue = genericBusinessEntity.FieldValues.GetRecord("DefaultValue") as string;
						VRLocalizationTextResource vrLocalizationTextResource = new VRLocalizationTextResource()
						{
							VRLocalizationTextResourceId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
							ResourceKey = genericBusinessEntity.FieldValues.GetRecord("ResourceKey") as string,
							ModuleId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ModuleID"),
							CreatedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
							LastModifiedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
							CreatedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
							LastModifiedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
						};
						result.Add(vrLocalizationTextResource.VRLocalizationTextResourceId, vrLocalizationTextResource);
					}
				}
				return result;
			});


			//return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRLocalizationTextResources",
			//   () =>
			//   {
			//       IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();
			//       IEnumerable<VRLocalizationTextResource> vrLocalizationTextResources = dataManager.GetVRLocalizationTextResources();
			//       return vrLocalizationTextResources.ToDictionary(itm => itm.VRLocalizationTextResourceId, itm => itm);
			//   });
	}


		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			IVRLocalizationTextResourceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return _dataManager.AreVRLocalizationTextResourcesUpdated(ref _updateHandle);
			}
		}

		#endregion

		#region Mapper
		VRLocalizationTextResourceDetail VRLocalizationTextResourceDetailMapper(VRLocalizationTextResource localizationTextResource)
		{

			VRLocalizationTextResourceDetail vrLocalizationTextResourceDetail = new Entities.VRLocalizationTextResourceDetail
			{
				VRLocalizationTextResourceId = localizationTextResource.VRLocalizationTextResourceId,
				ResourceKey = localizationTextResource.ResourceKey,
				ModuleId = localizationTextResource.ModuleId,
				Settings = localizationTextResource.Settings
			};
			VRLocalizationModuleManager vrLocalizationModuleManager = new VRLocalizationModuleManager();
			vrLocalizationTextResourceDetail.ModuleName = vrLocalizationModuleManager.GetVRModuleName(localizationTextResource.ModuleId);
			return vrLocalizationTextResourceDetail;
		}
		public VRLocalizationTextResourceInfo VRLocalizationTextResourceInfoMapper(VRLocalizationTextResource vrTextResourceLanguage)
		{
			VRLocalizationTextResourceInfo vrLocalizationTextResourceInfo = new VRLocalizationTextResourceInfo()
			{
				VRLocalizationTextResourceId = vrTextResourceLanguage.VRLocalizationTextResourceId,
				ResourceKey = vrTextResourceLanguage.ResourceKey
			};
			return vrLocalizationTextResourceInfo;
		}
		#endregion
	}
}
