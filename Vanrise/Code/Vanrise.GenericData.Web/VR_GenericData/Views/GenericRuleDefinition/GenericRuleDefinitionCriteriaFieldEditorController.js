(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var criteriaFields;
        var criteriaFieldName;
        var criteriaFieldEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            // Note that parameters are always passed to this editor
            if (parameters != undefined) {
                criteriaFieldName = parameters.genericRuleDefinitionCriteriaFieldName;
                criteriaFields = parameters.genericRuleDefinitionCriteriaFields;
            }

            isEditMode = (criteriaFieldName != undefined);
        }

        function defineScope() {
            $scope.behaviorTypes = UtilsService.getArrayEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum);

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
                    if ($scope.name != undefined && criteriaFields[i].FieldName == $scope.name) {
                        return 'Invalid name';
                    }
                    if ($scope.priority != undefined && criteriaFields[i].Priority == Number($scope.priority)) {
                        return 'Invalid priority';
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
            criteriaFieldEntity = UtilsService.getItemByValue(criteriaFields, criteriaFieldName, 'FieldName');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
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
            }
        }

        function insertCriteriaField() {
            var criteriaFieldObject = buildCriteriaFieldObjectFromScope();
            
            console.log($scope.onGenericRuleDefinitionCriteriaFieldAdded);
            if ($scope.onGenericRuleDefinitionCriteriaFieldAdded != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldAdded) == 'fucnction') {
                $scope.onGenericRuleDefinitionCriteriaFieldAdded(criteriaFieldObject);
                $scope.modalContext.closeModal();
            }
        }

        function updateCriteriaField() {
            var criteriaFieldObject = buildCriteriaFieldObjectFromScope();
            VRNotificationService.showSuccess('Generic rule definition criteria field updated');

            if ($scope.onGenericRuleDefinitionCriteriaFieldUpdated != undefined && typeof ($scope.onGenericRuleDefinitionCriteriaFieldUpdated) == 'fucnction') {
                $scope.onGenericRuleDefinitionCriteriaFieldUpdated(criteriaFieldObject);
                $scope.modalContext.closeModal();
            }
        }

        function buildCriteriaFieldObjectFromScope() {
            return {
                FieldName: $scope.name,
                Title: $scope.fieldTitle,
                FieldType: dataRecordTypeSelectorAPI.getSelectedIds(),
                RuleStructureBehaviorType: $scope.selectedBehaviorType,
                Priority: $scope.priority
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionCriteriaFieldEditorController', GenericRuleDefinitionCriteriaFieldEditorController);

})(appControllers);