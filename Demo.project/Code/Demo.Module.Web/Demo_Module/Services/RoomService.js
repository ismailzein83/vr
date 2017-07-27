app.service('Demo_Module_RoomService', ['VRModalService', 'Demo_Module_RoomAPIService', 'VRNotificationService',
function (VRModalService, Demo_Module_RoomAPIService, VRNotificationService) {
    var drillDownDefinitions = [];
    return ({
        editRoom: editRoom,
        addRoom: addRoom,
        deleteRoom: deleteRoom

    });
    function addRoom(onRoomAdded) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onRoomAdded = onRoomAdded;
        };
        var parameters = {};


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/RoomEditor.html', parameters, settings);
    }
    function editRoom(roomId, onRoomUpdated) {
        var settings = {
        };
        var parameters = {
            roomId: roomId
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onRoomUpdated = onRoomUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/RoomEditor.html', parameters, settings);
    }
    function deleteRoom(scope, dataItem, onRoomDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_RoomAPIService.DeleteRoom(dataItem.Entity.RoomId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('Room', responseObject);

                    if (deleted && onRoomDeleted && typeof onRoomDeleted == 'function') {
                        onRoomDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }
    
}]);