(function (appControllers) {

    'use strict';

    AccountActionDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountActionDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountActionDefinitionEntity;
       
        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountActionDefinitionEntity = parameters.accountActionDefinitionEntity;
            }
            isEditMode = (accountActionDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAccountActionDefinitionSettingsReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
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
                    UtilsService.buildTitleForUpdateEditor((accountActionDefinitionEntity != undefined) ? accountActionDefinitionEntity.Name : null, 'Account Action Definition') :
                    UtilsService.buildTitleForAddEditor('Account Action  Definition');
            }
            function loadStaticData() {
                if (accountActionDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = accountActionDefinitionEntity.Name;
            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                settingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload;
                    if (accountActionDefinitionEntity != undefined) {
                        settingsDirectivePayload = { accountActionDefinitionSettings: accountActionDefinitionEntity.ActionDefinitionSettings };
                    }
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
            function loadAccountConditionSelective() {
                var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                accountConditionSelectiveReadyDeferred.promise.then(function () {
                    var accountConditionSelectivePayload;
                    if (accountActionDefinitionEntity != undefined) {
                        accountConditionSelectivePayload = {
                            beFilter: accountActionDefinitionEntity.AvailabilityCondition
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
                });

                return accountConditionSelectiveLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective, loadAccountConditionSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var accountActionDefinitionObject = buildAccountActionDefinitionObjectFromScope();

            if ($scope.onAccountActionDefinitionAdded != undefined && typeof ($scope.onAccountActionDefinitionAdded) == 'function') {
                $scope.onAccountActionDefinitionAdded(accountActionDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var accountActionDefinitionObject = buildAccountActionDefinitionObjectFromScope();

            if ($scope.onAccountActionDefinitionUpdated != undefined && typeof ($scope.onAccountActionDefinitionUpdated) == 'function') {
                $scope.onAccountActionDefinitionUpdated(accountActionDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountActionDefinitionObjectFromScope() {
            return {
                AccountActionDefinitionId:accountActionDefinitionEntity != undefined?accountActionDefinitionEntity.AccountActionDefinitionId:UtilsService.guid(),
                Name: $scope.scopeModel.name,
                ActionDefinitionSettings: settingsDirectiveAPI.getData(),
                AvailabilityCondition: accountConditionSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_AccountActionDefinitionEditorController', AccountActionDefinitionEditorController);

})(appControllers);