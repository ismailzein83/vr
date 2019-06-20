(function (appControllers) {

    'use strict';

    GenericBEDownloadActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBEDownloadActionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'GenericBEDownloadAction';

        function DownloadGenericBEFile(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DownloadGenericBEFile'), input,
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                });
        }
        return {
            DownloadGenericBEFile: DownloadGenericBEFile
        };
    }

    appControllers.service('VR_GenericData_GenericBEDownloadActionAPIService', GenericBEDownloadActionAPIService);
})(appControllers);