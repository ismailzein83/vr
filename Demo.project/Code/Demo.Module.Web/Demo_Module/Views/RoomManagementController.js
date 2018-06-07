(function (appControllers) {
    "use strict";

    roomManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_RoomService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];

    function roomManagementController($scope, VRNotificationService, Demo_Module_RoomService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {

        var gridApi;
        var buildingDirectiveApi;
        var buildingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };
            $scope.scopeModel.onBuildingDirectiveReady = function (api) {
                buildingDirectiveApi = api;
                buildingReadyPromiseDeferred.resolve();
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
                Demo_Module_RoomService.addRoom(onRoomAdded);
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadBuildingSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadBuildingSelector() {
            var buildingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            buildingReadyPromiseDeferred.promise.then(function () {
                var directivePayload = undefined;
                VRUIUtilsService.callDirectiveLoad(buildingDirectiveApi, directivePayload, buildingLoadPromiseDeferred);
            });
            return buildingLoadPromiseDeferred.promise;
        }

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    BuildingIds: buildingDirectiveApi.getSelectedIds()
                }
            };
        };

    };

    appControllers.controller('Demo_Module_RoomManagementController', roomManagementController);
})(appControllers);