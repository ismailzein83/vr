(function (appControllers) {

    'use strict';

    DataTransformationStepConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataTransformationStepConfigAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataTransformationSteps: GetDataTransformationSteps,
        });
        function GetDataTransformationSteps(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, "DataTransformationStepConfig", "GetDataTransformationSteps"), { filter: filter });
        }
      
    }

    appControllers.service('VR_GenericData_DataTransformationStepConfigAPIService', DataTransformationStepConfigAPIService);

})(appControllers);
