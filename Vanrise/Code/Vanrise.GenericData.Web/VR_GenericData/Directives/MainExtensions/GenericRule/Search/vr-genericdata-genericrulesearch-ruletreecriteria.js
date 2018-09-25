'use strict';
app.directive('vrGenericdataGenericrulesearchRuletreecriteria', ['UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VRUIUtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VR_GenericData_GenericUIService', 'VR_GenericData_GenericRule',
    function (UtilsService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VR_GenericData_GenericUIService, VR_GenericData_GenericRule) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                advancedselected: '=',
                basicselected: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new searchRuleTreeCriteriaCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ruleTreeCriteriaCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Search/Templates/SearchRuleTreeCriteriaTemplate.html';
            }
        };

        function searchRuleTreeCriteriaCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var fieldTypes;
            var ruleDefinition;
            var genericUIObj;

            var settingsFilterDirectiveAPI;
            var settingsFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.filters = [];


                $scope.onSettingsFilterDirectiveReady = function (api) {
                    settingsFilterDirectiveAPI = api;
                    settingsFilterDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    $scope.scopeModel.filters.length = 0;
                    $scope.scopeModel.settingsFilterEditor = null;

                    ruleDefinition = payload.ruleDefinition;
                    genericUIObj = VR_GenericData_GenericUIService.createGenericUIObj(ruleDefinition.CriteriaDefinition.Fields);

                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();

                    var loadFilterSettingsPromise = loadFilterSettings();
                    promises.push(loadFilterSettingsPromise);

                    var loadSettingsFilterPromise = loadSettingsFilter();
                    promises.push(loadSettingsFilterPromise);


                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypes = [];
                            for (var i = 0; i < response.length; i++) {
                                fieldTypes.push(response[i]);
                            }
                        });
                    };

                    function loadFilterSettings() {
                        var loadFilterSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                        getFieldTypeConfigsPromise.then(function () {
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
                                loadFilterSettingsLoadDeferred.resolve();
                            });
                        }).catch(function (error) {
                            loadFilterSettingsLoadDeferred.reject(error);
                        });

                        return loadFilterSettingsLoadDeferred.promise;
                    };

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

                            criteriaField.genericUIContext = genericUIObj.getFieldContext(criteriaField);

                            var directivePayload = {
                                fieldTitle: criteriaField.Title,
                                fieldType: criteriaField.FieldType,
                                genericUIContext: criteriaField.genericUIContext
                            };


                            VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                        };

                        return filter;
                    };

                    function loadSettingsFilter() {
                        var loadSettingsFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        var getRuleTypesPromise = getRuleTypes();
                        getRuleTypesPromise.then(function () {
                            var settingsPromises = [];
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
                                }, 1);
                            }

                            UtilsService.waitMultiplePromises(settingsPromises).then(function () {
                                loadSettingsFilterLoadDeferred.resolve();
                            });
                        });
                        return loadSettingsFilterLoadDeferred.promise;
                    };

                    function getRuleTypes() {
                        return VR_GenericData_GenericRuleTypeConfigAPIService.GetGenericRuleTypes().then(function (response) {
                            var ruleTypes = [];
                            for (var i = 0; i < response.length; i++) {
                                ruleTypes.push(response[i]);
                            }
                            $scope.scopeModel.settingsFilterEditor = UtilsService.getItemByVal(ruleTypes, ruleDefinition.SettingsDefinition.ConfigId, 'ExtensionConfigurationId').FilterEditor;
                        });
                    };

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        genericUIObj.loadingFinish();
                    });
                };

                api.addGenericRule = function (addRuleObj) {
                    VR_GenericData_GenericRule.addGenericRule(ruleDefinition.GenericRuleDefinitionId, addRuleObj.onRuleAdded);
                };

                api.uploadGenericRules = function (uploadRulesObj) {
                    VR_GenericData_GenericRule.uploadGenericRules(ruleDefinition.GenericRuleDefinitionId, uploadRulesObj.context, undefined, undefined);
                };

                api.getSearchObject = function () {
                    var gridQuery = {};

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
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }]);