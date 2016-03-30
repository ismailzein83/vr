(function (appControllers) {

    'use strict';

    CDRComparisonAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRComparisonAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDRComparison';

        return {
            GetCDRSourceTemplateConfigs: GetCDRSourceTemplateConfigs,
            GetFileReaderTemplateConfigs: GetFileReaderTemplateConfigs,
            GetCDRComparisonSummary: GetCDRComparisonSummary
        };

        function GetCDRSourceTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRSourceTemplateConfigs'));
        }

        function GetFileReaderTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFileReaderTemplateConfigs'));
        }
        function GetCDRComparisonSummary()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRComparisonSummary'));
        }
    }

    appControllers.service('CDRComparison_CDRComparisonAPIService', CDRComparisonAPIService);

})(appControllers);