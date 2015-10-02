app.service("PSTN_BE_Service", ["SwitchTrunkAPIService", "VRModalService", "VRNotificationService", function (SwitchTrunkAPIService, VRModalService, VRNotificationService) {
    
    return ({
        editSwitchTrunk: editSwitchTrunk,
        addSwitchTrunk: addSwitchTrunk,
        deleteSwitchTrunk: deleteSwitchTrunk
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
}]);
