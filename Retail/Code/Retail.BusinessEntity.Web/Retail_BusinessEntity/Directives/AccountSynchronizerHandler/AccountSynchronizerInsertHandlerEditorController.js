(function (appControllers) {

    'use strict';

    AccountSynchronizerInsertHandlerEditoController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountSynchronizerInsertHandlerEditoController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var accountBEDefinitionId;

        var accountSynchronizerHandlerEntity;

        var accountSynchronizerHandlerSettingsAPI;
        var accountSynchronizerHandlerSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountSynchronizerHandlerEntity = parameters.accountSynchronizerInsertHandler;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
            }

            isEditMode = (accountSynchronizerHandlerEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountSynchronizerHandlerSettingsReady = function (api) {
                accountSynchronizerHandlerSettingsAPI = api;
                accountSynchronizerHandlerSettingsReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountSynchronizerHandlerSettings]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((accountSynchronizerHandlerEntity != undefined) ? accountSynchronizerHandlerEntity.Name : null, 'Account Synchronizer Handler') :
                UtilsService.buildTitleForAddEditor('Account Synchronizer Handler');
        }
        function loadStaticData() {
            if (accountSynchronizerHandlerEntity == undefined)
                return;

            $scope.scopeModel.name = accountSynchronizerHandlerEntity.Name;
        }
        function loadAccountSynchronizerHandlerSettings() {
            var settingsLoadDeferred = UtilsService.createPromiseDeferred();

            accountSynchronizerHandlerSettingsReadyDeferred.promise.then(function () {

                var payload = {
                    accountBEDefinitionId: accountBEDefinitionId
                };
                if (accountSynchronizerHandlerEntity != undefined) {
                    payload.Settings = accountSynchronizerHandlerEntity.Settings;
                }
                VRUIUtilsService.callDirectiveLoad(accountSynchronizerHandlerSettingsAPI, payload, settingsLoadDeferred);
            });

            return settingsLoadDeferred.promise;
        }

        function insert() {
            var object = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onAccountSynchronizerHandlerAdded != undefined && typeof ($scope.onAccountSynchronizerHandlerAdded) == 'function') {
                $scope.onAccountSynchronizerHandlerAdded(object);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var object = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onAccountSynchronizerHandlerUpdated != undefined && typeof ($scope.onAccountSynchronizerHandlerUpdated) == 'function') {
                $scope.onAccountSynchronizerHandlerUpdated(object);
            }
            $scope.modalContext.closeModal();
        }

        function buildRecurringChargeRuleSetObjectFromScope() {

            return {
                Name: $scope.scopeModel.name,
                AccountSynchronizerInsertHandlerId: accountSynchronizerHandlerEntity != undefined ? accountSynchronizerHandlerEntity.AccountSynchronizerInsertHandlerId : UtilsService.guid(),
                Settings: accountSynchronizerHandlerSettingsAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_AccountSynchronizerInsertHandlerEditoController', AccountSynchronizerInsertHandlerEditoController);

})(appControllers);