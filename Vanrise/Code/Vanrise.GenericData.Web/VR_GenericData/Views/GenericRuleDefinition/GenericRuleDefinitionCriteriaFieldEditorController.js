(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var criteriaFieldName;
        var criteriaFieldEntity;
        var criteriaFields; // criteriaFields are passed for validation

        var objectVariables;

        var dataRecordFieldTypeSelectiveAPI;
        var dataRecordFieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var objectPropertySelectorAPI;
        var objectPropertySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                criteriaFieldName = parameters.GenericRuleDefinitionCriteriaFieldName;
                criteriaFields = UtilsService.cloneObject(parameters.GenericRuleDefinitionCriteriaFields, true);
                if (parameters.Context != undefined)
                    objectVariables = parameters.Context.getObjects();
            }

            isEditMode = (criteriaFieldName != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.behaviorTypes = UtilsService.getArrayEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum);

            $scope.scopeModel.onDataRecordFieldTypeSelectiveReady = function (api) {
                dataRecordFieldTypeSelectiveAPI = api;
                dataRecordFieldTypeSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onObjectPropertySelectorReady = function (api) {
                objectPropertySelectorAPI = api;
                objectPropertySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return updateCriteriaField();
                else
                    return insertCriteriaField();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateCriteriaField = function () {
                var fieldName = ($scope.scopeModel.fieldName != undefined) ? $scope.scopeModel.fieldName.toUpperCase() : null;

                if (isEditMode && (fieldName == criteriaFieldName))
                    return null;
                else if (UtilsService.getItemIndexByVal(criteriaFields, fieldName, 'FieldName') > -1) {
                    return 'Another criteria field with the same name already exists';
                }
                return null;
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                setCriteriaFieldEntityFromParameters().then(function () {
                    loadAllControls().finally(function () {
                        genericRuleDefinitionEntity = undefined;
                    });
                });
            }
            else {
                loadAllControls();
            }
        }

        function setCriteriaFieldEntityFromParameters() {
            criteriaFieldEntity = UtilsService.getItemByVal(criteriaFields, criteriaFieldName, 'FieldName');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordFieldTypeSelective, loadObjectPropertySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.scopeModel.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((criteriaFieldEntity != undefined) ? criteriaFieldEntity.FieldName : null, 'Generic Rule Definition Criteria Field') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition Criteria Field');
            }
            function loadStaticData() {

                //if (context != undefined)
                //    $scope.objectsVariable = context.getObjects();

                if (criteriaFieldEntity == undefined) {
                    return;
                }
                $scope.scopeModel.fieldName = criteriaFieldEntity.FieldName;
                $scope.scopeModel.fieldTitle = criteriaFieldEntity.Title;
                $scope.scopeModel.showInBasicSearch = criteriaFieldEntity.ShowInBasicSearch;

                $scope.scopeModel.selectedBehaviorType = UtilsService.getItemByVal($scope.scopeModel.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');

                prepareGlobalVarsForValidation();

                function prepareGlobalVarsForValidation() {
                    criteriaFieldName = criteriaFieldName.toUpperCase();
                    for (var i = 0; i < criteriaFields.length; i++) {
                        criteriaFields[i].FieldName = criteriaFields[i].FieldName.toUpperCase();
                    }
                }
            }
            function loadDataRecordFieldTypeSelective() {
                var dataRecordFieldTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordFieldTypeSelectiveReadyDeferred.promise.then(function () {
                    var payload;

                    if (criteriaFieldEntity != undefined) {
                        payload = criteriaFieldEntity.FieldType;
                    }

                    VRUIUtilsService.callDirectiveLoad(dataRecordFieldTypeSelectiveAPI, payload, dataRecordFieldTypeSelectiveLoadDeferred);
                });

                return dataRecordFieldTypeSelectiveLoadDeferred.promise;
            }
            function loadObjectPropertySelector() {
                var objectPropertySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                objectPropertySelectorReadyDeferred.promise.then(function () {
                    var payload = {};

                    if (objectVariables != undefined) {
                        payload.objectVariables = objectVariables                        
                    }
                    if (criteriaFieldEntity != undefined) {
                        payload.criteriaField = criteriaFieldEntity
                    }

                    VRUIUtilsService.callDirectiveLoad(objectPropertySelectorAPI, payload, objectPropertySelectorLoadDeferred);
                });

                return objectPropertySelectorLoadDeferred.promise;
            }
        }

        function insertCriteriaField() {
            var criteriaFieldObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onGenericRuleDefinitionCriteriaFieldAdded != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldAdded) == 'function') {
                $scope.onGenericRuleDefinitionCriteriaFieldAdded(criteriaFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function updateCriteriaField() {
            var criteriaFieldObject = buildCriteriaFieldObjectFromScope();
            if ($scope.onGenericRuleDefinitionCriteriaFieldUpdated != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldUpdated) == 'function') {
                $scope.onGenericRuleDefinitionCriteriaFieldUpdated(criteriaFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCriteriaFieldObjectFromScope() {

            if (objectPropertySelectorAPI != undefined)
                var objectPropertyData = objectPropertySelectorAPI.getData();

            return {
                $type: "Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities",
                FieldName: $scope.scopeModel.fieldName,
                Title: $scope.scopeModel.fieldTitle,
                FieldType: dataRecordFieldTypeSelectiveAPI.getData(),
                RuleStructureBehaviorType: $scope.scopeModel.selectedBehaviorType.value,
                ShowInBasicSearch: $scope.scopeModel.showInBasicSearch,
                ValueObjectName: objectPropertyData.ValueObjectName,
                ValueEvaluator: objectPropertyData.ValueEvaluator
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionCriteriaFieldEditorController', GenericRuleDefinitionCriteriaFieldEditorController);

})(appControllers);