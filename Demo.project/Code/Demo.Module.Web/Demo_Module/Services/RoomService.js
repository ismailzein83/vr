app.service('Demo_Module_RoomService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addRoom(onRoomAdded, buildingIdItem) {

        var settings = {};
        var parameters = {
            buildingIdItem: buildingIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onRoomAdded = onRoomAdded;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/RoomEditor.html', parameters, settings);
    };

    function editRoom(roomId, onRoomUpdated, buildingIdItem) {
        var settings = {};
        var parameters = {
            roomId: roomId,
            buildingIdItem: buildingIdItem
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onRoomUpdated = onRoomUpdated;

        };
        VRModalService.showModal('/Client/Modules/Demo_Module/Views/RoomEditor.html', parameters, settings);
    };

    return {
        addRoom: addRoom,
        editRoom: editRoom,
    };

}]);