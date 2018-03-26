using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class PackageManager : BaseBusinessEntityManager
    {
        #region ctor/Local Variables
        static PackageDefinitionManager _packageDefinitionManager = new PackageDefinitionManager();
        static AccountBEManager s_accountBEManager = new AccountBEManager();

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<PackageDetail> GetFilteredPackages(Vanrise.Entities.DataRetrievalInput<PackageQuery> input)
        {            
            var allPackages = GetCachedPackages();

            var allowedViewPackages =_packageDefinitionManager.GetViewAllowedPackageDefinitions();
            Func<Package, bool> filterExpression = (package) =>
            {
                if (input.Query.Name != null && !package.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (allowedViewPackages.Count > 0 && !allowedViewPackages.Contains(package.Settings.PackageDefinitionId))
                    return false;
                return true;
            };
            var resultProcessingHandler = new ResultProcessingHandler<PackageDetail>()
            {
                ExportExcelHandler = new PackageDetailExportExcelHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPackages.ToBigResult(input, filterExpression, PackageDetailMapper), resultProcessingHandler);
        }

        public Package GetPackage(int packageId, bool isViewedFromUI)
        {
            var packages = GetCachedPackages();
            var package = packages.GetRecord(packageId);
            if (package != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new PackageLoggableEntity(_packageDefinitionManager.GetPackageDefinitionAccountBEDefId(package.Settings.PackageDefinitionId)), package);
            return package;
        }
        public Package GetPackage(int packageId)
        { 
            return GetPackage(packageId, false);
        }

        public PackageEditorRuntime GetPackageEditorRuntime(int packageId)
        {
            PackageEditorRuntime packageEditorRuntime = new PackageEditorRuntime();

            packageEditorRuntime.Entity = GetPackage(packageId,true);
            if (packageEditorRuntime.Entity == null)
                throw new NullReferenceException(string.Format("packageEditorRuntime.Entity for Package ID: {0} is null", packageId));

            if (packageEditorRuntime.Entity.Settings == null)
                throw new NullReferenceException(string.Format("packageEditorRuntime.Entity.Settings for Package ID: {0} is null", packageId));

            if (packageEditorRuntime.Entity.Settings.ExtendedSettings == null)
                throw new NullReferenceException(string.Format("packageEditorRuntime.Entity.Settings.ExtendedSettings for Package ID: {0} is null", packageId));

            packageEditorRuntime.ExtendedSettingsEditorRuntime = packageEditorRuntime.Entity.Settings.ExtendedSettings.GetEditorRuntime();
            packageEditorRuntime.PackageDefinition = new PackageDefinitionManager().GetPackageDefinitionById(packageEditorRuntime.Entity.Settings.PackageDefinitionId);
            return packageEditorRuntime;
        }
        public string GetPackageName(int packageId)
        {
            Package package = GetPackage(packageId);
            if (package != null)
                return GetPackageName(package);
            else
                return null;
        }

        public string GetPackageName(Package package)
        {
            return package != null ? package.Name : null;
        }
        public List<Package> GetPackagesByIds(IEnumerable<int> packagesIds)
        {
            List<Package> packages = null;

            if (packagesIds != null && packagesIds.Count() > 0)
            {
                packages = new List<Package>();

                foreach (var id in packagesIds)
                {
                    packages.Add(GetPackage(id));
                }
            }
            return packages;
        }


        public Guid GetPackageAccountDefinitionId(Package package)
        {
            package.Settings.ThrowIfNull("package.Settings", package.PackageId);
            var packageDefinition = _packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
            packageDefinition.ThrowIfNull("packageDefinition", package.Settings.PackageDefinitionId);
            packageDefinition.Settings.ThrowIfNull("packageDefinition.Settings", package.Settings.PackageDefinitionId);
            return packageDefinition.Settings.AccountBEDefinitionId;
        }

        public Guid GetPackageAccountDefinitionId(int packageId)
        {
            var package = GetPackage(packageId);
            package.ThrowIfNull("package", packageId);
            return GetPackageAccountDefinitionId(package);
        }

        public InsertOperationOutput<PackageDetail> AddPackage(Package package)
        {
            InsertOperationOutput<PackageDetail> insertOperationOutput = new InsertOperationOutput<PackageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int packageId = -1;

            IPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageDataManager>();
            bool insertActionSucc = dataManager.Insert(package, out packageId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                package.PackageId = packageId;
                VRActionLogger.Current.TrackAndLogObjectAdded(new PackageLoggableEntity(_packageDefinitionManager.GetPackageDefinitionAccountBEDefId(package.Settings.PackageDefinitionId)), package);
                insertOperationOutput.InsertedObject = PackageDetailMapper(package);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public UpdateOperationOutput<PackageDetail> UpdatePackage(Package package)
        {
            IPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageDataManager>();

            bool updateActionSucc = dataManager.Update(package);
            UpdateOperationOutput<PackageDetail> updateOperationOutput = new UpdateOperationOutput<PackageDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(new PackageLoggableEntity(_packageDefinitionManager.GetPackageDefinitionAccountBEDefId(package.Settings.PackageDefinitionId)), package);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PackageDetailMapper(package);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
       
        public IEnumerable<PackageExtendedSettingsConfig> GetPackageExtendedSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageExtendedSettingsConfig>(PackageExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ServiceVoiceTypeTemplateConfig> GetVoiceTypesTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ServiceVoiceTypeTemplateConfig>(Constants.ServiceVoiceTypeTemplateConfigType);
        }

        public IEnumerable<PackageInfo> GetPackagesInfo(PackageFilter filter)
        {
            Func<Package, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (package) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(package, filter.Filters))
                        return false;

                    if (filter.ExcludedPackageIds != null && filter.ExcludedPackageIds.Contains(package.PackageId))
                        return false;

                    return true;
                };
            }

            return GetCachedPackages().MapRecords(PackageInfoMapper, filterExpression);
        }

        public IEnumerable<ServicePackageItemConfig> GetServicePackageItemConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ServicePackageItemConfig>(ServicePackageItemConfig.EXTENSION_TYPE);
        }

        public Vanrise.Entities.IDataRetrievalResult<PackageServiceDetail> GetFilteredPackageServices(Vanrise.Entities.DataRetrievalInput<PackageServiceQuery> input)
        {
            Package package = this.GetPackage(input.Query.PackageId);
            if (package == null)
                throw new NullReferenceException("package");
            if (package.Settings == null)
                throw new NullReferenceException("package.Settings");
            if (package.Settings.Services == null)
                throw new NullReferenceException("package.Settings.Services");
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, package.Settings.Services.ToBigResult(input, null, PackageServiceDetailMapper));
        }

        public bool IsPackageAvailableInAccountProduct(Guid accountBEDefinitionId, long accountId, int packageId)
        {
            IAccountPayment accountPayment;

            s_accountBEManager.HasAccountPayment(accountBEDefinitionId, accountId, true, out accountPayment);
            if (accountPayment == null)
                throw new NullReferenceException(string.Format("accountPayment for accountId {0}", accountId));
            IEnumerable<int> packageIds = new ProductManager().GetProductPackageIds(accountPayment.ProductId);

            if (packageIds == null || !packageIds.Contains(packageId))
                return false;

            return true;
        }

        #endregion

        #region Private Methods

        public Dictionary<int, Package> GetCachedPackages()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPackages",
               () =>
               {
                   IPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageDataManager>();
                   IEnumerable<Package> packages = dataManager.GetPackages();
                   return packages.ToDictionary(cn => cn.PackageId, cn => cn);
               });
        }

        private bool CheckIfFilterIsMatch(Package package, List<IPackageFilter> filters)
        {
            PackageFilterContext context = new PackageFilterContext { Package = package };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPackageDataManager _dataManager = BEDataManagerFactory.GetDataManager<IPackageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePackagesUpdated(ref _updateHandle);
            }
        }

        private class PackageLoggableEntity : VRLoggableEntityBase
        {
            Guid _accountDefinitionId;
            static AccountBEDefinitionManager _accountBEDefintionManager = new AccountBEDefinitionManager();
            static PackageManager s_packageManager = new PackageManager();
            public PackageLoggableEntity(Guid acountDefinitionId)
            {
                _accountDefinitionId = acountDefinitionId;
            }

            public override string EntityUniqueName
            {
                get { return String.Format("Retail_BusinessEntity_Package_{0}", _accountDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get { return String.Format(_accountBEDefintionManager.GetAccountBEDefinitionName(_accountDefinitionId), "_Packges"); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Retail_BusinessEntity_Package_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Package package = context.Object.CastWithValidate<Package>("context.Object");
                return package.PackageId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Package package = context.Object.CastWithValidate<Package>("context.Object");
                return s_packageManager.GetPackageName(package.PackageId);
               
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }
        }

        private class PackageDetailExportExcelHandler : ExcelExportHandler<PackageDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<PackageDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Credit Classes",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name" });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.PackageId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #region  Mappers

        private PackageInfo PackageInfoMapper(Package package)
        {
            return new PackageInfo()
            {
                PackageId = package.PackageId,
                Name = package.Name,
            };
        }

        private PackageDetail PackageDetailMapper(Package package)
        {
            
            
            PackageDetail packageDetail = new PackageDetail();
            packageDetail.AccountBEDefinitionId = _packageDefinitionManager.GetPackageDefinitionAccountBEDefId(package.Settings.PackageDefinitionId);
            packageDetail.Entity = package;
            packageDetail.AllowEdit = _packageDefinitionManager.DoesUserHaveEditPackageDefinitions(package.Settings.PackageDefinitionId);
            return packageDetail;
        }

        private PackageServiceDetail PackageServiceDetailMapper(PackageItem packageItem)
        {
            var serviceTypeManager = new ServiceTypeManager();
            ChargingPolicyDefinitionSettings settings = serviceTypeManager.GetServiceTypeChargingPolicyDefinitionSettings(packageItem.ServiceTypeId);
            return new PackageServiceDetail()
            {
                Entity = packageItem,
                ServiceTypeName = serviceTypeManager.GetServiceTypeName(packageItem.ServiceTypeId),
                RuleDefinitions = (settings != null) ? settings.RuleDefinitions : null
            };
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedPackages = GetCachedPackages();
            if (cachedPackages != null)
                return cachedPackages.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetPackage(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetPackageName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var package = context.Entity as Package;
            return package.PackageId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
