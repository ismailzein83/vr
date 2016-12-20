(function (appControllers) {

    'use strict';

    ColumnDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ColumnDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var propertyName;
        var columnDefinitionEntity;
        var properties; // properties are passed for validation
        var objectType;

        //var objectPropertySelectiveAPI;
        //var objectPropertySelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                columnDefinitionEntity = parameters.columnDefinition;
            }
            isEditMode = (columnDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            //$scope.scopeModel.onObjectPropertySelectiveReady = function (api) {
            //    objectPropertySelectiveAPI = api;
            //    objectPropertySelectiveReadyDeferred.resolve();
            //};

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((columnDefinitionEntity != undefined) ? columnDefinitionEntity.FieldName : null, 'Column Definition') :
                    UtilsService.buildTitleForAddEditor('Column Definition');
            }
            function loadStaticData() {

                if (columnDefinitionEntity == undefined)
                    return;

                $scope.scopeModel.fieldName = columnDefinitionEntity.FieldName;
                $scope.scopeModel.header = columnDefinitionEntity.Header;
            }
            function loadObjectPropertySelective() {
                var objectPropertySelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                objectPropertySelectiveReadyDeferred.promise.then(function () {
                    var payload = {};

                    if (objectType != undefined) {
                        payload.objectType = objectType;
                    }
                    if (propertyEntity != undefined) {
                        payload.objectPropertyEvaluator = propertyEntity.PropertyEvaluator;
                    }

                    VRUIUtilsService.callDirectiveLoad(objectPropertySelectiveAPI, payload, objectPropertySelectiveLoadDeferred);
                });

                return objectPropertySelectiveLoadDeferred.promise;
            }
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

            //var propertyEvaluator = objectPropertySelectiveAPI.getData();

            return {
                FieldName: $scope.scopeModel.fieldName,
                Header: $scope.scopeModel.header
                //PropertyEvaluator: propertyEvaluator
            };
        }
    }

    appControllers.controller('Retail_BE_ColumnDefinitionEditorController', ColumnDefinitionEditorController);

})(appControllers);