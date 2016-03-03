(function (appControllers) {

    'use strict';

    BusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function BusinessEntityAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetExtensibleBEItemRuntime: GetExtensibleBEItemRuntime,
            GetDataRecordTypesInfo: GetDataRecordTypesInfo,
        };

        function GetExtensibleBEItemRuntime(dataRecordTypeId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntity', 'GetExtensibleBEItemRuntime'), {
                dataRecordTypeId: dataRecordTypeId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function GetDataRecordTypesInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntity', 'GetDataRecordTypesInfo'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

    }

    appControllers.service('VR_GenericData_BusinessEntityAPIService', BusinessEntityAPIService);

})(appControllers);