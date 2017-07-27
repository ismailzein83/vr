(function (appControllers) {

    'use strict';

    RoomAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function RoomAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'Room';

        function GetRoomById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetRoomById'),
                { roomId: Id }
                );
        }

        function GetFilteredRooms(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredRooms"), input);
        }

        function GetRoomsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetRoomsInfo"),{
            filter: filter
        });
    }
        function AddRoom(room) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddRoom"), room);
        }
        function UpdateRoom(room) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateRoom"), room);
        }
        function DeleteRoom(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteRoom'), {
                roomId: Id
            });
        }

        return ({
            GetFilteredRooms: GetFilteredRooms,
            UpdateRoom: UpdateRoom,
            AddRoom: AddRoom,
            GetRoomById: GetRoomById,
            DeleteRoom: DeleteRoom,
            GetRoomsInfo:GetRoomsInfo
        });
    }


    appControllers.service('Demo_Module_RoomAPIService', RoomAPIService);
})(appControllers);