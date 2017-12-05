(function (appControllers) {

    "use strict";

    subViewEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function subViewEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var subViewEntity;
        var context;

        var subViewDefinitionSettingAPI;
        var subViewDefinitionSettingReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                subViewEntity = parameters.subViewEntity;
                context = parameters.context;
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
            $scope.scopeModel.onSubViewSelectorReady = function (api) {
                subViewDefinitionSettingAPI = api;
                subViewDefinitionSettingReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function loadStaticData() {
                if (subViewEntity != undefined) {
                    $scope.scopeModel.subViewName = subViewEntity.Name;
                }
            }
            function loadSubViewSelective() {
                var selectiveLoadDeferred = UtilsService.createPromiseDeferred();
                subViewDefinitionSettingReadyDeferred.promise.then(function () {
                    var payload = { context: getContext() };
                    if (subViewEntity != undefined) {
                        payload.subViewEntity = subViewEntity.Settings;
                    };
                    VRUIUtilsService.callDirectiveLoad(subViewDefinitionSettingAPI, payload, selectiveLoadDeferred);
                });
                return selectiveLoadDeferred.promise;
            }
            function setTitle() {
                if (!isEditMode)
                    $scope.title = UtilsService.buildTitleForAddEditor('Sub View');
                else
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Sub View');

            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSubViewSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildObjectFromScope() {
            var subView = {
                Name: $scope.scopeModel.subViewName,
                Settings: subViewDefinitionSettingAPI.getData()
            };
            if (isEditMode) {
                subView.AccountViewDefinitionId = subViewEntity.AccountViewDefinitionId;
            }
            else {
                subView.AccountViewDefinitionId = UtilsService.guid();
            }
            return subView;
        }
      
        function addSubView() {
            var subView = buildObjectFromScope();
            if ($scope.onSubViewAdded != undefined) {
                $scope.onSubViewAdded(subView);
                $scope.modalContext.closeModal();
            };
        }
        function updateSubView() {
            var subView = buildObjectFromScope();
            if ($scope.onSubViewUpdated != undefined) {
                $scope.onSubViewUpdated(subView);
                $scope.modalContext.closeModal();
            };
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }
    appControllers.controller("VR_AccountManager_AccountManagerSubViewDefinitionEditorController", subViewEditorController);
})(appControllers);
