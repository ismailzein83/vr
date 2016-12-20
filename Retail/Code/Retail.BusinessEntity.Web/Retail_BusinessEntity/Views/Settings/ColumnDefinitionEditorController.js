(function (appControllers) {

    'use strict';

    ColumnDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ColumnDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var propertyName;
        var propertyEntity;
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
                propertyName = parameters.propertyName;
                properties = parameters.properties;
                if (parameters.context != undefined)
                    objectType = parameters.context.getObjectType();
            }
            isEditMode = (propertyName != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onObjectPropertySelectiveReady = function (api) {
                objectPropertySelectiveAPI = api;
                objectPropertySelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidateProperties = function () {

                if (propertyEntity != undefined && propertyEntity.Name == $scope.scopeModel.propertyName)
                    return null;

                for (var i = 0; i < properties.length; i++) {
                    var property = properties[i];
                    if ($scope.scopeModel.propertyName.toLowerCase() == property.Name.toLowerCase()) {
                        return 'Same Property Name Exists';
                    }
                }
                return null;
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

            if (isEditMode) {
                setPropertyEntityFromParameters().then(function () {
                    loadAllControls()
                });
            }
            else {
                loadAllControls();
            }
        }

        function setPropertyEntityFromParameters() {
            propertyEntity = UtilsService.getItemByVal(properties, propertyName, 'Name');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectPropertySelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((propertyEntity != undefined) ? propertyEntity.Name : null, 'Object Type Property Definition') :
                    UtilsService.buildTitleForAddEditor('Object Type Property Definition');
            }
            function loadStaticData() {

                if (propertyEntity == undefined)
                    return;

                $scope.scopeModel.propertyName = propertyEntity.Name;
                $scope.scopeModel.description = propertyEntity.Description;
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

    appControllers.controller('Retail_BE_ColumnDefinitionEditorController', VRObjectTypePropertyDefinitionController);

})(appControllers);