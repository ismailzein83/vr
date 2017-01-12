(function (appControllers) {

    'use strict';

    ColumnDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ColumnDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountBEDefinitionId;
        var columnDefinitionEntity;

        var accountGenericFieldDefinitionSelectorAPI;
        var accountGenericFieldDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                columnDefinitionEntity = parameters.columnDefinition;
            }
            isEditMode = (columnDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountGenericFieldDefinitionSelectorReady = function (api) {
                accountGenericFieldDefinitionSelectorAPI = api;
                accountGenericFieldDefinitionSelectorReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountGenericFieldDefinitionSelector, loadAccountConditionSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((columnDefinitionEntity != undefined) ? columnDefinitionEntity.FieldName : null, 'Column') :
                UtilsService.buildTitleForAddEditor('Column');
        }
        function loadStaticData() {

            if (columnDefinitionEntity == undefined)
                return;

            $scope.scopeModel.header = columnDefinitionEntity.Header;
            $scope.scopeModel.IsAvailableInRoot = columnDefinitionEntity.IsAvailableInRoot;
            $scope.scopeModel.IsAvailableInSubAccounts = columnDefinitionEntity.IsAvailableInSubAccounts;
        }
        function loadAccountGenericFieldDefinitionSelector() {
            var accountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountGenericFieldDefinitionSelectorReadyDeferred.promise.then(function () {

                var accountGenericFieldDefinitionSelectorPayload = {
                    accountBEDefinitionId: accountBEDefinitionId
                };
                if (columnDefinitionEntity != undefined) {
                    accountGenericFieldDefinitionSelectorPayload.genericFieldDefinition = { Name: columnDefinitionEntity.FieldName };
                };

                VRUIUtilsService.callDirectiveLoad(accountGenericFieldDefinitionSelectorAPI, accountGenericFieldDefinitionSelectorPayload, accountGenericFieldDefinitionSelectorLoadDeferred);
            });

            return accountGenericFieldDefinitionSelectorLoadDeferred.promise;
        }
        function loadAccountConditionSelective() {
            var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountConditionSelectiveReadyDeferred.promise.then(function () {

                var accountConditionSelectivePayload = {
                    accountBEDefinitionId: accountBEDefinitionId
                };
                if (columnDefinitionEntity != undefined) {
                    accountConditionSelectivePayload.beFilter = columnDefinitionEntity.SubAccountsAvailabilityCondition;
                }
                VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
            });

            return accountConditionSelectiveLoadDeferred.promise;
        }

        function insert() {
            var columnDefinitionObject = buildColumnDefinitionObjectFromScope();

            if ($scope.onColumnDefinitionAdded != undefined && typeof ($scope.onColumnDefinitionAdded) == 'function') {
                $scope.onColumnDefinitionAdded(columnDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var columnDefinitionObject = buildColumnDefinitionObjectFromScope();

            if ($scope.onColumnDefinitionUpdated != undefined && typeof ($scope.onColumnDefinitionUpdated) == 'function') {
                $scope.onColumnDefinitionUpdated(columnDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildColumnDefinitionObjectFromScope() {

            var accountGenericFieldDefinitionSelectorObj = accountGenericFieldDefinitionSelectorAPI.getData();

            return {
                FieldName: accountGenericFieldDefinitionSelectorObj != undefined ? accountGenericFieldDefinitionSelectorObj.Name : undefined,
                Header: $scope.scopeModel.header,
                IsAvailableInRoot: $scope.scopeModel.IsAvailableInRoot,
                IsAvailableInSubAccounts: $scope.scopeModel.IsAvailableInSubAccounts,
                SubAccountsAvailabilityCondition: $scope.scopeModel.IsAvailableInSubAccounts == true ? accountConditionSelectiveAPI.getData() : null
            };
        }
    }

    appControllers.controller('Retail_BE_GridColumnDefinitionEditorController', ColumnDefinitionEditorController);

})(appControllers);