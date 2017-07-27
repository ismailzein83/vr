(function (appControllers) {

    "use strict";

    roomEditorController.$inject = ['$scope', 'Demo_Module_RoomAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function roomEditorController($scope, Demo_Module_RoomAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var roomId;
        var roomEntity;

        var BuildingId;
        var BuildingDirectiveApi;
        var BuildingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                roomId = parameters.roomId;
            }
            isEditMode = (roomId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.saveRoom = function () {
                if (isEditMode)
                    return updateRoom();
                else
                    return insertRoom();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onBuildingDirectiveReady = function (api) {
                BuildingDirectiveApi = api;
                BuildingReadyPromiseDeferred.resolve();
            };

        }


        function load(){

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getRoom().then(function () {
                    loadAllControls()
                        .finally(function () {
                            roomEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getRoom() {
            return Demo_Module_RoomAPIService.GetRoomById(roomId).then(function (roomObject) {
                roomEntity = roomObject;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && roomEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(roomEntity.Name, "Room");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Room");
            }

            function loadStaticData() {
                if (roomEntity != undefined)
                    $scope.scopeModel.name = roomEntity.Name;
            }

            function loadBuildingSelector() {
                var BuildingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                BuildingReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = {
                        selectedIds: roomEntity != undefined ? roomEntity.BuildingId : undefined,
                    };

                    VRUIUtilsService.callDirectiveLoad(BuildingDirectiveApi, directivePayload, BuildingLoadPromiseDeferred);

                });
                return BuildingLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBuildingSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        

        function buildRoomObjFromScope() {
            var obj = {
                RoomId: (roomId != null) ? roomId : 0,
                Name: $scope.scopeModel.name,
                BuildingId: BuildingDirectiveApi.getSelectedIds()
            };
            return obj;
        }


        function insertRoom() {
            $scope.scopeModel.isLoading = true;

            var roomObject = buildRoomObjFromScope();
            return Demo_Module_RoomAPIService.AddRoom(roomObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Room", response, "Name")) {
                    if ($scope.onRoomAdded != undefined) {

                        $scope.onRoomAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }
        function updateRoom() {
            $scope.scopeModel.isLoading = true;

            var roomObject = buildRoomObjFromScope();
            Demo_Module_RoomAPIService.UpdateRoom(roomObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Room", response, "Name")) {
                    if ($scope.onRoomUpdated != undefined)
                        $scope.onRoomUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_RoomEditorController', roomEditorController);
})(appControllers);
