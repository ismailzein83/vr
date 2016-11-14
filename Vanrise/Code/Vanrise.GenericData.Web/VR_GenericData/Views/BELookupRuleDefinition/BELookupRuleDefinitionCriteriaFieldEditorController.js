(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionCriteriaFieldEditorController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function BELookupRuleDefinitionCriteriaFieldEditorController($scope, VR_GenericData_BusinessEntityDefinitionAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService)
    {
        var isEditMode;
        
        var criteriaFieldTitle;
        var criteriaFieldEntity;
        var criteriaFields;

        var beDefinitionId;
        var beDataRecordTypeId;

        var dataRecordTypeFieldSelectorAPI;
        var dataRecordTypeFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
            {
                criteriaFieldTitle = parameters.criteriaFieldTitle;
                criteriaFields = UtilsService.cloneObject(parameters.criteriaFields, true);
                beDefinitionId = parameters.beDefinitionId;
            }

            isEditMode = (criteriaFieldTitle != undefined);
        }

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.behaviorTypes = UtilsService.getArrayEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum);

            $scope.scopeModel.showDataRecordTypeFieldSelector = false;

            $scope.scopeModel.onDataRecordTypeFieldSelectorReady = function (api)
            {
                dataRecordTypeFieldSelectorAPI = api;
                dataRecordTypeFieldSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateTitle = function ()
            {
                if (criteriaFields == undefined || criteriaFields == null)
                    return null;
                for (var i = 0; i < criteriaFields.length; i++)
                {
                    var item = criteriaFields[i];
                    if (item.Title.toLowerCase() == $scope.scopeModel.title.toLowerCase())
                        return 'Another criteria field with the same title already exists';
                }
                return null;
            };

            $scope.scopeModel.save = function ()
            {
                var crietriaField = buildCriteriaFieldFromScope();
                if (isEditMode) 
                    $scope.onBELookupRuleDefinitionCriteriaFieldUpdated(crietriaField);
                else
                    $scope.onBELookupRuleDefinitionCriteriaFieldAdded(crietriaField);
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.close = function ()
            {
                $scope.modalContext.closeModal();
            };
        }

        function load()
        {
            $scope.scopeModel.isLoading = true;

            if (isEditMode)
            {
                setCriteriaFieldEntity();

                loadAllControls().finally(function () {
                    var index = UtilsService.getItemIndexByVal(criteriaFields, criteriaFieldTitle, 'Title');
                    criteriaFields.splice(index, 1);
                    criteriaFieldEntity = undefined;
                });
            }
            else {
                loadAllControls();
            }
        }

        function setCriteriaFieldEntity()
        {
            criteriaFieldEntity = UtilsService.getItemByVal(criteriaFields, criteriaFieldTitle, 'Title');
        }

        function loadAllControls()
        {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeFieldSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode)
            {
                if (criteriaFieldEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(criteriaFieldEntity.Title, 'Criteria Field');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Criteria Field');
        }

        function loadStaticData() {
            if (criteriaFieldEntity == undefined)
                return;
            $scope.scopeModel.title = criteriaFieldEntity.Title;
            $scope.scopeModel.fieldPath = criteriaFieldEntity.FieldPath;
            $scope.scopeModel.selectedBehaviorType = UtilsService.getItemByVal($scope.scopeModel.behaviorTypes, criteriaFieldEntity.RuleStructureBehaviorType, 'value');
        }

        function loadDataRecordTypeFieldSelector()
        {
            var promises = [];

            var getBEDataRecordTypeIdIfGenericPromise = getBEDataRecordTypeIdIfGeneric();
            promises.push(getBEDataRecordTypeIdIfGenericPromise);

            var dataRecordTypeFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataRecordTypeFieldSelectorLoadDeferred.promise);

            UtilsService.waitMultiplePromises([dataRecordTypeFieldSelectorReadyDeferred.promise, getBEDataRecordTypeIdIfGenericPromise]).then(function ()
            {
                if (beDataRecordTypeId != null)
                {
                    $scope.scopeModel.showDataRecordTypeFieldSelector = true;
                    var payload = {
                        dataRecordTypeId: beDataRecordTypeId,
                        selectedIds: (criteriaFieldEntity != undefined) ? criteriaFieldEntity.FieldPath : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldSelectorAPI, payload, dataRecordTypeFieldSelectorLoadDeferred);
                }
                else
                    dataRecordTypeFieldSelectorLoadDeferred.resolve();
            });

            return UtilsService.waitMultiplePromises(promises);

            function getBEDataRecordTypeIdIfGeneric() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDataRecordTypeIdIfGeneric(beDefinitionId).then(function (response) {
                    beDataRecordTypeId = response;
                });
            }
        }

        function buildCriteriaFieldFromScope()
        {
            return {
                Title: $scope.scopeModel.title,
                FieldPath: (beDataRecordTypeId != undefined) ? dataRecordTypeFieldSelectorAPI.getSelectedIds() : $scope.scopeModel.fieldPath,
                RuleStructureBehaviorType: $scope.scopeModel.selectedBehaviorType.value
            };
        }
    }

    appControllers.controller('VR_GenericData_BELookupRuleDefinitionCriteriaFieldEditorController', BELookupRuleDefinitionCriteriaFieldEditorController);

})(appControllers);