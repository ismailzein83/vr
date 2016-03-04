 (function (appControllers) {

    'use strict';

    GenericBEAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBEAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
        };


    }

    appControllers.service('VR_GenericData_GenericBEAPIService', GenericBEAPIService);

})(appControllers);