(function (appControllers) {
    "use strict";
    roomAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function roomAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Room";

        function GetFilteredRooms(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredRooms"), input);
        }
        function GetRoomById(roomId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetRoomById"),
                {
                    roomId: roomId
                });
        }

        function UpdateRoom(room) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateRoom"), room);
        }
        function AddRoom(room) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddRoom"), room);
        };
        function GetRoomShapeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetRoomShapeConfigs"));
        }

        return {
            GetFilteredRooms: GetFilteredRooms,
            GetRoomById: GetRoomById,
            UpdateRoom: UpdateRoom,
            AddRoom: AddRoom,
            GetRoomShapeConfigs: GetRoomShapeConfigs
        };
    };
    appControllers.service("Demo_Module_RoomAPIService", roomAPIService);

})(appControllers);