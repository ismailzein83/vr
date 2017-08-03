﻿app.service("PSTN_BE_Service", ["CDRAnalysis_PSTN_TrunkAPIService", "NormalizationRuleAPIService", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "VRModalService", "VRNotificationService", function (TrunkAPIService, NormalizationRuleAPIService, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, VRModalService, VRNotificationService) {
    
    return ({
        editTrunk: editTrunk,
        addTrunk: addTrunk,
        deleteTrunk: deleteTrunk,
        editNormalizationRule: editNormalizationRule,
        addNormalizationRule: addNormalizationRule,
        deleteNormalizationRule: deleteNormalizationRule
    });

    function editTrunk(trunkObj, onTrunkUpdated) {
        var modalSettings = {};

        var parameters = {
            TrunkId: trunkObj.Entity.TrunkId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Trunk", trunkObj.Name);
            modalScope.onTrunkUpdated = onTrunkUpdated;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, modalSettings);
    }

    function addTrunk(switchId, onTrunkAdded) {
        var modalSettings = {};

        var parameters = {
            SwitchId: switchId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForAddEditor("Trunk");
            modalScope.onTrunkAdded = onTrunkAdded;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, modalSettings);
    }

    function deleteTrunk(trunkObj, onTrunkDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    trunkObj.LinkedToTrunkId = (trunkObj.LinkedToTrunkId != null) ? trunkObj.LinkedToTrunkId : -1;

                    return TrunkAPIService.DeleteTrunk(trunkObj.Entity.TrunkId, trunkObj.LinkedToTrunkId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Trunk", deletionResponse))
                                onTrunkDeleted(trunkObj, trunkObj.LinkedToTrunkId);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function editNormalizationRule(normalizationRuleDetail, onNormalizationRuleUpdated) {
        var modalSettings = {};

        var parameters = {
            NormalizationRuleId: normalizationRuleDetail.Entity.RuleId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule", normalizationRuleDetail.Entity.Description);

            modalScope.onNormalizationRuleUpdated = onNormalizationRuleUpdated;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Normalization/NormalizationRuleEditor.html", parameters, modalSettings);
    }

    function addNormalizationRule(normalizationRuleTypeValue, onNormalizationRuleAdded) {
        var modalSettings = {};

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onNormalizationRuleAdded = onNormalizationRuleAdded;
        };

        var parameters = {
            NormalizationRuleTypeValue: normalizationRuleTypeValue
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Normalization/NormalizationRuleEditor.html", parameters, modalSettings);
    }

    function deleteNormalizationRule(ruleDetail, onNormalizationRuleDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return NormalizationRuleAPIService.DeleteRule(ruleDetail.Entity.RuleId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Normalization Rule", deletionResponse))
                                onNormalizationRuleDeleted(ruleDetail);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
