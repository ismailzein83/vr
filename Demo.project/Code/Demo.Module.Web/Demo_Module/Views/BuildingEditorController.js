(function (appControllers) {

    "use strict";
    buildingEditorController.$inject = ['$scope', 'Demo_Module_BuildingAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function buildingEditorController($scope, Demo_BestPractices_BuildingAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var buildingId;
        var buildingEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                buildingId = parameters.buildingId;
            }
            isEditMode = (buildingId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveBuilding = function () {
                if (isEditMode)
                    return updateBuilding();
                else
                    return insertBuilding();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getBuilding().then(function () {
                    loadAllControls().finally(function () {
                        buildingEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getBuilding() {
            return Demo_BestPractices_BuildingAPIService.GetBuildingById(buildingId).then(function (response) {
                buildingEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && buildingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(buildingEntity.Name, "Building");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Building");
            };

            function loadStaticData() {
                if (buildingEntity != undefined)
                    $scope.scopeModel.name = buildingEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildBuildingObjectFromScope() {
            var object = {
                BuildingId: (buildingId != undefined) ? buildingId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        };

        function insertBuilding() {

            $scope.scopeModel.isLoading = true;
            var buildingObject = buildBuildingObjectFromScope();
            return Demo_BestPractices_BuildingAPIService.AddBuilding(buildingObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Building", response, "Name")) {
                    if ($scope.onBuildingAdded != undefined) {
                        $scope.onBuildingAdded(response.InsertedObject);
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

        function updateBuilding() {
            $scope.scopeModel.isLoading = true;
            var buildingObject = buildBuildingObjectFromScope();
            Demo_BestPractices_BuildingAPIService.UpdateBuilding(buildingObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Building", response, "Name")) {
                    if ($scope.onBuildingUpdated != undefined) {
                        $scope.onBuildingUpdated(response.UpdatedObject);
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
    appControllers.controller('Demo_Module_BuildingEditorController', buildingEditorController);
})(appControllers);