(function (appControllers) {

    'use strict';

    SaleEntityZoneRoutingProductHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function SaleEntityZoneRoutingProductHistoryAPIService(BaseAPIService, UtilsService, whSBeModuleConfig) {
        var controllerName = 'SaleEntityZoneRoutingProductHistory';

        function GetFilteredSaleEntityZoneRoutingProductHistoryRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredSaleEntityZoneRoutingProductHistoryRecords'), input);
        }

        return {
            GetFilteredSaleEntityZoneRoutingProductHistoryRecords: GetFilteredSaleEntityZoneRoutingProductHistoryRecords
        };
    }

    appControllers.service('WhS_BE_SaleEntityZoneRoutingProductHistoryAPIService', SaleEntityZoneRoutingProductHistoryAPIService);

})(appControllers);