(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_BELookupRuleDefinitionService', 'VR_GenericData_BELookupRuleDefinitionAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function BELookupRuleDefinitionEditorController($scope, VR_GenericData_BELookupRuleDefinitionService, VR_GenericData_BELookupRuleDefinitionAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var beLookupRuleDefinitionId;
        var beLookupRuleDefinitionEntity;

        var beDefinitionSelectorAPI;
        var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                beLookupRuleDefinitionId = parameters.beLookupRuleDefinitionId;

            isEditMode = (beLookupRuleDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.criteriaFields = [];

            $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                beDefinitionSelectorAPI = api;
                beDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateCriteriaFields = function () {
                return ($scope.scopeModel.criteriaFields.length == 0) ? 'Please add at least one criteria field' : null;
            };

            $scope.scopeModel.addCriteriaField = function () {
                var onCriteriaFieldAdded = function (addedCriteriaField) {
                    setBehaviorTypeDescription(addedCriteriaField);
                    $scope.scopeModel.criteriaFields.push(addedCriteriaField);
                };

                VR_GenericData_BELookupRuleDefinitionService.addBELookupRuleDefinitionCriteriaField
                (
                    $scope.scopeModel.criteriaFields,
                    beDefinitionSelectorAPI.getSelectedIds(),
                    onCriteriaFieldAdded
                );
            };

            $scope.scopeModel.removeCriteriaField = function (criteriaField) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.criteriaFields, criteriaField.Title, 'Title');
                $scope.scopeModel.criteriaFields.splice(index, 1);
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateBELookupRuleDefinition() : insertBELookupRuleDefinition();
            };

            $scope.scopeModel.hasSavePermission = function () {
                return (isEditMode) ?
                    VR_GenericData_BELookupRuleDefinitionAPIService.HasEditBELookupRuleDefinitionPermission() :
                    VR_GenericData_BELookupRuleDefinitionAPIService.HasAddBELookupRuleDefinitionPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions = [{
                name: 'Edit',
                clicked: editCriteriaField
            }];

            function editCriteriaField(dataItem) {
                var onCriteriaFieldUpdated = function (updatedCriteriaField) {
                    setBehaviorTypeDescription(updatedCriteriaField);
                    var outdatedCriteriaFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.criteriaFields, dataItem.Title, 'Title');
                    $scope.scopeModel.criteriaFields[outdatedCriteriaFieldIndex] = updatedCriteriaField;
                };

                VR_GenericData_BELookupRuleDefinitionService.editBELookupRuleDefinitionCriteriaField
                (
                    dataItem.Title,
                    $scope.scopeModel.criteriaFields,
                    beDefinitionSelectorAPI.getSelectedIds(),
                    onCriteriaFieldUpdated
                );
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getBELookupRuleDefinition().then(function () {
                    loadAllControls().finally(function () {
                        beLookupRuleDefinitionEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getBELookupRuleDefinition() {
            return VR_GenericData_BELookupRuleDefinitionAPIService.GetBELookupRuleDefinition(beLookupRuleDefinitionId).then(function (response) {
                beLookupRuleDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBEDefinitionSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode) {
                if (beLookupRuleDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(beLookupRuleDefinitionEntity.Name, 'BE Lookup Rule Definition');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('BE Lookup Rule Definition');
        }

        function loadStaticData() {
            if (beLookupRuleDefinitionEntity == undefined)
                return;
            $scope.scopeModel.name = beLookupRuleDefinitionEntity.Name;

            // Load the criteria fields
            for (var i = 0; i < beLookupRuleDefinitionEntity.CriteriaFields.length; i++) {
                var item = beLookupRuleDefinitionEntity.CriteriaFields[i];
                setBehaviorTypeDescription(item);
                $scope.scopeModel.criteriaFields.push(item);
            }
        }

        function loadBEDefinitionSelector() {
            var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            beDefinitionSelectorReadyDeferred.promise.then(function () {
                var payload = (beLookupRuleDefinitionEntity != undefined) ? { selectedIds: beLookupRuleDefinitionEntity.BusinessEntityDefinitionId } : undefined;
                VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, payload, beDefinitionSelectorLoadDeferred);
            });

            return beDefinitionSelectorLoadDeferred.promise;
        }

        function insertBELookupRuleDefinition() {
            $scope.scopeModel.isLoading = true;

            var beLookupRuleDefinition = buildBELookupRuleDefinitionFromScope();

            return VR_GenericData_BELookupRuleDefinitionAPIService.AddBELookupRuleDefinition(beLookupRuleDefinition).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('BE Lookup Rule Definition', response, 'Name')) {
                    if ($scope.onBELookupRuleDefinitionAdded != undefined)
                        $scope.onBELookupRuleDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateBELookupRuleDefinition() {
            $scope.scopeModel.isLoading = true;

            var beLookupRuleDefinition = buildBELookupRuleDefinitionFromScope();

            return VR_GenericData_BELookupRuleDefinitionAPIService.UpdateBELookupRuleDefinition(beLookupRuleDefinition).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('BE Lookup Rule Definition', response, 'Name')) {
                    if ($scope.onBELookupRuleDefinitionUpdated != undefined)
                        $scope.onBELookupRuleDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildBELookupRuleDefinitionFromScope() {
            return {
                BELookupRuleDefinitionId: beLookupRuleDefinitionId,
                Name: $scope.scopeModel.name,
                BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                CriteriaFields: buildCriteriaFields()
            };

            function buildCriteriaFields() {
                var criteriaFields = [];
                for (var i = 0; i < $scope.scopeModel.criteriaFields.length; i++) {
                    var item = $scope.scopeModel.criteriaFields[i];
                    criteriaFields.push({
                        Title: item.Title,
                        FieldPath: item.FieldPath,
                        RuleStructureBehaviorType: item.RuleStructureBehaviorType
                    });
                }
                return criteriaFields;
            }
        }

        function setBehaviorTypeDescription(criteriaField) {
            var behaviorType = UtilsService.getEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum, 'value', criteriaField.RuleStructureBehaviorType);
            criteriaField.RuleStructureBehaviorTypeDescription = behaviorType.description;
        }
    }

    appControllers.controller('VR_GenericData_BELookupRuleDefinitionEditorController', BELookupRuleDefinitionEditorController);

})(appControllers);