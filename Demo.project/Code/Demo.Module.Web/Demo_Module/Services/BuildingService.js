app.service('Demo_Module_BuildingService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addBuilding(onBuildingAdded) {

        var settings = {};
        var parameters = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onBuildingAdded = onBuildingAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BuildingEditor.html', parameters, settings);
    };

    function editBuilding(buildingId, onBuildingUpdated) {
        var settings = {};
        var parameters = {
            buildingId: buildingId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onBuildingUpdated = onBuildingUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BuildingEditor.html', parameters, settings);
    };

    return {
        addBuilding: addBuilding,
        editBuilding: editBuilding,
    };

}]);