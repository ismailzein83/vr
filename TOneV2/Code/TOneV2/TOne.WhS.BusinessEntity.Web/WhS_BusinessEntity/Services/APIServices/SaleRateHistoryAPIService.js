(function (appControllers) {

    'use strict';

    SaleRateHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function SaleRateHistoryAPIService(BaseAPIService, UtilsService, whSBeModuleConfig) {
        var controllerName = 'SaleRateHistory';

        function GetFilteredSaleRateHistoryRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, 'GetFilteredSaleRateHistoryRecords'), input);
        }

        return {
            GetFilteredSaleRateHistoryRecords: GetFilteredSaleRateHistoryRecords
        };
    }

    appControllers.service('WhS_BE_SaleRateHistoryAPIService', SaleRateHistoryAPIService);

})(appControllers);