app.service("SwitchService", ["VRModalService", function (VRModalService) {
    
    return ({
        editSwitchTrunk: editSwitchTrunk
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
}]);
