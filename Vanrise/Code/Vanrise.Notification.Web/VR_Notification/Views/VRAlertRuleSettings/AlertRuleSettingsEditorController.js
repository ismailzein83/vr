(function (appControllers) {

    'use strict';

    AlertRuleSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AlertRuleSettingsEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var thresholdEntity;
        var isEditMode;
        var balanceAlertThresholdAPI;
        var balanceAlertThresholdReadyDeferred = UtilsService.createPromiseDeferred();
        var balanceAlertActionExtensionType;
        var thresholdActionExtensionType;
        var vRActionManagementAPI;
        var vRActionManagementReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        var rollBackVRActionManagementAPI;
        var rollBackVRActionManagementReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                thresholdEntity = parameters.thresholdActionEntity;
                balanceAlertActionExtensionType = parameters.actionExtensionType;
                thresholdActionExtensionType = parameters.thresholdExtensionType;
                context = parameters.context;
                isEditMode = (thresholdEntity != undefined);
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onVRBalanceAlertRuleThresholdDirectiveReady = function (api) {
                balanceAlertThresholdAPI = api;
                balanceAlertThresholdReadyDeferred.resolve();
            };

            $scope.scopeModel.onVRActionManagementDirectiveReady = function (api) {
                vRActionManagementAPI = api;
                vRActionManagementReadyDeferred.resolve();
            };

            $scope.scopeModel.onRollBackVRActionManagementDirectiveReady = function (api) {
                rollBackVRActionManagementAPI = api;
                rollBackVRActionManagementReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRActionManagement, loadBalanceAlertThresholdDirective, loadRollBackVRActionManagement]).catch(function (error) {
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
                var vrActionPayload = {context:getContext(), extensionType: balanceAlertActionExtensionType, actions: thresholdEntity != undefined ? thresholdEntity.Actions : undefined, isRequired: true };
                VRUIUtilsService.callDirectiveLoad(vRActionManagementAPI, vrActionPayload, vRActionManagementLoadDeferred);
            });
            return vRActionManagementLoadDeferred.promises;
        }

        function loadRollBackVRActionManagement() {
            var rollBackVRActionManagementLoadDeferred = UtilsService.createPromiseDeferred();
            rollBackVRActionManagementReadyDeferred.promise.then(function () {
                var vrActionPayload = { context: getContext(), extensionType: balanceAlertActionExtensionType, actions: thresholdEntity != undefined ? thresholdEntity.RollbackActions : undefined, isRequired: false };
                VRUIUtilsService.callDirectiveLoad(rollBackVRActionManagementAPI, vrActionPayload, rollBackVRActionManagementLoadDeferred);
            });
            return rollBackVRActionManagementLoadDeferred.promises;
        }

        function loadBalanceAlertThresholdDirective() {
            var balanceAlertThresholdLoadDeferred = UtilsService.createPromiseDeferred();
            balanceAlertThresholdReadyDeferred.promise.then(function () {
                var balanceAlertThresholdPayload = { context: getContext(), thresholdEntity: thresholdEntity != undefined ? thresholdEntity.Threshold : undefined, extensionType: thresholdActionExtensionType };
                VRUIUtilsService.callDirectiveLoad(balanceAlertThresholdAPI, balanceAlertThresholdPayload, balanceAlertThresholdLoadDeferred);
            });
            return balanceAlertThresholdLoadDeferred.promises;
        }

        function insert() {
            var balanceAlertThresholdObj = buildBalanceAlertThresholdObjFromScope();
            if ($scope.onAlertRuleSettingsAdded != undefined)
                $scope.onAlertRuleSettingsAdded(balanceAlertThresholdObj);
            $scope.modalContext.closeModal();
        }

        function update() {
            if ($scope.onAlertRuleSettingsUpdated != undefined) {
                $scope.onAlertRuleSettingsUpdated(buildBalanceAlertThresholdObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function buildBalanceAlertThresholdObjFromScope() {
            var threshold = balanceAlertThresholdAPI.getData();
            var obj = {
                Threshold: threshold,
                Actions: vRActionManagementAPI.getData(),
                RollbackActions: rollBackVRActionManagementAPI.getData(),
                ThresholdDescription: threshold.ThresholdDescription
            };
            return obj;
        }
        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_Notification_AlertRuleSettingsEditorController', AlertRuleSettingsEditorController);

})(appControllers);