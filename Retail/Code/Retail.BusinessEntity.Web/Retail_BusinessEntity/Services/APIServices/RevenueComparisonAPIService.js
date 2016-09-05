(function (appControllers) {

    'use strict';

    RevenueComparisonAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function RevenueComparisonAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'RevenueComparison';

        function GetFilteredRevenueComparisons(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredRevenueComparisons"), input);
        }

        return {
            GetFilteredRevenueComparisons: GetFilteredRevenueComparisons
        };
    }

    appControllers.service('Retail_BE_RevenueComparisonAPIService', RevenueComparisonAPIService);

})(appControllers);