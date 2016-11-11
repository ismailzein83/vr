(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRule, VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericRuleTypeConfigAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        var settingsFilterDirectiveAPI;
        var settingsFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        var ruleDefinitionId

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                ruleDefinitionId = parameters.ruleDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.filters = [];



            $scope.onSettingsFilterDirectiveReady = function (api) {
                settingsFilterDirectiveAPI = api;
                settingsFilterDirectiveReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: ruleDefinitionId,
                    EffectiveDate: $scope.scopeModel.effectiveDate,
                    Description: $scope.scopeModel.description
                };
                gridAPI.loadGrid(defFilter);
            };
            $scope.hasAddGenericRulePermission = function () {
                return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(ruleDefinitionId);
            };
            $scope.search = function () {
                var filter = getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                VR_GenericData_GenericRule.addGenericRule(ruleDefinitionId, onGenericRuleAdded);
            };

            function getFilterObject() {
                var gridQuery = { RuleDefinitionId: ruleDefinitionId };
                gridQuery.EffectiveDate = $scope.scopeModel.effectiveDate;
                gridQuery.Description = $scope.scopeModel.description;

                gridQuery.CriteriaFieldValues = {};
                var criteriaFilterValuesExist = false;

                for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                    var criteriaFilterData = $scope.scopeModel.filters[i].directiveAPI.getData();
                    if (criteriaFilterData != undefined) {
                        gridQuery.CriteriaFieldValues[$scope.scopeModel.filters[i].criteriaFieldName] = criteriaFilterData;
                        criteriaFilterValuesExist = true;
                    }
                }

                if (!criteriaFilterValuesExist) {
                    gridQuery.CriteriaFieldValues = undefined;
                }

                if (settingsFilterDirectiveAPI != undefined) {
                    var settingsFilterData = settingsFilterDirectiveAPI.getData();
                    if (settingsFilterData != undefined) {
                        gridQuery.SettingsFilterValue = settingsFilterData;
                    }
                }

                return gridQuery;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setStaticData, loadFilters]).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });

                function setStaticData() {
                    $scope.scopeModel.effectiveDate = new Date();
                }

                function loadFilters() {
                    var promises = [];
                    var ruleDefinition;
                    var fieldTypes;

                    var getRuleDefinitionPromise = getRuleDefinition();
                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
                    var getRuleDefinitionAndFieldTypeConfigsDeferred = UtilsService.createPromiseDeferred();
                    promises.push(getRuleDefinitionAndFieldTypeConfigsDeferred.promise);

                    var loadSettingsFilterDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadSettingsFilterDirectiveDeferred.promise);

                    getRuleDefinitionPromise.then(function () {
                        loadSettingsFilter().then(function () {
                            loadSettingsFilterDirectiveDeferred.resolve();
                        }).catch(function (error) {
                            loadSettingsFilterDirectiveDeferred.reject(error);
                        });
                    });

                    UtilsService.waitMultiplePromises([getRuleDefinitionPromise, getFieldTypeConfigsPromise]).then(function () {
                        var criteriaFilterPromises = [];

                        for (var i = 0; i < ruleDefinition.CriteriaDefinition.Fields.length; i++) {
                            var criteriaField = ruleDefinition.CriteriaDefinition.Fields[i];
                            var filter = getFilter(criteriaField);
                            if (filter != undefined) {
                                criteriaFilterPromises.push(filter.directiveLoadDeferred.promise);
                                $scope.scopeModel.filters.push(filter);
                            }
                        }

                        UtilsService.waitMultiplePromises(criteriaFilterPromises).then(function () {
                            getRuleDefinitionAndFieldTypeConfigsDeferred.resolve();
                        });
                    }).catch(function (error) {
                        getRuleDefinitionAndFieldTypeConfigsDeferred.reject(error);
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getRuleDefinition() {
                        return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId).then(function (response) {
                            ruleDefinition = response;
                        });
                    }
                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypes = [];
                            for (var i = 0; i < response.length; i++) {
                                fieldTypes.push(response[i]);
                            }
                        });
                    }
                    function getFilter(criteriaField) {
                        var filter;
                        var filterEditor = UtilsService.getItemByVal(fieldTypes, criteriaField.FieldType.ConfigId, 'ExtensionConfigurationId').FilterEditor;

                        if (filterEditor == null) return filter;

                        filter = {};
                        filter.criteriaFieldName = criteriaField.FieldName;
                        filter.showInBasicSearch = criteriaField.ShowInBasicSearch;

                        filter.directiveEditor = filterEditor;
                        filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        filter.onDirectiveReady = function (api) {
                            filter.directiveAPI = api;
                            var directivePayload = {
                                fieldTitle: criteriaField.Title,
                                fieldType: criteriaField.FieldType
                            };
                            VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                        };

                        return filter;
                    }
                    function loadSettingsFilter() {
                        var settingsPromises = [];
                        var ruleTypes;

                        var getRuleTypesPromise = getRuleTypes();
                        settingsPromises.push(getRuleTypesPromise);

                        var settingsFilterEditorLoadDeferred = UtilsService.createPromiseDeferred();
                        settingsPromises.push(settingsFilterEditorLoadDeferred.promise);

                        getRuleTypesPromise.then(function () {
                            $scope.scopeModel.settingsFilterEditor = UtilsService.getItemByVal(ruleTypes, ruleDefinition.SettingsDefinition.ConfigId, 'ExtensionConfigurationId').FilterEditor;

                            if ($scope.scopeModel.settingsFilterEditor != null) {
                                settingsFilterDirectiveReadyDeferred.promise.then(function () {
                                    var settingsFilterDirectivePayload = {
                                        genericRuleDefinition: ruleDefinition,
                                        settings: ruleDefinition.SettingsDefinition
                                    };
                                    VRUIUtilsService.callDirectiveLoad(settingsFilterDirectiveAPI, settingsFilterDirectivePayload, settingsFilterEditorLoadDeferred);
                                });
                            }
                            else {
                                settingsFilterEditorLoadDeferred.resolve();
                            }
                        });

                        return UtilsService.waitMultiplePromises(settingsPromises);

                        function getRuleTypes() {
                            return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypes().then(function (response) {
                                ruleTypes = [];
                                for (var i = 0; i < response.length; i++) {
                                    ruleTypes.push(response[i]);
                                }
                            });
                        }
                    }
                }


            }
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
