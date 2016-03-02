(function (appControllers) {

    "use strict";
    supplierPriceListAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_SupPriceList_ModuleConfig'];

    function supplierPriceListAPIService(baseApiService, utilsService, SecurityService, moduleConfig) {

        function importPriceList(obj) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "ImportPriceList"), obj);
        }
        function GetUpdated(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "GetUpdated"), input);
        }
        function GetUploadPriceListTemplates() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "GetUploadPriceListTemplates"));
        }
        function GetResultPriceListTemplates() {
            return baseApiService.get(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "GetResultPriceListTemplates"));
        }
        function HasImportPriceList() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(moduleConfig.moduleName, "PriceList", ['ImportPriceList']));
        }
        function GetBeforeId(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "GetBeforeId"), input);
        }
        return ({
            importPriceList: importPriceList,
            GetUpdated: GetUpdated,
            GetUploadPriceListTemplates: GetUploadPriceListTemplates,
            GetResultPriceListTemplates: GetResultPriceListTemplates,
            HasImportPriceList: HasImportPriceList,
            GetBeforeId: GetBeforeId
        });

    }

    appControllers.service('CP_SupplierPricelist_SupplierPriceListAPIService', supplierPriceListAPIService);

})(appControllers);