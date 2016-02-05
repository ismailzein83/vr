﻿(function (appControllers) {

    "use strict";
    trunkAPIService.$inject = ['BaseAPIService', 'UtilsService', 'PSTN_BE_ModuleConfig'];

    function trunkAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {


        function GetFilteredTrunks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "GetFilteredTrunks"), input);

        }

        function GetTrunkById(trunkId) {

            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "GetTrunkById"), {
                trunkId: trunkId
            });
        }
        function GetTrunksInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "GetTrunksInfo"), {
                serializedFilter:filter
            });
        }
        function GetTrunksBySwitchIds(trunkFilterObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "GetTrunksBySwitchIds"), trunkFilterObj);
        }

        function GetTrunks() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "GetTrunks"));
        }

        function AddTrunk(trunkObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "AddTrunk"), trunkObj);
        }

        function UpdateTrunk(trunkObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "UpdateTrunk"), trunkObj);
        }

        function DeleteTrunk(trunkId, linkedToTrunkId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "Trunk", "DeleteTrunk"), {
                trunkId: trunkId,
                linkedToTrunkId: linkedToTrunkId
            });
        }
        
        return ({
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