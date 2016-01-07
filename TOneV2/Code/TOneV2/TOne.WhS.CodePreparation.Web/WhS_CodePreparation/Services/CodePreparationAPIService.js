﻿(function (appControllers) {

    "use strict";
    codePreparationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CodePrep_ModuleConfig', 'VRModalService'];

    function codePreparationAPIService(BaseAPIService, UtilsService, WhS_CodePrep_ModuleConfig, VRModalService) {

        function ApplyCodePreparationForEntities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "ApplyCodePreparationForEntities"), input);
        }


        function GetZoneItems(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "GetZoneItems"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }
        function CheckCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "CheckCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }
        function CancelCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "CancelCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }
        function GetCodeItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "GetCodeItems"), input);
        }
        function DownloadImportCodePreparationTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "DownloadImportCodePreparationTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "SaveChanges"), input);
        }
        function MoveCodes(input) {
            return BaseAPISer
            vice.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "MoveCodes"), input);
        }
        function CloseCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "CloseCodes"), input);
        }
        function SaveNewZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "SaveNewZone"), input);
        }
        function SaveNewCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "SaveNewCode"), input);
        }
        function GetChanges(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CodePrep_ModuleConfig.moduleName, "CodePreparation", "GetChanges"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
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

            VRModalService.showModal('/Client/Modules/WhS_CodePreparation/Views/CodePreparationUploadEditor.html', parameters, settings);
        }

        return ({
            ApplyCodePreparationForEntities: ApplyCodePreparationForEntities,
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
            UploadCodePreparationSheet: UploadCodePreparationSheet
        });
    }

    appControllers.service('WhS_CodePrep_CodePrepAPIService', codePreparationAPIService);
})(appControllers);