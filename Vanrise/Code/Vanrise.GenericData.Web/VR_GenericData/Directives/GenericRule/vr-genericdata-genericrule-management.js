"use strict";

app.directive("vrGenericdataGenericruleManagement", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VR_GenericData_DataRecordFieldAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_GenericData_GenericRule, VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRuleTypeConfigAPIService, VR_GenericData_DataRecordFieldAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var genericRuleManagement = new GenericRuleManagement($scope, ctrl, $attrs);
            genericRuleManagement.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericRule/Templates/GenericRuleManagementTemplate.html"

    };

    function GenericRuleManagement($scope, ctrl, $attrs) {

        var gridAPI;
        var gridPromiseDeferred = UtilsService.createPromiseDeferred();

        var settingsFilterDirectiveAPI;
        var settingsFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var criteriaFieldsToHide;
        var ruleDefinitionId;
        var predefinedCriteriaFieldValues;
        var accessibility;
        var context;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            

            $scope.scopeModel.filters = [];
            $scope.scopeModel.effectiveDate;
            $scope.scopeModel.settingsFilterEditor;
            $scope.scopeModel.hasAddRulePermission = false;
            $scope.scopeModel.ruleDefinition;
            $scope.scopeModel.supportUpload = false;

            $scope.scopeModel.searchClicked = function () {
                return loadGenericRuleGrid();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSettingsFilterDirectiveReady = function (api) {
                settingsFilterDirectiveAPI = api;
                settingsFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                var preDefinedData = {};
                preDefinedData.criteriaFieldsValues = {};
                if (predefinedCriteriaFieldValues != undefined) {
                    for (var key in predefinedCriteriaFieldValues)
                        preDefinedData.criteriaFieldsValues[key] = GetValues([predefinedCriteriaFieldValues[key]]);
                }
                VR_GenericData_GenericRule.addGenericRule(ruleDefinitionId, onGenericRuleAdded, preDefinedData, accessibility);
            };
            $scope.scopeModel.uploadGenericRules = function () {
                var criteriaFieldsValues = {};
                if (predefinedCriteriaFieldValues != undefined) {
                    for (var key in predefinedCriteriaFieldValues)
                        criteriaFieldsValues[key] = GetValues([predefinedCriteriaFieldValues[key]]);
                }
                VR_GenericData_GenericRule.uploadGenericRules(ruleDefinitionId, getContext(), criteriaFieldsToHide, criteriaFieldsValues);
            };

            UtilsService.waitMultiplePromises([gridPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function GetValues(fieldValues) {
            var obj = {
                Values: []
            };
            for (var i = 0; i < fieldValues.length; i++) {
                var bes = fieldValues[i];
                for (var j = 0; j < bes.BusinessEntityIds.length; j++) {
                    obj.Values.push(bes.BusinessEntityIds[j]);
                }
            }
            return obj;
        }

        function defineAPI() {
            var api = {};
            api.loadDirective = function (payload) {
                var promises = [];
    
                if (payload != undefined) {
                    ruleDefinitionId = payload.RuleDefinitionId;
                    criteriaFieldsToHide = payload.criteriaFieldsToHide;
                    predefinedCriteriaFieldValues = payload.CriteriaFieldValues;
                    accessibility = payload.accessibility;

                    var hasAddGenericRulePermissionPromise = hasAddGenericRulePermission();
                    promises.push(hasAddGenericRulePermissionPromise);

                    var supportUploadPromise = supportUpload();
                    promises.push(supportUploadPromise);

                    var loadRuleDefinitionPromise = loadRuleDefinition();
                    promises.push(loadRuleDefinitionPromise);

                    var loadFiltersDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadFiltersDeferred.promise);

                    var loadGridDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadGridDeferred.promise);

                    loadRuleDefinitionPromise.then(function () {
                        $scope.scopeModel.filters = $scope.scopeModel.ruleDefinition.CriteriaDefinition.Fields;
                        loadFilters().then(function () {
                            loadFiltersDeferred.resolve();
                        }).catch(function (error) {
                            loadFiltersDeferred.reject(error);
                        });
                    });

                    loadFiltersDeferred.promise.then(function () {
                        loadGenericRuleGrid().then(function () {
                            loadGridDeferred.resolve();
                        }).catch(function (error) {
                            loadGridDeferred.reject(error);
                        });
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function loadGenericRuleGrid() {
            return gridAPI.loadGrid(getFilterObject());
        }

        function loadRuleDefinition() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId).then(function (response) {
                $scope.scopeModel.ruleDefinition = response;
            });
        }

        function hasAddGenericRulePermission() {
            return VR_GenericData_GenericRuleAPIService.DoesUserHaveAddAccess(ruleDefinitionId).then(function (response) {
                $scope.scopeModel.hasAddRulePermission = response;
            });
        };

        function supportUpload() {
            return VR_GenericData_GenericRuleAPIService.DoesRuleSupportUpload(ruleDefinitionId).then(function (response) {
                $scope.scopeModel.supportUpload = response;
            });
        }

        function getFilterObject() {
            var gridQuery = {
                RuleDefinitionId: ruleDefinitionId,
                criteriaFieldsToHide: criteriaFieldsToHide,
                accessibility: accessibility
            };
            gridQuery.EffectiveDate = $scope.scopeModel.effectiveDate;
            gridQuery.Description = $scope.scopeModel.description;

            gridQuery.CriteriaFieldValues = {};
            var criteriaFilterValuesExist = false;

            for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                var filter = $scope.scopeModel.filters[i];
                var directiveAPI = filter.directiveAPI;
                if (directiveAPI != undefined) {
                    var criteriaFilterData = filter.directiveAPI.getData();
                    if (criteriaFilterData != undefined) {
                        gridQuery.CriteriaFieldValues[filter.criteriaFieldName] = criteriaFilterData;
                        criteriaFilterValuesExist = true;
                    }
                }
            }
            if (predefinedCriteriaFieldValues != undefined) {
                for (var key in predefinedCriteriaFieldValues) {
                    gridQuery.CriteriaFieldValues[key] = predefinedCriteriaFieldValues[key];
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

        function loadFilters() {

            var promises = [];

            var fieldTypes;

            var getFieldTypeConfigsPromise = getFieldTypeConfigs();
            promises.push(getFieldTypeConfigsPromise);

            var loadSettingsFilterPromise = loadSettingsFilter();
            promises.push(loadSettingsFilterPromise);

            var getRuleDefinitionAndFieldTypeConfigsDeferred = UtilsService.createPromiseDeferred();
            promises.push(getRuleDefinitionAndFieldTypeConfigsDeferred.promise);

            var loadSettingsFilterDirectiveDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadSettingsFilterDirectiveDeferred.promise);

            getFieldTypeConfigsPromise.then(function () {
                var criteriaFilterPromises = [];

                if ($scope.scopeModel.ruleDefinition.CriteriaDefinition != undefined) {
                    for (var i = 0; i < $scope.scopeModel.ruleDefinition.CriteriaDefinition.Fields.length; i++) {
                        var criteriaField = $scope.scopeModel.ruleDefinition.CriteriaDefinition.Fields[i];

                        if (criteriaField.FieldType != undefined) {
                            var filter = getFilter(criteriaField);
                            if (filter != undefined) {
                                criteriaFilterPromises.push(filter.directiveLoadDeferred.promise);
                                $scope.scopeModel.filters.push(filter);
                            }
                        }
                    }
                }

                UtilsService.waitMultiplePromises(criteriaFilterPromises).then(function () {
                    getRuleDefinitionAndFieldTypeConfigsDeferred.resolve();
                }).catch(function (error) {
                    getRuleDefinitionAndFieldTypeConfigsDeferred.reject(error);
                });
            });

            loadSettingsFilterPromise.then(function () {
                loadSettingsFilterDirectiveDeferred.resolve();
            }).catch(function (error) {
                loadSettingsFilterDirectiveDeferred.reject(error);
            });

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
                filter.showInBasicSearch = !UtilsService.contains(criteriaFieldsToHide, criteriaField.FieldName);//criteriaField.ShowInBasicSearch;

                filter.directiveEditor = filterEditor;
                filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                filter.onDirectiveReady = function (api) {
                    filter.directiveAPI = api;
                    var directivePayload = {
                        fieldTitle: criteriaField.Title,
                        fieldType: criteriaField.FieldType,
                        fieldValue: (predefinedCriteriaFieldValues != undefined) ? predefinedCriteriaFieldValues[criteriaField.FieldName] : undefined
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
                    $scope.scopeModel.settingsFilterEditor = UtilsService.getItemByVal(ruleTypes, $scope.scopeModel.ruleDefinition.SettingsDefinition.ConfigId, 'ExtensionConfigurationId').FilterEditor;
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
                                    genericRuleDefinition: $scope.scopeModel.ruleDefinition,
                                    settings: $scope.scopeModel.ruleDefinition.SettingsDefinition
                                };
                                VRUIUtilsService.callDirectiveLoad(settingsFilterDirectiveAPI, settingsFilterDirectivePayload, settingsFilterEditorLoadDeferred);
                            });
                        }, 1)
                    }
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;                                
        }
    }

    return directiveDefinitionObject;

}]);
