(function (appControllers) {

    "use strict";

    buildingEditorController.$inject = ['$scope', 'Demo_Module_BuildingAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function buildingEditorController($scope, Demo_Module_BuildingAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var editMode;
        var buildingId;
        var buildingEntity;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                buildingId = parameters.buildingId;
                context = parameters.context;
            }
            editMode = (buildingId != undefined);
        }

        function defineScope() {

            $scope.saveBuilding = function () {
                if (editMode)
                    return updateBuilding();
                else
                    return insertBuilding();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }


        function load() {

            $scope.isLoading = true;

            if (editMode) {
                getBuilding().then(function () {
                    loadAllControls()
                        .finally(function () {
                            buildingEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getBuilding() {
            return Demo_Module_BuildingAPIService.GetBuildingById(buildingId).then(function (buildingObject) {
                buildingEntity = buildingObject;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (editMode && buildingEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(buildingEntity.Name, "Building");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Building");
        }

        function loadStaticData() {
            if (buildingEntity != undefined)
                $scope.name = buildingEntity.Name;
        }

        


        function buildBuildingObjFromScope() {
            var obj = {
                BuildingId: (buildingId != null) ? buildingId : 0,
                Name: $scope.name,
            };
            return obj;
        }


        function insertBuilding() {
            $scope.isLoading = true;

            var buildingObject = buildBuildingObjFromScope();
            return Demo_Module_BuildingAPIService.AddBuilding(buildingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Building", response, "Name")) {
                    if ($scope.onBuildingAdded != undefined) {

                        $scope.onBuildingAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
        function updateBuilding() {
            $scope.isLoading = true;

            var buildingObject = buildBuildingObjFromScope();
            Demo_Module_BuildingAPIService.UpdateBuilding(buildingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Building", response, "Name")) {
                    if ($scope.onBuildingUpdated != undefined)
                        $scope.onBuildingUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }
    appControllers.controller('Demo_Module_BuildingEditorController', buildingEditorController);
})(appControllers);
