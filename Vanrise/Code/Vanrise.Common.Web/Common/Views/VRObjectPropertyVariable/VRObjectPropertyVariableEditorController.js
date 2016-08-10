(function (appControllers) {

    'use strict';

    VRObjectPropertyVariableController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectPropertyVariableController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var variableName;
        var variableEntity;
        var variables; // variables are passed for validation

        var objects;

        var objectPropertySelectorAPI;
        var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                variableName = parameters.variableName;
                variables = parameters.variables
                if (parameters.context != undefined)
                    objects = parameters.context.getObjectVariables();
            }

            isEditMode = (variableName != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                objectPropertySelectorAPI = api;
                objectPropertySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidateVariables = function () {

                if (variableEntity != undefined && variableEntity.VariableName == $scope.scopeModel.variableName)
                    return null;

                for (var i = 0; i < variables.length; i++) {
                    var variable = variables[i];
                    if ($scope.scopeModel.variableName.toLowerCase() == variable.VariableName.toLowerCase()) {
                        return 'Same Variable Name Exists';
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
                setVariableEntityFromParameters().then(function () {
                    loadAllControls()
                });
            }
            else {
                loadAllControls();
            }
        }

        function setVariableEntityFromParameters() {
            variableEntity = UtilsService.getItemByVal(variables, variableName, 'VariableName');

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
                    UtilsService.buildTitleForUpdateEditor((variableEntity != undefined) ? variableEntity.FieldName : null, 'Object Property Variable') :
                    UtilsService.buildTitleForAddEditor('Object Property Variable');
            }
            function loadStaticData() {

                if (variableEntity == undefined) 
                    return;
                
                $scope.scopeModel.variableName = variableEntity.VariableName;
                $scope.scopeModel.description = variableEntity.Description;
            }
            function loadObjectPropertySelector() {
                var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                objectPropertySelectorReadyDeferred.promise.then(function () {
                    var payload = {};

                    if (objects != undefined) {
                        payload.objectVariables = objects
                    }
                    if (variableEntity != undefined) {
                        var objectProperty = { objectName: variableEntity.ObjectName, propertyEvaluator: variableEntity.PropertyEvaluator };
                        payload.objectProperty = objectProperty;
                    }

                    VRUIUtilsService.callDirectiveLoad(objectPropertySelectorAPI, payload, objectPropertySelectorLoadDeferred);
                });

                return objectPropertySelectorLoadDeferred.promise;
            }
        }

        function insert() {
            var objectPropertyVariableObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onObjectPropertyVariableAdded != undefined && typeof ($scope.onObjectPropertyVariableAdded) == 'function') {
                $scope.onObjectPropertyVariableAdded(objectPropertyVariableObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var objectPropertyVariableObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onObjectPropertyVariableUpdated != undefined && typeof ($scope.onObjectPropertyVariableUpdated) == 'function') {
                $scope.onObjectPropertyVariableUpdated(objectPropertyVariableObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCriteriaFieldObjectFromScope() {

            var objectProperty = objectPropertySelectorAPI.getData();
            if (objectProperty != undefined) {
                var objectName = objectProperty.objectName;
                var propertyEvaluator = objectProperty.propertyEvaluator;
            }

            return {
                VariableName: $scope.scopeModel.variableName,
                Description: $scope.scopeModel.description,
                ObjectName: objectName,
                PropertyEvaluator: propertyEvaluator
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectPropertyVariableController', VRObjectPropertyVariableController);

})(appControllers);