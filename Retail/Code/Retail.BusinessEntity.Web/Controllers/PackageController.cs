using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Package")]
    [JSONWithTypeAttribute]
    public class PackageController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredPackages")]
        public object GetFilteredPackages(Vanrise.Entities.DataRetrievalInput<PackageQuery> input)
        {
            PackageManager manager = new PackageManager();
            return GetWebResponse(input, manager.GetFilteredPackages(input));
        }

        [HttpGet]
        [Route("GetPackageEditorRuntime")]
        public PackageEditorRuntime GetPackageEditorRuntime(int packageId)
        {
            PackageManager manager = new PackageManager();
            return manager.GetPackageEditorRuntime(packageId);
        }

        [HttpPost]
        [Route("AddPackage")]
        public InsertOperationOutput<PackageDetail> AddPackage(Package package)
        {
            PackageManager manager = new PackageManager();
            return manager.AddPackage(package);
        }

        [HttpPost]
        [Route("UpdatePackage")]
        public UpdateOperationOutput<PackageDetail> UpdatePackage(Package package)
        {
            PackageManager manager = new PackageManager();
            return manager.UpdatePackage(package);
        }

        [HttpGet]
        [Route("GetPackageExtendedSettingsTemplateConfigs")]
        public IEnumerable<PackageExtendedSettingsConfig> GetPackageExtendedSettingsTemplateConfigs()
        {
            PackageManager manager = new PackageManager();
            return manager.GetPackageExtendedSettingsTemplateConfigs();
        }

        [HttpGet]
        [Route("GetVoiceTypesTemplateConfigs")]
        public IEnumerable<ServiceVoiceTypeTemplateConfig> GetVoiceTypesTemplateConfigs()
        {
            PackageManager manager = new PackageManager();
            return manager.GetVoiceTypesTemplateConfigs();
        }

        [HttpGet]
        [Route("GetServicePackageItemConfigs")]
        public IEnumerable<ServicePackageItemConfig> GetServicePackageItemConfigs()
        {
            PackageManager manager = new PackageManager();
            return manager.GetServicePackageItemConfigs();
        }

        [HttpGet]
        [Route("GetPackagesInfo")]
        public IEnumerable<PackageInfo> GetPackagesInfo(string filter = null)
        {
            PackageManager manager = new PackageManager();
            var deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<PackageFilter>(filter) : null;
            return manager.GetPackagesInfo(deserializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredPackageServices")]
        public object GetFilteredPackageServices(Vanrise.Entities.DataRetrievalInput<PackageServiceQuery> input)
        {
            PackageManager manager = new PackageManager();
            return GetWebResponse(input, manager.GetFilteredPackageServices(input));
        }
    }
}