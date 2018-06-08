(function (appControllers) {

    "use strict";
    roomEditorController.$inject = ['$scope', 'Demo_Module_RoomAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function roomEditorController($scope, Demo_Module_RoomAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var roomId;
        var roomEntity;

        var buildingIdItem;

        var buildingDirectiveApi;
        var buildingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var roomShapeDirectiveApi;
        var roomShapeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                roomId = parameters.roomId;
                buildingIdItem = parameters.buildingIdItem;

            }
            isEditMode = (roomId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.disableBuilding = buildingIdItem != undefined;
            $scope.scopeModel.onBuildingDirectiveReady = function (api) {
                buildingDirectiveApi = api;
                buildingReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRoomShapeDirectiveReady = function (api) {
                roomShapeDirectiveApi = api;
                roomShapeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveRoom = function () {
                if (isEditMode)
                    return updateRoom();
                else
                    return insertRoom();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getRoom().then(function () {
                    loadAllControls().finally(function () {
                        roomEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getRoom() {
            return Demo_Module_RoomAPIService.GetRoomById(roomId).then(function (response) {
                roomEntity = response;
            });
        };

        function loadAllControls() {

            function loadRoomShapeDirective() {
                var roomShapeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                roomShapeReadyPromiseDeferred.promise.then(function () {
                    var roomShapePayload;
                    if (roomEntity != undefined && roomEntity.Settings != undefined)
                        roomShapePayload = {
                            roomShapeEntity: roomEntity.Settings.RoomShape
                        };
                    VRUIUtilsService.callDirectiveLoad(roomShapeDirectiveApi, roomShapePayload, roomShapeLoadPromiseDeferred);
                });
                return roomShapeLoadPromiseDeferred.promise;
            }

            function loadBuildingSelector() {
                var buildingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                buildingReadyPromiseDeferred.promise.then(function () {
                    var buildingPayload = {};
                    if (buildingIdItem != undefined)
                        buildingPayload.selectedIds = buildingIdItem.BuildingId;

                    if (roomEntity != undefined)
                        buildingPayload.selectedIds = roomEntity.BuildingId;

                    VRUIUtilsService.callDirectiveLoad(buildingDirectiveApi, buildingPayload, buildingLoadPromiseDeferred);
                });
                return buildingLoadPromiseDeferred.promise;

            }

            function setTitle() {
                if (isEditMode && roomEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(roomEntity.Name, "Room");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Room");
            };

            function loadStaticData() {
                if (roomEntity != undefined)
                    $scope.scopeModel.name = roomEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBuildingSelector, loadRoomShapeDirective])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildRoomObjectFromScope() {
            var object = {
                RoomId: (roomId != undefined) ? roomId : undefined,
                Name: $scope.scopeModel.name,
                BuildingId: buildingDirectiveApi.getSelectedIds(),
                Settings: {
                    RoomShape: roomShapeDirectiveApi.getData()
                }
            };
            return object;
        };

        function insertRoom() {

            $scope.scopeModel.isLoading = true;
            var roomObject = buildRoomObjectFromScope();
            return Demo_Module_RoomAPIService.AddRoom(roomObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Room", response, "Name")) {
                    if ($scope.onRoomAdded != undefined) {
                        $scope.onRoomAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateRoom() {
            $scope.scopeModel.isLoading = true;
            var roomObject = buildRoomObjectFromScope();
            Demo_Module_RoomAPIService.UpdateRoom(roomObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Room", response, "Name")) {
                    if ($scope.onRoomUpdated != undefined) {
                        $scope.onRoomUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_RoomEditorController', roomEditorController);
})(appControllers);