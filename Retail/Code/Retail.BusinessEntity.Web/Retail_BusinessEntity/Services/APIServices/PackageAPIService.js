﻿(function (appControllers) {

    "use strict";
    packageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function packageAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "Package";

        function GetFilteredPackages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredPackages"), input);
        }

        function GetPackage(packageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetPackage"), {
                packageId: packageId
            });
        }

        function GetPackagesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetPackagesInfo"));

        }

        function UpdatePackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdatePackage"), packageObject);
        }

        function AddPackage(packageObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddPackage"), packageObject);
        }

        function HasUpdatePackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdatePackage']));
        }

        function HasAddPackagePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddPackage']));
        }

        return ({
            GetPackagesInfo: GetPackagesInfo,
            GetFilteredPackages: GetFilteredPackages,
            GetPackage: GetPackage,
            AddPackage: AddPackage,
            UpdatePackage: UpdatePackage,
            HasUpdatePackagePermission: HasUpdatePackagePermission,
            HasAddPackagePermission: HasAddPackagePermission
        });
    }

    appControllers.service('Retail_BE_PackageAPIService', packageAPIService);

})(appControllers);