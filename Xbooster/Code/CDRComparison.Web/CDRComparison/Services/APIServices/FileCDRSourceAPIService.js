(function (appControllers) {

    'use strict';

    FileCDRSourceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function FileCDRSourceAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'FileCDRSource';

        return {
            GetMaxUncompressedFileSizeInMegaBytes: GetMaxUncompressedFileSizeInMegaBytes
        };

        function GetMaxUncompressedFileSizeInMegaBytes() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetMaxUncompressedFileSizeInMegaBytes'));
        }
    }

    appControllers.service('CDRComparison_FileCDRSourceAPIService', FileCDRSourceAPIService);

})(appControllers);