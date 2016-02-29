(function (appControllers) {

    'use strict';

    GenericEditorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericEditorAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetEditorRuntimeMock: GetEditorRuntimeMock,
            GetGenericEditorDefinition: GetGenericEditorDefinition,
            AddGenericEditor: AddGenericEditor,
            GetFilteredGenericEditorDefinitions: GetFilteredGenericEditorDefinitions,
            UpdateGenericEditor: UpdateGenericEditor,
            GetEditorRuntime: GetEditorRuntime
        };

        function GetEditorRuntime(businessEntityId, dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetEditorRuntime'), {
                businessEntityId: businessEntityId,
                dataRecordTypeId: dataRecordTypeId
            });
        }
        function GetEditorRuntimeMock(editorId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetEditorRuntimeMock'), { editorId: editorId });
        }
        function GetGenericEditorDefinition(editorId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetGenericEditorDefinition'), { editorId: editorId });
        }
        function AddGenericEditor(genericEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'AddGenericEditor'), genericEditorObject);
        }

        function UpdateGenericEditor(genericEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'UpdateGenericEditor'), genericEditorObject);
        }
        function GetFilteredGenericEditorDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetFilteredGenericEditorDefinitions'), input);
        }

    }

    appControllers.service('VR_GenericData_GenericEditorAPIService', GenericEditorAPIService);

})(appControllers);