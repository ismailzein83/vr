(function (appControllers) {

    'use strict';

    VRObjectTypePropertyDefinitionController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectTypePropertyDefinitionController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var propertyName;
        var propertyEntity;
        var properties; // properties are passed for validation
        var objectType;

        var objectPropertySelectiveAPI;
        var objectPropertySelectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
            }

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectPropertySelector]).catch(function (error) {
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
            function loadObjectPropertySelector() {
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
            var objectTypePropertyDefinitionObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onObjectTypePropertyDefinitionAdded != undefined && typeof ($scope.onObjectTypePropertyDefinitionAdded) == 'function') {
                $scope.onObjectTypePropertyDefinitionAdded(objectTypePropertyDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var objectTypePropertyDefinitionObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onObjectTypePropertyDefinitionUpdated != undefined && typeof ($scope.onObjectTypePropertyDefinitionUpdated) == 'function') {
                $scope.onObjectTypePropertyDefinitionUpdated(objectTypePropertyDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCriteriaFieldObjectFromScope() {

            var propertyEvaluator = objectPropertySelectiveAPI.getData();

            return {
                Name: $scope.scopeModel.propertyName,
                Description: $scope.scopeModel.description,
                PropertyEvaluator: propertyEvaluator
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectTypePropertyDefinitionController', VRObjectTypePropertyDefinitionController);

})(appControllers);