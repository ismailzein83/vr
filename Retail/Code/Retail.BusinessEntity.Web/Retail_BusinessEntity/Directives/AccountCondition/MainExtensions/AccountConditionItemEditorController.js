(function (appControllers) {

    'use strict';

    AccountConditionItemEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountConditionItemEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountBEDefinitionId;
        var accountConditionItemEntity;

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountConditionItemEntity = parameters.accountConditionItemEntity;
            }
            isEditMode = (accountConditionItemEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

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
                    UtilsService.buildTitleForUpdateEditor((accountConditionItemEntity != undefined) ? accountConditionItemEntity.Name : null, 'Account Condition') :
                    UtilsService.buildTitleForAddEditor('Account Condition');
            }
            function loadStaticData() {
                if (accountConditionItemEntity == undefined)
                    return;
                $scope.scopeModel.name = accountConditionItemEntity.Name;

            }
            function loadAccountConditionSelective() {
                var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                accountConditionSelectiveReadyDeferred.promise.then(function () {
                    var accountConditionSelectivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    if (accountConditionItemEntity != undefined) {
                        accountConditionSelectivePayload.beFilter = accountConditionItemEntity.AccountCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
                });

                return accountConditionSelectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountConditionSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var accountConditionItemObject = buildAccountConditionItemObjectFromScope();

            if ($scope.onAccountConditionItemAdded != undefined && typeof ($scope.onAccountConditionItemAdded) == 'function') {
                $scope.onAccountConditionItemAdded(accountConditionItemObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var accountConditionItemObject = buildAccountConditionItemObjectFromScope();

            if ($scope.onAccountConditionItemUpdated != undefined && typeof ($scope.onAccountConditionItemUpdated) == 'function') {
                $scope.onAccountConditionItemUpdated(accountConditionItemObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountConditionItemObjectFromScope() {
            return {
                Name: $scope.scopeModel.name,
                AccountCondition: accountConditionSelectiveAPI.getData(),
            };
        }
    }

    appControllers.controller('Retail_BE_AccountConditionItemEditorController', AccountConditionItemEditorController);

})(appControllers);