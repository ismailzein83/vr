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
            this.initializeController = initializeController;

            var criteriaFieldsToHide;
            var ruleDefinitionId;
            var criteriaDefinitionConfigs;
            var predefinedCriteriaFieldValues;
            var accessibility;
            var context;

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            var searchDirectiveAPI;
            var searchDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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

                $scope.scopeModel.onSearchDirectiveReady = function (api) {
                    searchDirectiveAPI = api;
                    searchDirectiveReadyDeferred.resolve();
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

                UtilsService.waitMultiplePromises([loadCriteriaDefinitionConfigs()]).then(function () {
                    defineAPI();
                });

                function loadCriteriaDefinitionConfigs() {
                    return VR_GenericData_GenericRuleDefinitionAPIService.GetCriteriaDefinitionConfigs().then(function (response) {
                        criteriaDefinitionConfigs = response;
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
                            loadSearchDirective().then(function () {
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

            function loadRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(ruleDefinitionId).then(function (response) {
                    $scope.scopeModel.ruleDefinition = response;
                    $scope.scopeModel.selectedRuleDefinitionType = UtilsService.getItemByVal(criteriaDefinitionConfigs, response.CriteriaDefinition.ConfigId, "ExtensionConfigurationId");
                });
            }

            function loadSearchDirective() {
                var loadSearchDirectiveDeferred = UtilsService.createPromiseDeferred();

                searchDirectiveReadyDeferred.promise.then(function () {
                    searchDirectiveReadyDeferred = undefined;

                    var searchDirectivePayload = {
                        ruleDefinition: $scope.scopeModel.ruleDefinition,
                        criteriaFieldsToHide: criteriaFieldsToHide
                    };
                    VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, loadSearchDirectiveDeferred);
                });

                return loadSearchDirectiveDeferred.promise;
            };

            function loadGenericRuleGrid() {
                var loadGridDeferred = UtilsService.createPromiseDeferred();

                gridPromiseDeferred.promise.then(function () {
                    gridAPI.loadGrid(getFilterObject()).then(function () {
                        loadGridDeferred.resolve();
                    });
                });

                function getFilterObject() {
                    var gridQuery = {};
                    gridQuery.RuleDefinitionId = ruleDefinitionId;
                    gridQuery.CriteriaFieldValues = getCriteriaFieldValues(predefinedCriteriaFieldValues);
                    gridQuery.criteriaFieldsToHide = criteriaFieldsToHide;
                    gridQuery.accessibility = accessibility;
                    gridQuery.EffectiveDate = $scope.scopeModel.effectiveDate;
                    gridQuery.Description = $scope.scopeModel.description;

                    return gridQuery;
                }
                function getCriteriaFieldValues(predefinedCriteriaFieldValues) {
                    var searchDirectiveCriteriaFieldValues = searchDirectiveAPI.getSearchObject();
                    if (searchDirectiveCriteriaFieldValues == undefined || searchDirectiveCriteriaFieldValues.CriteriaFieldValues == undefined)
                        return predefinedCriteriaFieldValues;

                    if (predefinedCriteriaFieldValues == undefined)
                        return searchDirectiveCriteriaFieldValues.CriteriaFieldValues;

                    var criteriaFieldValues = {};

                    for (var key in predefinedCriteriaFieldValues) {
                        criteriaFieldValues[key] = predefinedCriteriaFieldValues[key];
                    }

                    for (var key in searchDirectiveCriteriaFieldValues.CriteriaFieldValues) {
                        criteriaFieldValues[key] = searchDirectiveCriteriaFieldValues.CriteriaFieldValues[key];
                    }

                    return criteriaFieldValues;
                }

                return loadGridDeferred.promise;
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

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }]);