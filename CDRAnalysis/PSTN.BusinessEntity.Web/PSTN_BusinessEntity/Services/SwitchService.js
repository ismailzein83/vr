app.service("SwitchService", ["SwitchTrunkAPIService", "VRModalService", "VRNotificationService", function (SwitchTrunkAPIService, VRModalService, VRNotificationService) {
    
    return ({
        editSwitchTrunk: editSwitchTrunk,
        addSwitchTrunk: addSwitchTrunk,
        deleteSwitchTrunk: deleteSwitchTrunk
    });

    function editSwitchTrunk(trunkObject, eventHandler) {
        var modalSettings = {};

        var parameters = {
            TrunkID: trunkObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = (trunkObject.Name != undefined) ? "Edit Switch Trunk: " + trunkObject.Name : "Edit Switch Trunk";
            modalScope.onTrunkUpdated = eventHandler;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTrunkEditor.html", parameters, modalSettings);
    }

    function addSwitchTrunk(switchID, eventHandler) {
        var modalSettings = {};

        var parameters = {
            SwitchID: switchID
        }

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Switch Trunk";
            modalScope.onTrunkAdded = eventHandler;
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTrunkEditor.html", parameters, modalSettings);
    }

    function deleteSwitchTrunk(trunkObject, eventHandler) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SwitchTrunkAPIService.DeleteSwitchTrunk(trunkObject.ID)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch Trunk", deletionResponse))
                                eventHandler(trunkObject);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
