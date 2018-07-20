(function (appControllers) {

    "use strict";

    DBReplicationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function DBReplicationDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "DBReplicationDefinition";

        function GetDBReplicationDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDBReplicationDefinitionsInfo"), {
                filter: filter
            });
        }

        function GetDBDefinitionsInfo(dbReplicationDefinitionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDBDefinitionsInfo"), {
                dbReplicationDefinitionId: dbReplicationDefinitionId,
                filter: filter
            });
        }

        return ({
            GetDBReplicationDefinitionsInfo: GetDBReplicationDefinitionsInfo,
            GetDBDefinitionsInfo: GetDBDefinitionsInfo
        });
    }

    appControllers.service('VRCommon_DBReplicationDefinitionAPIService', DBReplicationDefinitionAPIService);
})(appControllers);