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
namespace Retail.BusinessEntity.Business
{
    public class PackageManager
    {
        #region ctor/Local Variables

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<PackageDetail> GetFilteredPackages(Vanrise.Entities.DataRetrievalInput<PackageQuery> input)
        {
            var allPackages = GetCachedPackages();

            Func<Package, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPackages.ToBigResult(input, filterExpression, PackageDetailMapper));
        }
        public Package GetPackage(int packageId)
        {
            var packages = GetCachedPackages();
            return packages.GetRecord(packageId);
        }
        public string GetPackageName(int packageId)
        {
            Package package = GetPackage(packageId);
            return package != null ? package.Name : null;
        }
        public IEnumerable<PackageInfo> GetPackagesInfo(PackageFilter filter)
        {
            var packages = GetCachedPackages();

            Func<Package, bool> filterExpression = null;

            if (filter != null)
            {
                if (filter.AssignedToAccountId.HasValue)
                {
                    var accountPackageManager = new AccountPackageManager();
                    IEnumerable<int> packageIdsAssignedToAccount = accountPackageManager.GetPackageIdsAssignedToAccount(filter.AssignedToAccountId.Value);
                    filterExpression = (package) => !packageIdsAssignedToAccount.Contains(package.PackageId);
                }
            }

            return packages.MapRecords(PackageInfoMapper, filterExpression);
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
                insertOperationOutput.InsertedObject = PackageDetailMapper(package);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
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
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PackageDetailMapper(package);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public IEnumerable<ServiceTemplateConfig> GetServicesTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ServiceTemplateConfig>(Constants.ServicesTemplateConfigsConfigType);
        }
        public IEnumerable<ServiceVoiceTypeTemplateConfig> GetVoiceTypesTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ServiceVoiceTypeTemplateConfig>(Constants.ServiceVoiceTypeTemplateConfigType);
        }
        #endregion

        #region Private Members
        private Dictionary<int, Package> GetCachedPackages()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPackages",
               () =>
               {
                   IPackageDataManager dataManager = BEDataManagerFactory.GetDataManager<IPackageDataManager>();
                   IEnumerable<Package> packages = dataManager.GetPackages();
                   return packages.ToDictionary(cn => cn.PackageId, cn => cn);
               });
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
            packageDetail.Entity = package;
            return packageDetail;
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

        #endregion
    }
}
