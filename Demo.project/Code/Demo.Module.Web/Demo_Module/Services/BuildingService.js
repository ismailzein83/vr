app.service('Demo_Module_BuildingService', ['VRModalService', 'Demo_Module_BuildingAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_BuildingAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editBuilding: editBuilding,
        addBuilding: addBuilding,
        deleteBuilding: deleteBuilding

    });
    function addBuilding(onBuildingAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onBuildingAdded = onBuildingAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BuildingEditor.html', parameters, settings);
    }
    function editBuilding(buildingId, onBuildingUpdated) {
        var settings = {
        };
        var parameters = {
            buildingId: buildingId
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onBuildingUpdated = onBuildingUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/BuildingEditor.html', parameters, settings);
    }
    function deleteBuilding(scope, dataItem, onBuildingDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_BuildingAPIService.DeleteBuilding(dataItem.Entity.BuildingId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('Building', responseObject);

                    if (deleted && onBuildingDeleted && typeof onBuildingDeleted == 'function') {
                        onBuildingDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }

}]);