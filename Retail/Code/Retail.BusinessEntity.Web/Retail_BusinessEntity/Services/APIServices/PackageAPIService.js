(function (appControllers) {

    "use strict";
    packageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function packageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "Package";

        function GetFilteredPackages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredPackages"), input);
        }

        function GetPackageEditorRuntime(packageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageEditorRuntime"), {
                packageId: packageId
            });
        }

        function UpdatePackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "UpdatePackage"), packageObject);
        }

        function AddPackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "AddPackage"), packageObject);
        }

        function GetPackageExtendedSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageExtendedSettingsTemplateConfigs"));
        }

        function GetVoiceTypesTemplateConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetVoiceTypesTemplateConfigs"));

        }

        function GetServicePackageItemConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetServicePackageItemConfigs"));

        }
        
        function GetPackagesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackagesInfo"), {
                filter: filter
            });
        }

        function GetFilteredPackageServices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredPackageServices"), input);
        }

        function HasUpdatePackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdatePackage']));
        }

        function HasAddPackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddPackage']));
        }


        return ({
            GetFilteredPackages: GetFilteredPackages,
            GetPackageEditorRuntime: GetPackageEditorRuntime,
            AddPackage: AddPackage,
            UpdatePackage: UpdatePackage,
            GetPackageExtendedSettingsTemplateConfigs: GetPackageExtendedSettingsTemplateConfigs,
            GetVoiceTypesTemplateConfigs: GetVoiceTypesTemplateConfigs,
            GetServicePackageItemConfigs: GetServicePackageItemConfigs,
            GetPackagesInfo: GetPackagesInfo,
            GetFilteredPackageServices: GetFilteredPackageServices,
            HasUpdatePackagePermission: HasUpdatePackagePermission,
            HasAddPackagePermission: HasAddPackagePermission
        });
    }

    appControllers.service('Retail_BE_PackageAPIService', packageAPIService);

})(appControllers);