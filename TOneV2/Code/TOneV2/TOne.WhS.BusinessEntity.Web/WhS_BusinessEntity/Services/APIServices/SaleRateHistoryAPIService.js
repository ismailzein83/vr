(function (appControllers) {

    'use strict';

    SaleRateHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function SaleRateHistoryAPIService(BaseAPIService, UtilsService, whSBeModuleConfig) {
        var controllerName = 'SaleRateHistory';

        function GetFilteredSellingProductZoneRateHistoryRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredSellingProductZoneRateHistoryRecords'), input);
        }

        function GetFilteredCustomerZoneRateHistoryRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredCustomerZoneRateHistoryRecords'), input);
        }

        return {
            GetFilteredSellingProductZoneRateHistoryRecords: GetFilteredSellingProductZoneRateHistoryRecords,
            GetFilteredCustomerZoneRateHistoryRecords: GetFilteredCustomerZoneRateHistoryRecords
        };
    }

    appControllers.service('WhS_BE_SaleRateHistoryAPIService', SaleRateHistoryAPIService);

})(appControllers);