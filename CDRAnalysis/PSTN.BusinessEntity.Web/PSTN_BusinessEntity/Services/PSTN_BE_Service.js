app.service("PSTN_BE_Service", ["SwitchTrunkAPIService", "NormalizationRuleAPIService", "VRModalService", "VRNotificationService", function (SwitchTrunkAPIService, NormalizationRuleAPIService, VRModalService, VRNotificationService) {
    
    return ({
        editSwitchTrunk: editSwitchTrunk,
        addSwitchTrunk: addSwitchTrunk,
        deleteSwitchTrunk: deleteSwitchTrunk,
        editNormalizationRule: editNormalizationRule,
        addNormalizationRule: addNormalizationRule,
        deleteNormalizationRule: deleteNormalizationRule
    });

    function editSwitchTrunk(trunkObject, onTrunkUpdated) {
        var modalSettings = {};

        var parameters = {
            TrunkID: trunkObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = (trunkObject.Name != undefined) ? "Edit Switch Trunk: " + trunkObject.Name : "Edit Switch Trunk";
            modalScope.onTrunkUpdated = onTrunkUpdated;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Trunk/SwitchTrunkEditor.html", parameters, modalSettings);
    }

    function addSwitchTrunk(switchID, onTrunkAdded) {
        var modalSettings = {};

        var parameters = {
            SwitchID: switchID
        }

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Switch Trunk";
            modalScope.onTrunkAdded = onTrunkAdded;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Trunk/SwitchTrunkEditor.html", parameters, modalSettings);
    }

    function deleteSwitchTrunk(trunkObject, onTrunkDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SwitchTrunkAPIService.DeleteSwitchTrunk(trunkObject.ID, trunkObject.LinkedToTrunkID)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch Trunk", deletionResponse))
                                onTrunkDeleted(trunkObject, trunkObject.LinkedToTrunkID);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function editNormalizationRule(normalizationRuleObject, onNormalizationRuleUpdated) {
        var modalSettings = {};

        var parameters = {
            NormalizationRuleId: normalizationRuleObject.NormalizationRuleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = (normalizationRuleObject.Name != undefined) ? "Edit Normalization Rule: " + normalizationRuleObject.Name : "Edit Normalization Rule";
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

    function deleteNormalizationRule(normalizationRuleObject, onNormalizationRuleDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return NormalizationRuleAPIService.DeleteNormalizationRule(normalizationRuleObject.NormalizationRuleId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Normalization Rule", deletionResponse))
                                onNormalizationRuleDeleted(normalizationRuleObject);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
