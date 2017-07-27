(function (appControllers) {

    "use strict";

    roomManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_RoomAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_RoomService', 'VRNavigationService'];

    function roomManagementController($scope, VRNotificationService, Demo_Module_RoomAPIService, UtilsService, VRUIUtilsService, Demo_Module_RoomService, VRNavigationService) {

        var gridAPI;
        var filter = {};

        var buildingDirectiveApi;
        var buildingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.rooms = [];
            $scope.searchClicked = function () {
                setfilterdobject()
                return gridAPI.loadGrid(filter);
            };

            $scope.onBuildingDirectiveReady = function (api) {
                buildingDirectiveApi = api;
                buildingReadyPromiseDeferred.resolve();
            };

            function setfilterdobject() {
                filter = {
                    Name: $scope.name,
                    BuildingIds: buildingDirectiveApi.getSelectedIds()
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };
            $scope.addNewRoom = addNewRoom;
        }
        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadBuildingSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadBuildingSelector() {
            var buildingLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            buildingReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(buildingDirectiveApi, undefined, buildingLoadPromiseDeferred);
                });
            return buildingLoadPromiseDeferred.promise;
        }


        function addNewRoom() {
            var onRoomAdded = function (room) {
                if (gridAPI != undefined)
                    gridAPI.onRoomAdded(room);
            };

            Demo_Module_RoomService.addRoom(onRoomAdded);
        }
    }




    appControllers.controller('Demo_Module_RoomManagementController', roomManagementController);
})(appControllers);