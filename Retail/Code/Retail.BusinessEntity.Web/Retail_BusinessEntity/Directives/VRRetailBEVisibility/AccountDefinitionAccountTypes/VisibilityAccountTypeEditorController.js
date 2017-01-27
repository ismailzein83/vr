(function (appControllers) {

    'use strict';

    VisibilityAccountTypeController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VisibilityAccountTypeController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var visibilityAccountTypeEntity;
        var accountBEDefinitionId;

        var accountTypeSelectorAPI;
        var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                visibilityAccountTypeEntity = parameters.visibilityAccountType;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
            }

            isEditMode = (visibilityAccountTypeEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((visibilityAccountTypeEntity != undefined) ? visibilityAccountTypeEntity.Title : null, 'Account Type') :
                UtilsService.buildTitleForAddEditor('Account Type');
        }
        function loadStaticData() {
            if (visibilityAccountTypeEntity == undefined)
                return;

            $scope.scopeModel.title = visibilityAccountTypeEntity.Title;
        }
        function loadAccountTypeSelector() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountTypeSelectorPromiseDeferred.promise.then(function () {

                var selectorPayload = {
                    filter: {
                        IncludeHiddenAccountTypes: true,
                        AccountBEDefinitionId: accountBEDefinitionId
                    },
                    selectedIds: visibilityAccountTypeEntity != undefined ? visibilityAccountTypeEntity.AccountTypeId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, selectorPayload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }

        function insert() {
            var visibilityAccountTypeObject = buildVisibilityAccountTypeFromScope();
            if ($scope.onVisibilityAccountTypeAdded != undefined && typeof ($scope.onVisibilityAccountTypeAdded) == 'function') {
                $scope.onVisibilityAccountTypeAdded(visibilityAccountTypeObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var visibilityAccountTypeObject = buildVisibilityAccountTypeFromScope();
            if ($scope.onVisibilityAccountTypeUpdated != undefined && typeof ($scope.onVisibilityAccountTypeUpdated) == 'function') {
                $scope.onVisibilityAccountTypeUpdated(visibilityAccountTypeObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildVisibilityAccountTypeFromScope() {

            var selectedAccountType = $scope.scopeModel.selectedAccountType;

            return {
                Title: $scope.scopeModel.title,
                AccountTypeId: selectedAccountType != undefined ? selectedAccountType.AccountTypeId : undefined,
                AccountTypeTitle: selectedAccountType != undefined ? selectedAccountType.Title : undefined
            };
        }
    }

    appControllers.controller('Retail_BE_VisibilityAccountTypeController', VisibilityAccountTypeController);

})(appControllers);