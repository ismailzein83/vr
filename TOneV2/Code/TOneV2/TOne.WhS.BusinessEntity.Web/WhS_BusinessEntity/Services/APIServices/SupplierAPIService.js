(function (appControllers) {

    "use strict";
    supplierAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function supplierAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = 'Supplier';

        function GetSupplierSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierSourceTemplates"));
        }

        return ({
            GetSupplierSourceTemplates: GetSupplierSourceTemplates
        });
    }

    appControllers.service('Whs_BE_SupplierAPIService', supplierAPIService);
})(appControllers);