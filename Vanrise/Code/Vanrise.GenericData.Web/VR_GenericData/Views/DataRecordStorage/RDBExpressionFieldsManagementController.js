(function (appControllers) {

    'use strict';

    DataRecordStorageRDBExpressionFieldsManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function DataRecordStorageRDBExpressionFieldsManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        var expressionFieldEntity;
        var context;
        var isEditMode;

        var fieldNameSelectorAPI;
        var fieldNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var expressionFieldSettingsSelectiveAPI;
        var expressionFieldSettingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                expressionFieldEntity = parameters.expressionFieldEntity;
                context = parameters.context;
            }

            isEditMode = (expressionFieldEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFieldNameSelectorReady = function (api) {
                fieldNameSelectorAPI = api;
                fieldNameSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onExpressionFieldSettingsSelectiveReady = function (api) {
                expressionFieldSettingsSelectiveAPI = api;
                expressionFieldSettingsSelectiveReadyDeferred.resolve();
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
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([setTitle, loadFieldNameSelector, loadExpressionFieldSelective]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((expressionFieldEntity != undefined) ? expressionFieldEntity.FieldName : null, 'Expression Field') :
                UtilsService.buildTitleForAddEditor('Expression Field');
        }

        function loadFieldNameSelector() {
            var fieldNameSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            fieldNameSelectorReadyDeferred.promise.then(function () {
                var fieldNameSelectorPayload = {
                    dataRecordTypeId: context.getMainDataRecordTypeId(),
                    selectedIds: expressionFieldEntity != undefined ? expressionFieldEntity.FieldName : undefined
                };
              
                VRUIUtilsService.callDirectiveLoad(fieldNameSelectorAPI, fieldNameSelectorPayload, fieldNameSelectorLoadDeferred);
            });
            return fieldNameSelectorLoadDeferred.promise;
        }

        function loadExpressionFieldSelective() {
            var expressionFieldSettingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            expressionFieldSettingsSelectiveReadyDeferred.promise.then(function () {
                var expressionFieldSettingsPayload = {
                    settings: expressionFieldEntity != undefined ? expressionFieldEntity.Settings : undefined,
                    context: context
                };
                VRUIUtilsService.callDirectiveLoad(expressionFieldSettingsSelectiveAPI, expressionFieldSettingsPayload, expressionFieldSettingsSelectiveLoadDeferred);
            });
            return expressionFieldSettingsSelectiveLoadDeferred.promise;
        }

        function insert() {
            if ($scope.onExpressionFieldAdded != undefined && typeof ($scope.onExpressionFieldAdded) == 'function') {
                $scope.onExpressionFieldAdded(buildExpressionFieldObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            if ($scope.onExpressionFieldUpdated != undefined && typeof ($scope.onExpressionFieldUpdated) == 'function') {
                $scope.onExpressionFieldUpdated(buildExpressionFieldObjFromScope());
            }
            $scope.modalContext.closeModal();
        }

        function buildExpressionFieldObjFromScope() {
            var expressionFieldObj = {
                FieldName: fieldNameSelectorAPI.getSelectedIds(),
                Settings: expressionFieldSettingsSelectiveAPI.getData()
            };
            return expressionFieldObj;
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageRDBExpressionFieldsManagementController', DataRecordStorageRDBExpressionFieldsManagementController);

})(appControllers);