(function (appControllers) {

    "use strict";
    ConnectorZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_BE_ModuleConfig'];

    function ConnectorZoneAPIService(BaseAPIService, UtilsService, QM_BE_ModuleConfig) {

        var controllerName = 'ConnectorZone';

        function GetConnectorZoneTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetConnectorZoneTemplates"));
        }
        return ({
            GetConnectorZoneTemplates: GetConnectorZoneTemplates
        });
    }

    appControllers.service('QM_BE_ConnectorZoneAPIService', ConnectorZoneAPIService);

})(appControllers);