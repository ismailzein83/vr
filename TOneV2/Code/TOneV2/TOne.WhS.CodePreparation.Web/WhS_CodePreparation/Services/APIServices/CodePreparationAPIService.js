(function (appControllers) {

    "use strict";
    codePreparationAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_CP_ModuleConfig", "VRModalService"];

    function codePreparationAPIService(BaseAPIService, UtilsService, WhS_CP_ModuleConfig, VRModalService) {

        var controllerName = "CodePreparation";

        function GetZoneItems(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "GetZoneItems"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }

        function CheckCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "CheckCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function CancelCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "CancelCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function GetCodeItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "GetCodeItems"), input);
        }

        function DownloadImportCodePreparationTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "DownloadImportCodePreparationTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "SaveChanges"), input);
        }

        function MoveCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "MoveCodes"), input);
        }

        function CloseCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "CloseCodes"), input);
        }

        function SaveNewZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "SaveNewZone"), input);
        }

        function SaveNewCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "SaveNewCode"), input);
        }

        function GetChanges(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "GetChanges"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }

        function CloseZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "CloseZone"), input);
        }

        function RenameZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "RenameZone"), input);
        }

        function GetCPEffectiveDateDayOffset() {
        	return BaseAPIService.get(UtilsService.getServiceURL(WhS_CP_ModuleConfig.moduleName, controllerName, "GetCPEffectiveDateDayOffset"));
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

            VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/CodePreparationUploadEditor.html", parameters, settings);
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

            VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/CodePreparationApplyStateEditor.html", parameters, settings);
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
            GetCPEffectiveDateDayOffset: GetCPEffectiveDateDayOffset
        });
    }

    appControllers.service("WhS_CP_CodePrepAPIService", codePreparationAPIService);
})(appControllers);