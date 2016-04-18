(function (appControllers) {

    'use strict';

    FileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function FileAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controller = 'File';

        return {
            GetFilteredRecentFiles: GetFilteredRecentFiles,
            GetMaxUncompressedFileSizeInMegaBytes: GetMaxUncompressedFileSizeInMegaBytes
        };

        function GetFilteredRecentFiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, 'GetFilteredRecentFiles'), input);
        }

        function GetMaxUncompressedFileSizeInMegaBytes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, 'GetMaxUncompressedFileSizeInMegaBytes'));
        }
    }

    appControllers.service('VRCommon_FileAPIService', FileAPIService);

})(appControllers);