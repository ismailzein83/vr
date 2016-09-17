(function (appControllers) {
    'use strict';

    DistributorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function DistributorAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'Distributor';

        function GetFilteredDistributors(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredDistributors'), input);
        }

        function AddDistributor(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddDistributor'), account);
        }

        function UpdateDistributor(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateDistributor'), account);
        }

        function GetDistributorsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetDistributorsInfo"));
        }

        function HasAddDistributorPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddDistributor']));
        }

        function HasViewDistributorsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredDistributors']));
        }

        function HasUpdateDistributorPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateDistributor']));
        }


        return {
            GetFilteredDistributors: GetFilteredDistributors,
            AddDistributor: AddDistributor,
            UpdateDistributor: UpdateDistributor,
            GetDistributorsInfo: GetDistributorsInfo,
            HasAddDistributorPermission: HasAddDistributorPermission,
            HasViewDistributorsPermission: HasViewDistributorsPermission,
            HasUpdateDistributorPermission: HasUpdateDistributorPermission
        };

    }
    appControllers.service('Retail_BE_DistributorAPIService', DistributorAPIService);

})(appControllers);