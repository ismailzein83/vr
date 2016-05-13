﻿(function (app) {

    'use strict';

    HistoryAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Analytic_AnalyticService','VR_Analytic_AnalyticConfigurationAPIService','VR_GenericData_DataRecordFieldTypeConfigAPIService','VR_Analytic_AnalyticItemConfigAPIService','VR_Analytic_AnalyticTypeEnum'];

    function HistoryAnalyticReportDirective(UtilsService, VRUIUtilsService, Analytic_AnalyticService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var historyAnalyticReport = new HistoryAnalyticReport($scope, ctrl, $attrs);
                historyAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/History/Templates/HistoryAnalyticReportRuntimeTemplates.html"

        };
        function HistoryAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var fieldTypes = [];
            var dimensions = [];
            var measures = [];
            var settings;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.widgets = [];
                $scope.scopeModel.filters = [];
                $scope.scopeModel.fromdate = "01/01/2015";
                $scope.scopeModel.todate = new Date();
                $scope.scopeModel.groupingDimentions = [];
                $scope.scopeModel.selectedGroupingDimentions = [];
                $scope.scopeModel.isGroupingRequired = false;

                $scope.search = function () {
                    if ($scope.scopeModel.widgets.length > 0) {
                        var promiseDeffer = UtilsService.createPromiseDeferred();
                        for (var i = 0; i < $scope.scopeModel.widgets.length ; i++) {
                            var widget = $scope.scopeModel.widgets[i];
                            var setLoader = function (value) { $scope.isLoadingDimensionDirective = value, !value? promiseDeffer.resolve():undefined };
                            var payload = getQuery(widget.settings);
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, widget.directiveAPI, payload, setLoader);
                        }
                        return promiseDeffer.promise;
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                    {
                        settings = payload.settings;
                    }
                    var loadPromiseDeffer = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultipleAsyncOperations([getWidgetsTemplateConfigs, getFieldTypeConfigs, loadMeasures, loadDimensions]).then(function () {
                        UtilsService.waitMultipleAsyncOperations([loadFilters, loadWidgets]).finally(function () {
                            loadPromiseDeffer.resolve();
                        }).catch(function (error) {
                            loadPromiseDeffer.reject(error);
                        });
                    }).catch(function (error) {
                        loadPromiseDeffer.reject(error);
                    });
                    return loadPromiseDeffer.promise;
                };

              
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }

            function getWidgetsTemplateConfigs() {
                return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                   
                    if (response != null) {
                        
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                    }
                });
            }

            function getFieldTypeConfigs() {
                return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                    fieldTypes.length = 0;
                    for (var i = 0; i < response.length; i++) {
                        fieldTypes.push(response[i]);
                    }
                });
            }

            function loadFilters() {
                var filterPromises = [];
                $scope.scopeModel.isGroupingRequired = settings.SearchSettings.IsRequiredGroupingDimensions;
                if (settings.SearchSettings.Filters != undefined) {
                    for (var i = 0; i < settings.SearchSettings.Filters.length; i++) {
                        var filterConfiguration = settings.SearchSettings.Filters[i];
                        var filter = getFilter(filterConfiguration);
                        if (filter != undefined) {
                            filterPromises.push(filter.directiveLoadDeferred.promise);
                            $scope.scopeModel.filters.push(filter);
                        }
                    }
                }

                if (settings.SearchSettings.GroupingDimensions != undefined) {
                    for (var i = 0; i < settings.SearchSettings.GroupingDimensions.length; i++) {
                        var groupingDimention = settings.SearchSettings.GroupingDimensions[i];
                        $scope.scopeModel.groupingDimentions.push(groupingDimention);
                        if (groupingDimention.IsSelected) {
                            $scope.scopeModel.selectedGroupingDimentions.push(groupingDimention);
                        }
                    }
                }

                return UtilsService.waitMultiplePromises(filterPromises);

                function getFilter(filterConfiguration) {
                    var dimension = UtilsService.getItemByVal(dimensions, filterConfiguration.DimensionName, 'Name');
                    var filter;
                    var filterEditor;
                    var fieldType;
                    if (dimension != undefined) {
                        fieldType = UtilsService.getItemByVal(fieldTypes, dimension.Config.FieldType.ConfigId, 'DataRecordFieldTypeConfigId');
                    }
                    if (fieldType != undefined) {
                        filterEditor = fieldType.FilterEditor;
                    }
                    if (filterEditor == null) return filter;

                    filter = {};
                    filter.dimesnionName = filterConfiguration.DimensionName;
                    filter.isRequired = filterConfiguration.IsRequired;
                    filter.directiveEditor = filterEditor;
                    filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    filter.onDirectiveReady = function (api) {
                        filter.directiveAPI = api;
                        var directivePayload = {
                            fieldTitle: filterConfiguration.Title,
                            fieldType: dimension != undefined ? dimension.Config.FieldType : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(api, directivePayload, filter.directiveLoadDeferred);
                    };

                    return filter;
                }
            }

            function loadMeasures() {
                var input = {
                    TableIds: settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                }
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    measures = response;
                });
            }

            function loadDimensions() {
                var input = {
                    TableIds: settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                }
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
            }

            function loadWidgets() {
                var promises = [];
                for (var i = 0; i < settings.Widgets.length; i++) {
                    var widgetItem = {
                        payload: settings.Widgets[i],
                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                    };
                    promises.push(widgetItem.loadPromiseDeferred.promise);
                    addWidget(widgetItem);
                }

                function addWidget(widgetItem) {
                    var matchItem = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, widgetItem.payload.ConfigId, "ExtensionConfigurationId");
                    
                    if (matchItem == null)
                        return;
                    var widget = {
                        id: $scope.scopeModel.widgets.length + 1,
                        configId: matchItem.ExtensionConfigurationId,
                        runtimeEditor: matchItem.RuntimeEditor,
                        name: widgetItem.WidgetTitle,
                        settings: widgetItem.payload
                    };
                    var dataItemPayload = getQuery(widgetItem.payload);

                    widget.onDirectiveReady = function (api) {

                        widget.directiveAPI = api;
                        widgetItem.readyPromiseDeferred.resolve();
                    };

                    widgetItem.readyPromiseDeferred.promise
                        .then(function () {

                            // widget.directiveAPI.loadGrid(dataItemPayload);
                            //  widgetItem.loadPromiseDeferred.resolve();

                            VRUIUtilsService.callDirectiveLoad(widget.directiveAPI, dataItemPayload, widgetItem.loadPromiseDeferred);
                        });
                    $scope.scopeModel.widgets.push(widget);
                }



            }

            function getQuery(widgetPayload) {
                var dimensionFilters = [];
                if ($scope.scopeModel.filters != undefined) {
                    for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                        var filter = $scope.scopeModel.filters[i];
                        if (filter.directiveAPI !=undefined && filter.directiveAPI.getData() != undefined) {
                            dimensionFilters.push({
                                Dimension: filter.dimesnionName,
                                FilterValues: filter.directiveAPI.getValuesAsArray()
                            })
                        }

                    }
                }


                var groupingDimensions = [];

                if ($scope.scopeModel.selectedGroupingDimentions != undefined && $scope.scopeModel.selectedGroupingDimentions.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.selectedGroupingDimentions.length; i++) {
                        groupingDimensions.push({ DimensionName: $scope.scopeModel.selectedGroupingDimentions[i].DimensionName });

                    }
                } else {
                    if (settings.SearchSettings.GroupingDimensions != undefined) {
                        for (var i = 0; i < settings.SearchSettings.GroupingDimensions.length; i++) {
                            var groupDimension = settings.SearchSettings.GroupingDimensions[i];
                            if (groupDimension.IsSelected) {
                                groupingDimensions.push({ DimensionName: groupDimension.DimensionName });
                            }
                        }
                    }

                }
                var query = {
                    Settings: widgetPayload,
                    DimensionFilters: dimensionFilters,
                    SelectedGroupingDimensions: groupingDimensions,
                    TableId: widgetPayload.AnalyticTableId,
                    FromTime: $scope.scopeModel.fromdate,
                    ToTime: $scope.scopeModel.todate
                };
                return query;
            }
        }

    }
    app.directive('vrAnalyticAnalyticreportHistoryRuntime', HistoryAnalyticReportDirective);
})(app);