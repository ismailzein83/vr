app.service("PSTN_BE_Service", ["TrunkAPIService", "NormalizationRuleAPIService", "VRModalService", "VRNotificationService", function (TrunkAPIService, NormalizationRuleAPIService, VRModalService, VRNotificationService) {
    
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
            TrunkId: trunkObj.TrunkId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = (trunkObj.Name != undefined) ? "Edit Trunk: " + trunkObj.Name : "Edit Trunk";
            modalScope.onTrunkUpdated = onTrunkUpdated;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, modalSettings);
    }

    function addTrunk(switchId, onTrunkAdded) {
        var modalSettings = {};

        var parameters = {
            SwitchId: switchId
        }

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Trunk";
            modalScope.onTrunkAdded = onTrunkAdded;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/TrunkEditor.html", parameters, modalSettings);
    }

    function deleteTrunk(trunkObj, onTrunkDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    trunkObj.LinkedToTrunkId = (trunkObj.LinkedToTrunkId != null) ? trunkObj.LinkedToTrunkId : -1;

                    return TrunkAPIService.DeleteTrunk(trunkObj.TrunkId, trunkObj.LinkedToTrunkId)
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

    function editNormalizationRule(normalizationRuleObj, onNormalizationRuleUpdated) {
        var modalSettings = {};

        var parameters = {
            NormalizationRuleId: normalizationRuleObj.NormalizationRuleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = (normalizationRuleObj.Name != undefined) ? "Edit Normalization Rule: " + normalizationRuleObj.Name : "Edit Normalization Rule";
            modalScope.onNormalizationRuleUpdated = onNormalizationRuleUpdated;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NormalizationRule/NormalizationRuleEditor.html", parameters, modalSettings);
    }

    function addNormalizationRule(onNormalizationRuleAdded) {
        var modalSettings = {};

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Normalization Rule";
            modalScope.onNormalizationRuleAdded = onNormalizationRuleAdded;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NormalizationRule/NormalizationRuleEditor.html", null, modalSettings);
    }

    function deleteNormalizationRule(normalizationRuleObj, onNormalizationRuleDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return NormalizationRuleAPIService.DeleteNormalizationRule(normalizationRuleObj.NormalizationRuleId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Normalization Rule", deletionResponse))
                                onNormalizationRuleDeleted(normalizationRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
