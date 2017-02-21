(function (appControllers) {

    'use strict';

    ColumnDefinitionExportExcelEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ColumnDefinitionExportExcelEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountBEDefinitionId;
        var columnDefinitionEntity;

        var accountGenericFieldDefinitionExportExcelSelectorAPI;
        var accountGenericFieldDefinitionExportExcelSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
                accountGenericFieldDefinitionExportExcelSelectorAPI = api;
                accountGenericFieldDefinitionExportExcelSelectorReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountGenericFieldDefinitionSelector]).catch(function (error) {
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
        }
        function loadAccountGenericFieldDefinitionSelector() {
            var accountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountGenericFieldDefinitionExportExcelSelectorReadyDeferred.promise.then(function () {

                var accountGenericFieldDefinitionSelectorPayload = {
                    accountBEDefinitionId: accountBEDefinitionId
                };
                if (columnDefinitionEntity != undefined) {
                    accountGenericFieldDefinitionSelectorPayload.genericFieldDefinition = { Name: columnDefinitionEntity.FieldName };
                };

                VRUIUtilsService.callDirectiveLoad(accountGenericFieldDefinitionExportExcelSelectorAPI, accountGenericFieldDefinitionSelectorPayload, accountGenericFieldDefinitionSelectorLoadDeferred);
            });

            return accountGenericFieldDefinitionSelectorLoadDeferred.promise;
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

            var accountGenericFieldDefinitionExportExcelSelectorObj = accountGenericFieldDefinitionExportExcelSelectorAPI.getData();

            return {
                FieldName: accountGenericFieldDefinitionExportExcelSelectorObj != undefined ? accountGenericFieldDefinitionExportExcelSelectorObj.Name : undefined,
                Header: $scope.scopeModel.header
            };
        }
    }

    appControllers.controller('Retail_BE_GridColumnDefinitionExportExcelEditorController', ColumnDefinitionExportExcelEditorController);

})(appControllers);