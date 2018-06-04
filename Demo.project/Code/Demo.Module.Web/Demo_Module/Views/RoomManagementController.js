(function (appControllers) {
    "use strict";

    roomManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_RoomService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function roomManagementController($scope, VRNotificationService, Demo_Module_RoomService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

        var gridApi;
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };


            $scope.scopeModel.addRoom = function () {
                var onRoomAdded = function (room) {
                    if (gridApi != undefined) {
                        gridApi.onRoomAdded(room);
                    }
                };
                Demo_Module_RoomService.addRoom(onoomAdded);
            };
        };

        function load() {

        }


        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };

    };

    appControllers.controller('Demo_Module_RoomManagementController', roomManagementController);
})(appControllers);