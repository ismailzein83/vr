(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService', 'VR_GenericData_GenericUIService'];

    function genericRuleManagementController($scope, VRNavigationService, VR_GenericData_GenericRule, VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericRuleTypeConfigAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService, VR_GenericData_GenericUIService) {

        var gridAPI;

        var genericRuleDefinitionAPI;
        var genericRuleDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        var settingsFilterDirectiveAPI;
        var settingsFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var genericUIObj;

        loadParameters();
        defineScope();
        load();

        var viewId;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.filters = [];

            $scope.scopeModel.haveAddPermission = false;
            $scope.scopeModel.gridloadded = false;

            $scope.onGenericRuleDefinitionSelectorDirectiveReady = function (api) {
                genericRuleDefinitionAPI = api;
                genericRuleDefinitionReadyDeferred.resolve();
            };
            $scope.onSettingsFilterDirectiveReady = function (api) {
                settingsFilterDirectiveAPI = api;
                settingsFilterDirectiveReadyDeferred.resolve();
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var defFilter = {
                    RuleDefinitionId: genericRuleDefinitionAPI.getSelectedIds(),
                    EffectiveDate: $scope.scopeModel.effectiveDate,
                    Description: $scope.scopeModel.description
                };
                gridAPI.loadGrid(defFilter);
            };

            $scope.onGenericRuleDefinitionSelectorSelectionChange = function () {
                if (genericRuleDefinitionAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.filters.length = 0;
                    $scope.scopeModel.settingsFilterEditor = null;
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                        hasAddGenericRulePermission();
                    });
                }
            };

            $scope.search = function () {
                var filter = getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                VR_GenericData_GenericRule.addGenericRule(genericRuleDefinitionAPI.getSelectedIds(), onGenericRuleAdded);
            };

            function getFilterObject() {
                var gridQuery = { RuleDefinitionId: genericRuleDefinitionAPI.getSelectedIds() };
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
            loadGenericRuleDefinition();
        }

        function loadGenericRuleDefinition() {
            var loadGenericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            genericRuleDefinitionReadyDeferred.promise.then(function () {
                var payLoad;
                payLoad = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.GenericData.Business.GenericRuleDefinitionViewFilter, Vanrise.GenericData.Business",
                            ViewId: viewId
                        }]
                    },
                    selectfirstitem: true
                };
                VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionAPI, payLoad, loadGenericRuleDefinitionSelectorPromiseDeferred);
            });
            return loadGenericRuleDefinitionSelectorPromiseDeferred.promise.then(function () {
                $scope.isLoading = false;
                $scope.hideGenericRuleDefinition = genericRuleDefinitionAPI.hasSingleItem();

            });
        }

        function loadAllControls() {
            $scope.isLoading = true;

            function setStaticData() {
                $scope.scopeModel.effectiveDate = VRDateTimeService.getNowDateTime();
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

                    if (ruleDefinition.CriteriaDefinition != undefined) {
                        for (var i = 0; i < ruleDefinition.CriteriaDefinition.Fields.length; i++) {
                            var criteriaField = ruleDefinition.CriteriaDefinition.Fields[i];
                            var filter = getFilter(criteriaField);
                            if (filter != undefined) {
                                criteriaFilterPromises.push(filter.directiveLoadDeferred.promise);
                                $scope.scopeModel.filters.push(filter);
                            }
                        }
                    }

                    UtilsService.waitMultiplePromises(criteriaFilterPromises).then(function () {
                        getRuleDefinitionAndFieldTypeConfigsDeferred.resolve();
                    });
                }).catch(function (error) {
                    getRuleDefinitionAndFieldTypeConfigsDeferred.reject(error);
                });


                function getRuleDefinition() {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                        ruleDefinition = response;
                        genericUIObj = VR_GenericData_GenericUIService.createGenericUIObj(ruleDefinition.CriteriaDefinition.Fields);
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

                        criteriaField.genericUIContext = genericUIObj.getFieldContext(criteriaField); //VR_GenericData_GenericUIService.buildGenericUIContext(ruleDefinition.CriteriaDefinition.Fields, criteriaField);

                        var directivePayload = {
                            fieldTitle: criteriaField.Title,
                            fieldType: criteriaField.FieldType,
                            genericUIContext: criteriaField.genericUIContext
                        };


                        VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                    };

                    return filter;
                }
                function getRuleTypes() {
                    return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypes().then(function (response) {
                        var ruleTypes = [];
                        for (var i = 0; i < response.length; i++) {
                            ruleTypes.push(response[i]);
                        }
                        $scope.scopeModel.settingsFilterEditor = UtilsService.getItemByVal(ruleTypes, ruleDefinition.SettingsDefinition.ConfigId, 'ExtensionConfigurationId').FilterEditor;

                    });
                }
                function loadSettingsFilter() {
                    var settingsPromises = [];

                    var getRuleTypesPromise = getRuleTypes();
                    settingsPromises.push(getRuleTypesPromise);
                    return UtilsService.waitMultiplePromises(settingsPromises).then(function () {
                        if ($scope.scopeModel.settingsFilterEditor != null) {
                            setTimeout(function () {
                                var settingsFilterEditorLoadDeferred = UtilsService.createPromiseDeferred();
                                settingsPromises.push(settingsFilterEditorLoadDeferred.promise);
                                settingsFilterDirectiveReadyDeferred.promise.then(function () {
                                    var settingsFilterDirectivePayload = {
                                        genericRuleDefinition: ruleDefinition,
                                        settings: ruleDefinition.SettingsDefinition
                                    };
                                    VRUIUtilsService.callDirectiveLoad(settingsFilterDirectiveAPI, settingsFilterDirectivePayload, settingsFilterEditorLoadDeferred);
                                });
                            }, 1)
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    genericUIObj.loadingFinish();
                });
            }

            return UtilsService.waitMultipleAsyncOperations([setStaticData, loadFilters]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function hasAddGenericRulePermission() {
            return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(genericRuleDefinitionAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.haveAddPermission = response;
            });
        };
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
