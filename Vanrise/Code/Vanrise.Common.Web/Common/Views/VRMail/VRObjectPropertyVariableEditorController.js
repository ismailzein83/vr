(function (appControllers) {

    'use strict';

    VRObjectPropertyVariableController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectPropertyVariableController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var objectPropertyVariableName;
        var objectPropertyVariableEntity;
        var objectPropertyVariables; // objectPropertyVariables are passed for validation

        var objects;

        var objectPropertySelectorAPI;
        var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                objectPropertyVariableName = parameters.objectPropertyVariableName;
                if (parameters.context != undefined)
                    objects = parameters.context.getObjectVariables();
            }

            isEditMode = (objectPropertyVariableName != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                objectPropertySelectorAPI = api;
                objectPropertySelectorReadyDeferred.resolve();
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
                setObjectPropertyVariableEntityFromParameters().then(function () {
                    loadAllControls()
                });
            }
            else {
                loadAllControls();
            }
        }

        function setObjectPropertyVariableEntityFromParameters() {
            objectPropertyVariableEntity = UtilsService.getItemByVal(objectPropertyVariables, objectPropertyVariableName, 'VariableName');

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
                    UtilsService.buildTitleForUpdateEditor((objectPropertyVariableEntity != undefined) ? objectPropertyVariableEntity.FieldName : null, 'Generic Rule Definition Criteria Field') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition Criteria Field');
            }
            function loadStaticData() {

                if (objectPropertyVariableEntity == undefined) 
                    return;
                
                $scope.scopeModel.variableName = objectPropertyVariableEntity.VariableName;
            }
            function loadObjectPropertySelector() {
                var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                objectPropertySelectorReadyDeferred.promise.then(function () {
                    var payload = {};

                    if (objects != undefined) {
                        payload.objectVariables = objects
                    }
                    if (objectPropertyVariableEntity != undefined) {
                        var objectProperty = { objectName: objectPropertyVariableEntity.ValueObjectName, propertyEvaluator: objectPropertyVariableEntity.ValueEvaluator };
                        payload.objectProperty = objectProperty;
                    }

                    VRUIUtilsService.callDirectiveLoad(objectPropertySelectorAPI, payload, objectPropertySelectorLoadDeferred);
                });

                return objectPropertySelectorLoadDeferred.promise;
            }
        }

        function insert() {
            var objectPropertyVariableObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onGenericRuleDefinitionCriteriaFieldAdded != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldAdded) == 'function') {
                $scope.onGenericRuleDefinitionCriteriaFieldAdded(objectPropertyVariableObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var objectPropertyVariableObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onGenericRuleDefinitionCriteriaFieldUpdated != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldUpdated) == 'function') {
                $scope.onGenericRuleDefinitionCriteriaFieldUpdated(objectPropertyVariableObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCriteriaFieldObjectFromScope() {

            var objectProperty = objectPropertySelectorAPI.getData();
            if (objectProperty != undefined) {
                var valueObjectName = objectProperty.objectName;
                var valueEvaluator = objectProperty.propertyEvaluator;
            }

            return {
                $type: "Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities",
                FieldName: $scope.scopeModel.fieldName,
                Title: $scope.scopeModel.fieldTitle,
                FieldType: dataRecordFieldTypeSelectiveAPI.getData(),
                RuleStructureBehaviorType: $scope.scopeModel.selectedBehaviorType.value,
                ShowInBasicSearch: $scope.scopeModel.showInBasicSearch,
                ValueObjectName: valueObjectName,
                ValueEvaluator: valueEvaluator
            };
        }
    }

    appControllers.controller('VRCommon_ObjectPropertyVariableController', VRObjectPropertyVariableController);

})(appControllers);