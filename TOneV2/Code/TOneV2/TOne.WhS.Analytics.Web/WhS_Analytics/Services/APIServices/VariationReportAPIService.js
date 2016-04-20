(function (appControllers) {

    'use strict';

    VariationReportAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function VariationReportAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = 'VariationReport';

        return {
            GetFilteredVariationReportRecords: GetFilteredVariationReportRecords
        };

        function GetFilteredVariationReportRecords(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, controllerName, 'GetFilteredVariationReportRecords'), input);
        }
    }

    appControllers.service('WhS_Analytics_VariationReportAPIService', VariationReportAPIService);

})(appControllers);