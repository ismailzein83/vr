"use strict";
app.directive("vrAnalyticAnalytictablequerydefinitionsettingsRuntimeeditor", ["VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService", "UtilsService", "VRUIUtilsService", "VR_Analytic_AnalyticItemConfigAPIService", "VR_Analytic_AnalyticTypeEnum", "VRNotificationService", "VR_Analytic_AnalyticTableQuerySettingsSubtablesService",
function (VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, UtilsService, VRUIUtilsService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticTypeEnum, VRNotificationService, VR_Analytic_AnalyticTableQuerySettingsSubtablesService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new RuntimeEditor($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: "ctrlrutnime",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticTableQueryDefinitionSettingsRuntimeEditorTemplate.html"
    };

    function RuntimeEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dimensionsSelectorAPI;
        var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var measuresSelectorAPI;
        var measuresSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var dimensionsDeferred = UtilsService.createPromiseDeferred();

        var analyticTableId;
        var analyticTableIdDeferred;

        var orderTypeSelectorAPI;
        var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var subtablesDirectiveAPI;
        var subtablesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        var dimensions = [];

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.hasNoExternalFilter = true;

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;
                timePeriodSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                dimensionsSelectorAPI = api;
                dimensionsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMeasureSelectorReady = function (api) {
                measuresSelectorAPI = api;
                measuresSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onOrderTypeSelectorReady = function (api) {
                orderTypeSelectorAPI = api;
                orderTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSubtablesDirectiveReady = function (api) {
                subtablesDirectiveAPI = api;
                subtablesDirectiveReadyDeferred.resolve();
            };            
            UtilsService.waitMultiplePromises([timePeriodSelectorReadyDeferred.promise, dimensionsSelectorReadyDeferred.promise, measuresSelectorReadyDeferred.promise, currencySelectorReadyDeferred.promise, recordFilterDirectiveReadyDeferred.promise, orderTypeSelectorReadyDeferred.promise, subtablesDirectiveReadyDeferred.promise]).then(function () {
              
                defineAPI();
            });
        }

        function defineAPI() {
            var api = {};
            var entity;
            var filterGroup;
            var selectedCurrencyId;
            var definitionId;

            api.load = function (payload) {
                $scope.scopeModel.withSummary = undefined;
                $scope.scopeModel.topRecords = undefined;
                analyticTableId = undefined;
                if (payload != undefined) {
                    entity = payload.runtimeDirectiveEntity;
                    definitionId = payload.definitionId;
                    context = payload.context;
                    if (context != undefined && context.hasExternalFilter != undefined) { $scope.scopeModel.hasNoExternalFilter = !context.hasExternalFilter(); }
                    if (entity != undefined) {
                        $scope.scopeModel.withSummary = entity.WithSummary;
                        $scope.scopeModel.topRecords = entity.TopRecords;
                    }
                   
                }
                function getAnalyticTableId() {
                    analyticTableIdDeferred = UtilsService.createPromiseDeferred();
                    VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService.GetVRAutomatedReportQueryDefinitionSettings(definitionId).then(function (response) {
                        if (response != undefined && response.ExtendedSettings != undefined) {
                            analyticTableId = response.ExtendedSettings.AnalyticTableId;
                            analyticTableIdDeferred.resolve();
                        }
                    });
                    return analyticTableIdDeferred.promise;
                }

                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodSelectorPayload = {
                            timePeriod: entity != undefined ? entity.TimePeriod : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodSelectorPayload, timePeriodSelectorLoadDeferred);
                    });
                    return timePeriodSelectorLoadDeferred.promise;
                }

                function loadDimensionsSelector() {
                    var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dimensionsSelectorReadyDeferred.promise.then(function () {
                        function getDimensionNames() {

                            var dimensionNames = [];
                            var dimensions = entity.Dimensions;
                            if (dimensions != undefined && dimensions.length != 0) {
                                for (var i = 0; i < dimensions.length; i++) {
                                    dimensionNames.push(dimensions[i].DimensionName);
                                }
                            }
                            return dimensionNames;
                        }
                        var dimensionsSelectorPayload = {
                            filter: 
                                {
                                    TableIds: [analyticTableId]
                                }
                            ,
                            selectedIds: entity!=undefined && entity.Dimensions!=undefined ? getDimensionNames(): undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, dimensionsSelectorPayload, dimensionsSelectorLoadDeferred);
                    });
                    return dimensionsSelectorLoadDeferred.promise;
                }

                function loadMeasuresSelector() {
                    var measuresSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    measuresSelectorReadyDeferred.promise.then(function () {
                        function getMeasureNames() {

                            var measureNames = [];
                            var measures = entity.Measures;
                            if (measures != undefined && measures.length != 0) {
                                for (var i = 0; i < measures.length; i++) {
                                    measureNames.push(measures[i].MeasureName);
                                }
                            }
                            return measureNames;
                    }
                        var measuresSelectorPayload = {
                            filter:
                                {
                                    TableIds: [analyticTableId]
                                }
                            ,
                            selectedIds: entity!=undefined && entity.Measures!=undefined ? getMeasureNames() : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, measuresSelectorPayload, measuresSelectorLoadDeferred);
                    });
                    return measuresSelectorLoadDeferred.promise;
                }

                function loadRecordFilterDirective() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {
                            context: buildContext(),
                            FilterGroup: entity!=undefined ? entity.FilterGroup : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                }

                function loadDimensions() {
                    var loadDimensionsLoadDeferred = UtilsService.createPromiseDeferred();

                    analyticTableIdDeferred.promise.then(function() {
                        var input = {
                            TableIds: [analyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                        };
                        VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                            dimensions = response;
                            dimensionsDeferred.resolve();
                            loadDimensionsLoadDeferred.resolve();
                        });
                    });
                    return loadDimensionsLoadDeferred.promise;
                }

                function loadCurrencySelector() {
                    var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    currencySelectorReadyDeferred.promise.then(function () {
                        var currencySelectorPayload = {
                        selectedIds: entity!=undefined? entity.CurrencyId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);
                    });

                    return currencySelectorLoadDeferred.promise;
                }

                function loadOrderTypeSelector() {
                    var orderTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    orderTypeSelectorReadyDeferred.promise.then(function () {
                        var orderTypeSelectorPayload = {
                            tableIds: [analyticTableId]};
                        if(entity!=undefined){
                            orderTypeSelectorPayload.orderTypeEntity = {
                                OrderType: entity.OrderType,
                                AdvancedOrderOptions: entity.AdvancedOrderOptions
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(orderTypeSelectorAPI, orderTypeSelectorPayload, orderTypeSelectorLoadDeferred);
                });
                return orderTypeSelectorLoadDeferred.promise;
                }

                function loadSubtablesDirective() {
                    var subtablesLoadDeferred = UtilsService.createPromiseDeferred();
                    subtablesDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            analyticTableId: analyticTableId,
                            subtables: entity != undefined ? entity.SubTables : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(subtablesDirectiveAPI, payload, subtablesLoadDeferred);
                    });
                    return subtablesLoadDeferred.promise;
                }
                var rootPromiseNode = {
                    promises: [getAnalyticTableId()],
                    getChildNode: function () {
                        return {
                            promises: [loadDimensions()],
                            getChildNode: function () {
                                var promises = [];
                                promises.push(loadRecordFilterDirective());

                                promises.push(loadCurrencySelector());
                                promises.push(loadDimensionsSelector());
                                promises.push(loadMeasuresSelector());
                                promises.push(loadOrderTypeSelector());
                                promises.push(loadSubtablesDirective());

                                if ($scope.scopeModel.hasNoExternalFilter)
                                    promises.push(loadTimePeriodSelector());

                                return {
                                    promises: [UtilsService.waitMultiplePromises(promises)]
                                };
                            },
                        };
                    }
                };
                return UtilsService.waitPromiseNode(rootPromiseNode);
            };

            api.getData = function () {
                var orderTypeEntity = orderTypeSelectorAPI.getData();
              
              var obj = {
                    $type: 'Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.AnalyticTableQuerySettings,Vanrise.Analytic.MainExtensions',
                    TimePeriod: getTimePeriod(),
                    Dimensions: getDimensions(),
                    Measures: getMeasures(),
                    FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    WithSummary: $scope.scopeModel.withSummary,
                    TopRecords: $scope.scopeModel.topRecords,
                    SubTables: subtablesDirectiveAPI.getData(),
                    OrderType: orderTypeEntity!=undefined ? orderTypeEntity.OrderType: undefined,
                    AdvancedOrderOptions: orderTypeEntity != undefined ? orderTypeEntity.AdvancedOrderOptions : undefined
              };
              return obj;
                
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function getTimePeriod() {
            return $scope.scopeModel.hasNoExternalFilter? timePeriodSelectorAPI.getData():undefined;
    }
        function buildContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (dimensions != undefined) {
                        for (var i = 0; i < dimensions.length; i++) {
                            var dimension = dimensions[i];
                            fields.push({
                                FieldName: dimension.Name,
                                FieldTitle: dimension.Title,
                                Type: dimension.Config.FieldType,
                            });
                        }
                    }
                    return fields;
                },
                getRuleEditor: getRuleFilterEditorByFieldType
            };
            return context;
        }
        function getRuleFilterEditorByFieldType(configId) {
            var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.dataRecordFieldTypesConfig, configId, 'ExtensionConfigurationId');
            if (dataRecordFieldTypeConfig != undefined) {
                return dataRecordFieldTypeConfig.RuleFilterEditor;
            }
        }
        

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {
                };
            return currentContext;
        }

        function getDimensions() {
            if (dimensionsSelectorAPI != undefined && dimensionsSelectorAPI.getSelectedIds != undefined && typeof (dimensionsSelectorAPI.getSelectedIds) == "function") {
                var dimensionNames = dimensionsSelectorAPI.getSelectedIds();
                var dimensions = [];
                for (var i = 0; i < dimensionNames.length; i++) {
                    dimensions.push({ DimensionName: dimensionNames[i] });
                }
                return dimensions;
            }
        }

        function getMeasures() {
            if (measuresSelectorAPI != undefined && measuresSelectorAPI.getSelectedIds != undefined && typeof (measuresSelectorAPI.getSelectedIds) == "function") {
                var measureNames = measuresSelectorAPI.getSelectedIds();
                var measures = [];
                for (var i = 0; i < measureNames.length; i++) {
                    measures.push({ MeasureName: measureNames[i] });
                }
                return measures;
            }
        }
       
    }

    return directiveDefinitionObject;
}
]);