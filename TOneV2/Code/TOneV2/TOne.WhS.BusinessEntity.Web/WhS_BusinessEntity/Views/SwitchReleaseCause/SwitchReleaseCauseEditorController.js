(function (appControllers) {

    "use strict";

    switchReleaseCauseEditorController.$inject = ["$scope", "WhS_BE_SwitchReleaseCauseAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function switchReleaseCauseEditorController($scope, WhS_BE_SwitchReleaseCauseAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;

        var switchReleaseCauseId;
        var switchReleaseCauseEntity;

        var gridAPI;

        var switchSelectorAPI;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var switchConnectivityEntity;

        loadParameters();
        defineScope();
        load();
        
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                switchReleaseCauseId = parameters.switchReleaseCauseId;
            }
            isEditMode = (switchReleaseCauseId != undefined);
        }
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();

            };

            $scope.scopeModel.save = function () {
                if (isEditMode == false) {
                  return  addSwitchReleaseCause();
                }
                else {
                    return updateSwitchReleaseCause();
                }
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getSwitchReleaseCause().then(function () {
                    loadAllControls();
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (switchReleaseCauseEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(switchReleaseCauseEntity.ReleaseCode, 'Switch Release Cause');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Switch Release Cause');
            }

            function loadStaticData() {
                if (switchReleaseCauseEntity != undefined) {
                    $scope.scopeModel.releaseCode = switchReleaseCauseEntity.ReleaseCode;
                    $scope.scopeModel.description = (switchReleaseCauseEntity.Settings != undefined) ? switchReleaseCauseEntity.Settings.Description : undefined;
                    $scope.scopeModel.isDelivered = (switchReleaseCauseEntity.Settings != undefined) ? switchReleaseCauseEntity.Settings.IsDelivered : undefined;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSwitchSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);  
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getSwitchReleaseCause() {
            return WhS_BE_SwitchReleaseCauseAPIService.GetSwitchReleaseCause(switchReleaseCauseId).then(function (response) {
                switchReleaseCauseEntity = response;
            });
        }
        function buildObjectFromScope() {
            var switchReleaseCauseObject = {
                SwitchReleaseCauseId: switchReleaseCauseId,
                ReleaseCode: $scope.scopeModel.releaseCode,
                SwitchId: switchSelectorAPI.getSelectedIds(),
                Settings: {
                    Description: $scope.scopeModel.description,
                    IsDelivered: $scope.scopeModel.isDelivered
                }
            };
           
            return switchReleaseCauseObject;
        }
        function addSwitchReleaseCause() {
            $scope.scopeModel.isLoading = true;
            var switchReleaseCauseObject = buildObjectFromScope();
           return WhS_BE_SwitchReleaseCauseAPIService.AddSwitchReleaseCause(switchReleaseCauseObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch Release Cause", response)) {
                    if ($scope.onSwitchReleaseCauseAdded != undefined)
                        $scope.onSwitchReleaseCauseAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateSwitchReleaseCause() {
            $scope.scopeModel.isLoading = true;
            var switchReleaseCauseObject = buildObjectFromScope();
          return  WhS_BE_SwitchReleaseCauseAPIService.UpdateSwitchReleaseCause(switchReleaseCauseObject).then(function (response) {
              if (VRNotificationService.notifyOnItemUpdated("Switch Release Cause", response)) {
                    if ($scope.onSwitchReleaseCauseUpdated != undefined)
                    $scope.onSwitchReleaseCauseUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadSwitchSelector() {
            var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            switchSelectorReadyDeferred.promise.then(function () {
                var payload = (switchReleaseCauseEntity != undefined) ? { selectedIds: switchReleaseCauseEntity.SwitchId } : undefined;
                VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
            });
            return switchSelectorLoadDeferred.promise;
        }
       
    }

    appControllers.controller("WhS_BE_SwitchReleaseCauseEditorController", switchReleaseCauseEditorController);
})(appControllers);
