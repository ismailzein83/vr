(function (appControllers) {

    "use strict";
    codePreparationAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig", "VRModalService"];

    function codePreparationAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig, VRModalService) {

        var controllerName = "CodePreparation";

        function GetZoneItems(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetZoneItems"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }

        function CheckCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "CheckCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function CancelCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "CancelCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function GetCodeItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetCodeItems"), input);
        }

        function DownloadImportCodePreparationTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "DownloadImportCodePreparationTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "SaveChanges"), input);
        }

        function MoveCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "MoveCodes"), input);
        }

        function CloseCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "CloseCodes"), input);
        }

        function SaveNewZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "SaveNewZone"), input);
        }

        function SaveNewCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "SaveNewCode"), input);
        }

        function GetChanges(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetChanges"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function CloseZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "CloseZone"), input);
        }

        function RenameZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "RenameZone"), input);
        }

        function GetCPSettings() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetCPSettings"), {});
        }

        function UploadCodePreparationSheet(sellingNumberPlanId, onCodePreparationUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodePreparationUpdated = onCodePreparationUpdated;
            };
            var parameters = {
                SellingNumberPlanId: sellingNumberPlanId
            };

            VRModalService.showModal("/Client/Modules/VR_NumberingPlan/Views/NumberingPlanUploadEditor.html", parameters, settings);
        }

        function ApplyCodePreparationState(sellingNumberPlanId, onCodePreparationApplied) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodePreparationApplied = onCodePreparationApplied;
            };
            var parameters = {
                SellingNumberPlanId: sellingNumberPlanId
            };

            VRModalService.showModal("/Client/Modules/VR_NumberingPlan/Views/NumberingPlanApplyStateEditor.html", parameters, settings);
        }

        return ({
            DownloadImportCodePreparationTemplate: DownloadImportCodePreparationTemplate,
            GetChanges: GetChanges,
            SaveChanges: SaveChanges,
            SaveNewCode: SaveNewCode,
            SaveNewZone: SaveNewZone,
            GetZoneItems: GetZoneItems,
            GetCodeItems: GetCodeItems,
            MoveCodes: MoveCodes,
            CloseCodes: CloseCodes,
            CheckCodePreparationState: CheckCodePreparationState,
            CancelCodePreparationState: CancelCodePreparationState,
            UploadCodePreparationSheet: UploadCodePreparationSheet,
            ApplyCodePreparationState: ApplyCodePreparationState,
            CloseZone: CloseZone,
            RenameZone: RenameZone,
            GetCPSettings: GetCPSettings
        });
    }

    appControllers.service("Vr_NP_CodePrepAPIService", codePreparationAPIService);
})(appControllers);