(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var criteriaFieldName;
        var criteriaFieldEntity;
        var criteriaFields; // criteriaFields are passed for validation

        var dataRecordFieldTypeSelectiveAPI;
        var dataRecordFieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                criteriaFieldName = parameters.GenericRuleDefinitionCriteriaFieldName;
                criteriaFields = UtilsService.cloneObject(parameters.GenericRuleDefinitionCriteriaFields, true);
            }

            isEditMode = (criteriaFieldName != undefined);
        }

        function defineScope() {
            $scope.behaviorTypes = UtilsService.getArrayEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum);

            $scope.onDataRecordFieldTypeSelectiveReady = function (api) {
                dataRecordFieldTypeSelectiveAPI = api;
                dataRecordFieldTypeSelectiveReadyDeferred.resolve();
            };

            $scope.save = function () {
                if (isEditMode)
                    return updateCriteriaField();
                else
                    return insertCriteriaField();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.validateCriteriaField = function () {
                var fieldName = ($scope.fieldName != undefined) ? $scope.fieldName.toUpperCase() : null;

                if (isEditMode && (fieldName == criteriaFieldName))
                    return null;
                else if (UtilsService.getItemIndexByVal(criteriaFields, fieldName, 'FieldName') > -1) {
                    return 'Another criteria field with the same name already exists';
                }
                return null;
            };
        }

        function load() {
            $scope.isLoading = true;

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordFieldTypeSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((criteriaFieldEntity != undefined) ? criteriaFieldEntity.FieldName : null, 'Generic Rule Definition Criteria Field') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition Criteria Field');
            }
            function loadStaticData() {
                if (criteriaFieldEntity == undefined) {
                    return;
                }
                $scope.fieldName = criteriaFieldEntity.FieldName;
                $scope.fieldTitle = criteriaFieldEntity.Title;
                $scope.showInBasicSearch = criteriaFieldEntity.ShowInBasicSearch;

                $scope.selectedBehaviorType = UtilsService.getItemByVal($scope.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');

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
            return {
                $type: "Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities",
                FieldName: $scope.fieldName,
                Title: $scope.fieldTitle,
                FieldType: dataRecordFieldTypeSelectiveAPI.getData(),
                RuleStructureBehaviorType: $scope.selectedBehaviorType.value,
                ShowInBasicSearch: $scope.showInBasicSearch
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionCriteriaFieldEditorController', GenericRuleDefinitionCriteriaFieldEditorController);

})(appControllers);