(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var criteriaFields;
        var criteriaFieldName;
        var criteriaFieldEntity;

        var dataRecordFieldTypeSelectiveAPI;
        var dataRecordFieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            // Note that parameters are always passed to this editor
            if (parameters != undefined) {
                criteriaFieldName = parameters.genericRuleDefinitionCriteriaFieldName;
                // setCriteriaFieldEntityFromParameters removes criteriaFieldEntity from criteriaFields
                // Therefore, parameters.genericRuleDefinitionCriteriaFields is cloned to avoid modifiying the original array
                criteriaFields = UtilsService.cloneObject(parameters.genericRuleDefinitionCriteriaFields);
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
                if (criteriaFields == undefined) {
                    return null;
                }
                for (var i = 0; i < criteriaFields.length; i++) {
                    if (criteriaFields[i].FieldName == $scope.fieldName) {
                        return 'Invalid name';
                    }
                }
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                setCriteriaFieldEntityFromParameters().then(function () {
                    loadAllControls().finally(function () {
                        genericRuleDefinitionEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function setCriteriaFieldEntityFromParameters() {
            var index = UtilsService.getItemIndexByVal(criteriaFields, criteriaFieldName, 'FieldName');
            criteriaFieldEntity = criteriaFields[index];
            criteriaFields.splice(index, 1);

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
                $scope.selectedBehaviorType = UtilsService.getItemByVal($scope.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');
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
            $scope.onGenericRuleDefinitionCriteriaFieldAdded(criteriaFieldObject);
            VRNotificationService.showSuccess('Criteria field added');
            $scope.modalContext.closeModal();
        }

        function updateCriteriaField() {
            var criteriaFieldObject = buildCriteriaFieldObjectFromScope();
            $scope.onGenericRuleDefinitionCriteriaFieldUpdated(criteriaFieldObject);
            VRNotificationService.showSuccess('Criteria field updated');
            $scope.modalContext.closeModal();
        }

        function buildCriteriaFieldObjectFromScope() {
            return {
                $type: "Vanrise.GenericData.Entities.GenericRuleDefinitionCriteriaField, Vanrise.GenericData.Entities",
                FieldName: $scope.fieldName,
                Title: $scope.fieldTitle,
                FieldType: dataRecordFieldTypeSelectiveAPI.getData(),
                RuleStructureBehaviorType: $scope.selectedBehaviorType.value
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionCriteriaFieldEditorController', GenericRuleDefinitionCriteriaFieldEditorController);

})(appControllers);