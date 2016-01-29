(function (appControllers) {

    "use strict";
    numberPrefixesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig', 'VRModalService'];

    function numberPrefixesAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig, VRModalService) {

        function ApplyCodePreparationForEntities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "ApplyCodePreparationForEntities"), input);
        }


        function GetZoneItems(sellingNumberPlanId, countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "GetZoneItems"), {
                sellingNumberPlanId: sellingNumberPlanId,
                countryId: countryId
            });
        }
        function CheckCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "CheckCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }
        function CancelCodePreparationState(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "CancelCodePreparationState"), {
                sellingNumberPlanId: sellingNumberPlanId
            });
        }
        function GetCodeItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "GetCodeItems"), input);
        }
        function DownloadImportCodePreparationTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "DownloadImportCodePreparationTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function SaveChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "SaveChanges"), input);
        }
        function MoveCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "MoveCodes"), input);
        }
        function CloseCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "CloseCodes"), input);
        }
        function SaveNewZone(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "SaveNewZone"), input);
        }
        function SaveNewCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "SaveNewCode"), input);
        }
        function GetChanges(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "CodePreparation", "GetChanges"), {
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

            VRModalService.showModal('/Client/Modules/Fzero_FraudAnalysis/Views/CodePreparationUploadEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/Fzero_FraudAnalysis/Views/CodePreparationApplyStateEditor.html', parameters, settings);
        }


        function GetPrefixesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "NumberPrefix", "GetPrefixesInfo"));
        }

        return ({
            GetPrefixesInfo: GetPrefixesInfo,
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
            UploadCodePreparationSheet: UploadCodePreparationSheet,
            ApplyCodePreparationState: ApplyCodePreparationState
        });
    }

    appControllers.service('FraudAnalysis_NumberPrefixAPIService', numberPrefixesAPIService);
})(appControllers);