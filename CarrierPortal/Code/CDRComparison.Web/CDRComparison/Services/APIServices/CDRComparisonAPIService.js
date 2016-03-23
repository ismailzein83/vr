(function (appControllers) {

    'use strict';

    CDRComparisonAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRComparisonAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDRComparison';

        return {
            GetCDRSourceTemplateConfigs: GetCDRSourceTemplateConfigs,
            GetFileReaderTemplateConfigs: GetFileReaderTemplateConfigs
        };

        function GetCDRSourceTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRSourceTemplateConfigs'));
        }

        function GetFileReaderTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFileReaderTemplateConfigs'));
        }
    }

    appControllers.service('CDRComparison_CDRComparisonAPIService', CDRComparisonAPIService);

})(appControllers);