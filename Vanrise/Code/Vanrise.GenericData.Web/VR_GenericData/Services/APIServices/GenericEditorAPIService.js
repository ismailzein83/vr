(function (appControllers) {

    'use strict';

    GenericEditorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericEditorAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetExtensibleBEItemRuntime: GetExtensibleBEItemRuntime
        };

        function GetExtensibleBEItemRuntime(businessEntityId, dataRecordTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericEditor', 'GetExtensibleBEItemRuntime'), {
                businessEntityId: businessEntityId,
                dataRecordTypeId: dataRecordTypeId
            });
        }

    }

    appControllers.service('VR_GenericData_GenericEditorAPIService', GenericEditorAPIService);

})(appControllers);