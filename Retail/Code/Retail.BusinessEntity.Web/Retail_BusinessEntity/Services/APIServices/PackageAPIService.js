(function (appControllers) {

    "use strict";
    packageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function packageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "Package";

        function GetFilteredPackages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredPackages"), input);
        }

        function GetPackage(packageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackage"), {
                packageId: packageId
            });
        }

        function GetPackagesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackagesInfo"));

        }

        function UpdatePackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdatePackage"), packageObject);
        }

        function AddPackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddPackage"), packageObject);
        }

        function HasUpdatePackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdatePackage']));
        }

        function HasAddPackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddPackage']));
        }

        function GetServicesTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetServicesTemplateConfigs"));

        }
        function GetVoiceTypesTemplateConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetVoiceTypesTemplateConfigs"));

        }
        return ({
            GetPackagesInfo: GetPackagesInfo,
            GetFilteredPackages: GetFilteredPackages,
            GetPackage: GetPackage,
            AddPackage: AddPackage,
            UpdatePackage: UpdatePackage,
            HasUpdatePackagePermission: HasUpdatePackagePermission,
            HasAddPackagePermission: HasAddPackagePermission,
            GetServicesTemplateConfigs: GetServicesTemplateConfigs,
            GetVoiceTypesTemplateConfigs: GetVoiceTypesTemplateConfigs
        });
    }

    appControllers.service('Retail_BE_PackageAPIService', packageAPIService);

})(appControllers);