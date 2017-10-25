(function (appControllers) {

    "use strict";

    subViewEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function subViewEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var subViewEntity
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                subViewEntity = parameters.subViewEntity;
            };
            isEditMode = (subViewEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.save = function () {
                if (!isEditMode) {
                    addSubView();
                }
                else {
                    updateSubView();
                }
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Sub View');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Sub View');

            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildObjectFromScope() {
            var subView = {
                Name: $scope.scopeModel.subViewName
            };
            if (isEditMode) {
                subView.AccountViewDefinitionId = subViewEntity.AccountViewDefinitionId;

            }
            else {
                subView.AccountViewDefinitionId = UtilsService.guid();
            }
            return subView;
        }
        function loadStaticData() {
            if (subViewEntity != undefined) {
                $scope.scopeModel.subViewName = subViewEntity.Name;
            }
        }
        function addSubView() {
            var subView = buildObjectFromScope();
            if ($scope.onSubViewAdded != undefined) {
                $scope.onSubViewAdded(subView);
                $scope.modalContext.closeModal();
            }
        }
        function updateSubView() {
            var subView = buildObjectFromScope();
            if ($scope.onSubViewUpdated != undefined) {
                $scope.onSubViewUpdated(subView);
                $scope.modalContext.closeModal();
            }
        }
    }
    appControllers.controller("VR_AccountManager_SubViewEditorController", subViewEditorController);
})(appControllers);
