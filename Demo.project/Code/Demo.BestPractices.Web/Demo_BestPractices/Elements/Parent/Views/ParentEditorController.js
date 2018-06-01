(function (appControllers) {

    "use strict";
    parentEditorController.$inject = ['$scope', 'Demo_BestPractices_ParentAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function parentEditorController($scope, Demo_BestPractices_ParentAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var parentId;
        var parentEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parentId = parameters.parentId;
            }
            isEditMode = (parentId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveParent = function () {
                if (isEditMode)
                    return updateParent();
                else
                    return insertParent();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getParent().then(function () {
                    loadAllControls().finally(function () {
                        parentEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getParent() {
            return Demo_BestPractices_ParentAPIService.GetParentById(parentId).then(function (response) {
                parentEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && parentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(parentEntity.Name, "Parent");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Parent");
            };

            function loadStaticData() {
                if (parentEntity != undefined)
                    $scope.scopeModel.name = parentEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildParentObjectFromScope() {
            var object = {
                ParentId: (parentId != undefined) ? parentId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        };

        function insertParent() {

            $scope.scopeModel.isLoading = true;
            var parentObject = buildParentObjectFromScope();
            return Demo_BestPractices_ParentAPIService.AddParent(parentObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Parent", response, "Name")) {
                    if ($scope.onParentAdded != undefined) {
                        $scope.onParentAdded(response.InsertedObject);
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

        function updateParent() {
            $scope.scopeModel.isLoading = true;
            var parentObject = buildParentObjectFromScope();
            Demo_BestPractices_ParentAPIService.UpdateParent(parentObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Parent", response, "Name")) {
                    if ($scope.onParentUpdated != undefined) {
                        $scope.onParentUpdated(response.UpdatedObject);
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
    appControllers.controller('Demo_BestPractices_ParentEditorController', parentEditorController);
})(appControllers);