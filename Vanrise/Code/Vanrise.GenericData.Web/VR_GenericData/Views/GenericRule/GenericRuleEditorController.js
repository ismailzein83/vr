(function (appControllers) {

    "use strict";

    genericRuleEditorController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_DataRecordFieldAPIService',
        'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'UtilsService', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService'];

    function genericRuleEditorController($scope, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_DataRecordFieldAPIService,
        VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleTypeConfigAPIService, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService, VRValidationService) {

        var isEditMode;

        var genericRuleId;
        var genericRuleDefinitionId;
        var genericRuleDefintion;
        var genericRuleEntity;
        var genericRuleTypeConfig;
        var criteriaDefinitionFields;
        var criteriaFieldsValues;

        var preDefinedData;
        var accessibility;

        var settingsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var settingsDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                genericRuleId = parameters.genericRuleId;
                genericRuleDefinitionId = parameters.genericRuleDefinitionId;
                preDefinedData = parameters.preDefinedData;
                accessibility = parameters.accessibility;
            }
            isEditMode = (genericRuleId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveGenericRule = function () {
                if (isEditMode) {
                    return updateRouteRule();
                }
                else {
                    return insertRouteRule();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateDates = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
            };


            $scope.scopeModel.beginEffectiveDate = new Date();
            $scope.scopeModel.endEffectiveDate = undefined;

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            getGenericRuleDefinition().then(function () {
                if (isEditMode) {
                    getGenericRule().then(function () {
                        loadAllControls()
                            .finally(function () {
                                genericRuleEntity = undefined;
                            });
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else {
                    loadAllControls();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });


        }

        function getGenericRuleDefinition() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                genericRuleDefintion = response;
                criteriaDefinitionFields = (genericRuleDefintion != null && genericRuleDefintion.CriteriaDefinition != null) ? genericRuleDefintion.CriteriaDefinition.Fields : undefined;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadCriteriaSection, loadSettingsSection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function getGenericRule() {
            return VR_GenericData_GenericRuleAPIService.GetGenericRule(genericRuleDefinitionId, genericRuleId).then(function (genericRule) {
                genericRuleEntity = genericRule;
                criteriaFieldsValues = (genericRuleEntity != undefined && genericRuleEntity.Criteria != null && genericRuleEntity.Criteria.FieldsValues != null) ? genericRuleEntity.Criteria.FieldsValues : undefined;
            });
        }

        function setTitle() {
            $scope.title = (isEditMode) ? UtilsService.buildTitleForUpdateEditor(genericRuleDefintion.Name) : UtilsService.buildTitleForAddEditor(genericRuleDefintion.Name);
        }

        function loadStaticSection() {
            if (genericRuleEntity == undefined)
                return;
            $scope.scopeModel.description = genericRuleEntity.Description;
            $scope.scopeModel.beginEffectiveDate = genericRuleEntity.BeginEffectiveTime;
            $scope.scopeModel.endEffectiveDate = genericRuleEntity.EndEffectiveTime;
        }

        function loadCriteriaSection() {
            if (criteriaDefinitionFields == undefined)
                return;
            var promises = [];

            var loadAllFieldsPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadAllFieldsPromiseDeferred.promise);

            var loadFieldTypeConfigPromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (allConfigs) {

                var criteriaFieldsPromises = [];

                angular.forEach(criteriaDefinitionFields, function (field) {
                    var dataFieldTypeConfig = UtilsService.getItemByVal(allConfigs, field.FieldType.ConfigId, 'ExtensionConfigurationId');
                    field.runtimeEditor = {};
                    field.runtimeEditor.directive = dataFieldTypeConfig.RuntimeEditor;
                    field.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    field.runtimeEditor.onDirectiveReady = function (api) {
                        field.runtimeEditor.directiveAPI = api;
                        field.runtimeEditor.onReadyPromiseDeferred.resolve();
                    };

                    field.runtimeEditor.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    criteriaFieldsPromises.push(field.runtimeEditor.loadPromiseDeferred.promise);

                    if (accessibility != undefined && accessibility.criteriaAccessibility != undefined) {
                        var accessibleField = accessibility.criteriaAccessibility[field.FieldName];
                        if (accessibleField != undefined) {
                            field.notAccessible = accessibleField.notAccessible;
                        }
                    }

                    field.runtimeEditor.onReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            fieldTitle: field.Title,
                            fieldType: field.FieldType,
                            fieldValue: (criteriaFieldsValues != undefined) ? criteriaFieldsValues[field.FieldName] : (preDefinedData != undefined && preDefinedData.criteriaFieldsValues != undefined ? preDefinedData.criteriaFieldsValues[field.FieldName] : undefined),
                        };
                        VRUIUtilsService.callDirectiveLoad(field.runtimeEditor.directiveAPI, payload, field.runtimeEditor.loadPromiseDeferred);
                    });
                });

                UtilsService.waitMultiplePromises(criteriaFieldsPromises).then(function () {
                    loadAllFieldsPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadAllFieldsPromiseDeferred.reject(error);
                });

                $scope.scopeModel.criteriaFields = genericRuleDefintion.CriteriaDefinition.Fields;
            });

            promises.push(loadFieldTypeConfigPromise);

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadSettingsSection() {
            if (genericRuleDefintion == undefined && genericRuleDefintion.SettingsDefinition != null)
                return;

            var promises = [];

            var loadSettingsSectionPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadSettingsSectionPromiseDeferred.promise);

            var loadRuleTypeConfigPromise = VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypeById(genericRuleDefintion.SettingsDefinition.ConfigId).then(function (response) {
                genericRuleTypeConfig = response;
                $scope.scopeModel.settingsDirective = genericRuleTypeConfig.RuntimeEditor;

                settingsDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        genericRuleDefinition: genericRuleDefintion,
                        settings: (genericRuleEntity != undefined && genericRuleEntity.Settings != null) ? genericRuleEntity.Settings : (preDefinedData != undefined && preDefinedData.settings != undefined ? preDefinedData.settings : undefined)
                    };
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, loadSettingsSectionPromiseDeferred);
                });
            });
            $scope.scopeModel.settingNotAccessible = accessibility != undefined ? accessibility.settingNotAccessible : undefined;
            promises.push(loadRuleTypeConfigPromise);

            return UtilsService.waitMultiplePromises(promises);
        }

        function buildGenericRuleObjFromScope() {
            var genericRuleCriteria = {};

            if (criteriaDefinitionFields != undefined) {
                genericRuleCriteria.FieldsValues = {};
                var criteriaValuesExist = false;

                angular.forEach(criteriaDefinitionFields, function (field) {
                    var fieldData = field.runtimeEditor.directiveAPI.getData();
                    if (fieldData != undefined) {
                        genericRuleCriteria.FieldsValues[field.FieldName] = fieldData;
                        criteriaValuesExist = true;
                    }
                });

                if (!criteriaValuesExist)
                    genericRuleCriteria = undefined;
            }

            var genericRule = {
                $type: genericRuleTypeConfig.RuleTypeFQTN,
                RuleId: (genericRuleId != null) ? genericRuleId : 0,
                DefinitionId: genericRuleDefinitionId,
                Criteria: genericRuleCriteria,
                Settings: settingsDirectiveAPI.getData(),
                BeginEffectiveTime: $scope.scopeModel.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModel.endEffectiveDate,
                Description: $scope.scopeModel.description
            };

            return genericRule;
        }

        function insertRouteRule() {
            var genericRuleObj = buildGenericRuleObjFromScope();
            return VR_GenericData_GenericRuleAPIService.AddGenericRule(genericRuleObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded(genericRuleDefintion.Name, response)) {
                    if ($scope.onGenericRuleAdded != undefined)
                        $scope.onGenericRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateRouteRule() {
            var genericRuleObj = buildGenericRuleObjFromScope();
            return VR_GenericData_GenericRuleAPIService.UpdateGenericRule(genericRuleObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated(genericRuleDefintion.Name, response)) {
                    if ($scope.onGenericRuleUpdated != undefined)
                        $scope.onGenericRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleEditorController', genericRuleEditorController);
})(appControllers);
