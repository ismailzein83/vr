(function (appControllers) {

    'use strict';

    AccountBulkActionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountBulkActionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountBEDefinitionId;
        var accountBulkActionEntity;

        var accountbBulkActionSettingsSelectiveAPI;
        var accountbBulkActionSettingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountBulkActionEntity = parameters.accountBulkActionEntity;
            }
            isEditMode = (accountBulkActionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountBulkActionSettingsSelectiveReady = function (api) {
                accountbBulkActionSettingsSelectiveAPI = api;
                accountbBulkActionSettingsSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                accountConditionSelectiveAPI = api;
                accountConditionSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((accountBulkActionEntity != undefined) ? accountBulkActionEntity.Title : null, 'Bulk Action') :
                    UtilsService.buildTitleForAddEditor('Bulk Action');
            }
            function loadStaticData() {
                if (accountBulkActionEntity == undefined)
                    return;
                $scope.scopeModel.title = accountBulkActionEntity.Title;
            }
            function loadAccountBulkActionSettingsSelective() {
                var accountBulkActionSettingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                accountbBulkActionSettingsSelectiveReadyDeferred.promise.then(function () {

                    var settingsDirectivePayload = {
                        Settings: accountBulkActionEntity != undefined ? accountBulkActionEntity.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(accountbBulkActionSettingsSelectiveAPI, settingsDirectivePayload, accountBulkActionSettingsSelectiveLoadDeferred);
                });

                return accountBulkActionSettingsSelectiveLoadDeferred.promise;
            }
            function loadAccountConditionSelective() {
                var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                accountConditionSelectiveReadyDeferred.promise.then(function () {

                    var accountConditionSelectivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    if (accountBulkActionEntity != undefined) {
                        accountConditionSelectivePayload.beFilter = accountBulkActionEntity.AccountCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
                });

                return accountConditionSelectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountBulkActionSettingsSelective, loadAccountConditionSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var accountBulkActionObject = buildAccountBulkActionObjectFromScope();

            if ($scope.onAccountBulkActionAdded != undefined && typeof ($scope.onAccountBulkActionAdded) == 'function') {
                $scope.onAccountBulkActionAdded(accountBulkActionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var accountBulkActionObject = buildAccountBulkActionObjectFromScope();

            if ($scope.onAccountBulkActionUpdated != undefined && typeof ($scope.onAccountBulkActionUpdated) == 'function') {
                $scope.onAccountBulkActionUpdated(accountBulkActionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountBulkActionObjectFromScope() {
            return {
                AccountBulkActionId: accountBulkActionEntity != undefined ? accountBulkActionEntity.AccountBulkActionId : UtilsService.guid(),
                Title: $scope.scopeModel.title,
                Settings: accountbBulkActionSettingsSelectiveAPI.getData(),
                AccountCondition: accountConditionSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_AccountBulkActionEditorController', AccountBulkActionEditorController);

})(appControllers);