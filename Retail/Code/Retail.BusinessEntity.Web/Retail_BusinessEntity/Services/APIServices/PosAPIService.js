(function (appControllers) {
    'use strict';

    PosAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function PosAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'Pos';

        function GetFilteredPointOfSales(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredPointOfSales'), input);
        }

        function AddPointOfSale(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddPointOfSale'), account);
        }

        function UpdatePointOfSale(account) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdatePointOfSale'), account);
        }

        function GetPointOfSalesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPointOfSalesInfo"));
        }

        function HasAddPosPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddPointOfSale']));
        }

        function HasViewPointOfSalesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['GetFilteredPointOfSales']));
        }

        function HasUpdatePointOfSalesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdatePointOfSale']));
        }


        return {
            GetFilteredPointOfSales: GetFilteredPointOfSales,
            AddPointOfSale: AddPointOfSale,
            UpdatePointOfSale: UpdatePointOfSale,
            GetPointOfSalesInfo: GetPointOfSalesInfo,
            HasAddPosPermission: HasAddPosPermission,
            HasViewPointOfSalesPermission: HasViewPointOfSalesPermission,
            HasUpdatePointOfSalesPermission: HasUpdatePointOfSalesPermission
        };

    }
    appControllers.service('Retail_BE_PosAPIService', PosAPIService);

})(appControllers);