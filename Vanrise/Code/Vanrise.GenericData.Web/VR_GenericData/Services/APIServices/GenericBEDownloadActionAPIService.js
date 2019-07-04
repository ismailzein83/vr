(function (appControllers) {

    'use strict';

    GenericBEDownloadActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBEDownloadActionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'GenericBEDownloadAction';

        function DownloadGenericBEFile(input, returnNormalResponse) {
            var responseParameters;
            if (!returnNormalResponse)
                responseParameters = {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                };
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DownloadGenericBEFile'), input, responseParameters);
        }
        return {
            DownloadGenericBEFile: DownloadGenericBEFile
        };
    }

    appControllers.service('VR_GenericData_GenericBEDownloadActionAPIService', GenericBEDownloadActionAPIService);
})(appControllers);