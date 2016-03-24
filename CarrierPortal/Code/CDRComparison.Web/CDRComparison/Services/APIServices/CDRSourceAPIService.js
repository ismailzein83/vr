(function (appControllers) {

    'use strict';

    CDRSourceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRSourceAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDRSource';

        return {
            ReadSample: ReadSample
        };

        function ReadSample(cdrSource) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'ReadSample'), cdrSource);
        }
    }

    appControllers.service('CDRComparison_CDRSourceAPIService', CDRSourceAPIService);

})(appControllers);