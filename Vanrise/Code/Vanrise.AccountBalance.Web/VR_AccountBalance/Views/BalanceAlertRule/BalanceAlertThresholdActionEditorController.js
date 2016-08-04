(function (appControllers) {

    'use strict';

    ThresholdActionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ThresholdActionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var thresholdEntity;
        var isEditMode;
        var balanceAlertThresholdAPI;
        var balanceAlertThresholdReadyDeferred = UtilsService.createPromiseDeferred();
        var extensionType = "VR_Notification_VRAction";
        var vRActionManagementAPI;
        var vRActionManagementReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                thresholdEntity = parameters.thresholdActionEntity;
                isEditMode = (thresholdEntity != undefined);
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onBalanceAlertThresholdDirectiveReady = function (api) {
                balanceAlertThresholdAPI = api;
                balanceAlertThresholdReadyDeferred.resolve();
            };

            $scope.scopeModel.onVRActionManagementDirectiveReady = function (api) {
                vRActionManagementAPI = api;
                vRActionManagementReadyDeferred.resolve();
            };

            $scope.scopeModel.saveThreshold = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRActionManagement, loadBalanceAlertThresholdDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor('Threshold') : UtilsService.buildTitleForAddEditor('Threshold');
        }

        function loadStaticData() {

        }

        function loadVRActionManagement() {
            var vRActionManagementLoadDeferred = UtilsService.createPromiseDeferred();
            vRActionManagementReadyDeferred.promise.then(function () {
                var vrActionPayload = { extensionType: extensionType, actions: thresholdEntity != undefined ? thresholdEntity.Actions : undefined };
                VRUIUtilsService.callDirectiveLoad(vRActionManagementAPI, vrActionPayload, vRActionManagementLoadDeferred);
            });
            return vRActionManagementLoadDeferred.promises;
        }

        function loadBalanceAlertThresholdDirective() {
            var balanceAlertThresholdLoadDeferred = UtilsService.createPromiseDeferred();
            balanceAlertThresholdReadyDeferred.promise.then(function () {
                var balanceAlertThresholdPayload = thresholdEntity != undefined ? { thresholdEntity: thresholdEntity.Threshold } : undefined
                VRUIUtilsService.callDirectiveLoad(balanceAlertThresholdAPI, balanceAlertThresholdPayload, balanceAlertThresholdLoadDeferred);
            });
            return balanceAlertThresholdLoadDeferred.promises;
        }

        function insert() {
            var balanceAlertThresholdObj = buildBalanceAlertThresholdObjFromScope();
            if ($scope.onBalanceAlertThresholdAdded != undefined)
                $scope.onBalanceAlertThresholdAdded(balanceAlertThresholdObj);
            $scope.modalContext.closeModal();
        }

        function update() {
            if ($scope.onBalanceAlertThresholdUpdated != undefined) {
                $scope.onBalanceAlertThresholdUpdated(buildBalanceAlertThresholdObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function buildBalanceAlertThresholdObjFromScope() {
            var obj = {
                Threshold: balanceAlertThresholdAPI.getData(),
                Actions: vRActionManagementAPI.getData()
            };
            return obj;
        }
    }

    appControllers.controller('VR_AccountBalance_ThresholdActionEditorController', ThresholdActionEditorController);

})(appControllers);