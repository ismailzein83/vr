(function (appControllers) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var criteriaFields;
        var criteriaFieldName;
        var criteriaFieldEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var behaviorTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.onBehaviorTypeSelectorReady = function (api) {
                behaviorTypeSelectorReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadBehaviorTypeDirective]).catch(function (error) {
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
                $scope.name = criteriaFieldEntity.FieldName;
                $scope.fieldTitle = criteriaFieldEntity.Title;
                $scope.priority = criteriaFieldEntity.Priority;
            }
            function loadDataRecordTypeSelector() {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeSelectorPayload;
                    
                    if (criteriaFieldEntity != undefined) {
                        dataRecordTypeSelectorPayload = {
                            selectedIds: criteriaFieldEntity.FieldType
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }
            function loadBehaviorTypeDirective() {
                var behaviorTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                behaviorTypeSelectorReadyDeferred.promise.then(function () {
                    if (criteriaFieldEntity != undefined) {
                        $scope.selectedBehaviorType = UtilsService.getItemByValue($scope.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');
                    }
                    behaviorTypeSelectorLoadDeferred.resolve();
                });

                return behaviorTypeSelectorLoadDeferred.promise;
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