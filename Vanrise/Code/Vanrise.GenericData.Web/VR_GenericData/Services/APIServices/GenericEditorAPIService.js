(function (appControllers) {

    'use strict';

    GenericEditorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericEditorAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetEditorRuntimeMock: GetEditorRuntimeMock,
        };

        function GetEditorRuntimeMock(editorId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetEditorRuntimeMock'), { editorId: editorId });
        }

    }

    appControllers.service('VR_GenericData_GenericEditorAPIService', GenericEditorAPIService);

})(appControllers);