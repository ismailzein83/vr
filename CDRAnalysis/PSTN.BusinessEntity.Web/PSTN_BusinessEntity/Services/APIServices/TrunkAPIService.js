(function (appControllers) {

    "use strict";
    trunkAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig', 'SecurityService'];

    function trunkAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig, SecurityService) {
        var controllerName = 'Trunk';

        function GetFilteredTrunks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetFilteredTrunks"), input);

        }

        function GetTrunkById(trunkId) {

            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetTrunkById"), {
                trunkId: trunkId
            });
        }
        function GetTrunksInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetTrunksInfo"), {
                serializedFilter: filter
            });
        }
        function GetTrunksBySwitchIds(trunkFilterObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetTrunksBySwitchIds"), trunkFilterObj);
        }

        function GetTrunks() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetTrunks"));
        }

        function AddTrunk(trunkObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "AddTrunk"), trunkObj);
        }

        function HasAddTrunkPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['AddTrunk']));
        }

        function UpdateTrunk(trunkObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "UpdateTrunk"), trunkObj);
        }

        function HasUpdateTrunkPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['UpdateTrunk']));
        }

        function DeleteTrunk(trunkId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "DeleteTrunk"), {
                trunkId: trunkId,
                linkedToTrunkId: linkedToTrunkId
            });
        }

        function HasDeleteTrunkPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['DeleteTrunk']));
        }


        return ({
            HasAddTrunkPermission: HasAddTrunkPermission,
            HasUpdateTrunkPermission: HasUpdateTrunkPermission,
            HasDeleteTrunkPermission: HasDeleteTrunkPermission,
            GetFilteredTrunks: GetFilteredTrunks,
            GetTrunkById: GetTrunkById,
            GetTrunksBySwitchIds: GetTrunksBySwitchIds,
            GetTrunks: GetTrunks,
            AddTrunk: AddTrunk,
            UpdateTrunk: UpdateTrunk,
            DeleteTrunk: DeleteTrunk,
            GetTrunksInfo: GetTrunksInfo
        });
    }

    appControllers.service('CDRAnalysis_PSTN_TrunkAPIService', trunkAPIService);

})(appControllers);