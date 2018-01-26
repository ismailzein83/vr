(function (app) {

    'use strict';

    GenericBESerialNumberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBESerialNumberAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = "GenericBESerialNumber";

        function GetSerialNumberPartDefinitionsInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetSerialNumberPartDefinitionsInfo"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }

        return ({
            GetSerialNumberPartDefinitionsInfo: GetSerialNumberPartDefinitionsInfo
        });
    }

    app.service('VR_GenericData_GenericBESerialNumberAPIService', GenericBESerialNumberAPIService);

})(app);
