(function (app) {

    'use strict';

    HistoryAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_GenericData_DataRecordTypeService', 'ColumnWidthEnum', 'PeriodEnum', 'VRDateTimeService'];

    function HistoryAnalyticReportDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldAPIService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticTypeEnum, VR_GenericData_DataRecordTypeService, ColumnWidthEnum, PeriodEnum, VRDateTimeService) {
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
            var filterObj;

            var dimensions = [];
            var measures = [];
            var settings;

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var timeRangeDirectiveAPI;
            var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.widgets = [];
                $scope.scopeModel.filters = [];
                $scope.scopeModel.legends = [];
                $scope.scopeModel.groupingDimentions = [];
                $scope.scopeModel.selectedGroupingDimentions = [];
                $scope.scopeModel.isGroupingRequired = false;
                $scope.scopeModel.fromdate = VRDateTimeService.getNowDateTime();
                $scope.scopeModel.todate;

                $scope.scopeModel.onTimeRangeDirectiveReady = function (api) {
                    timeRangeDirectiveAPI = api;
                    timeRangeReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.addFilter = function () {
                    var onDimensionFilterAdded = function (filter, expression) {
                        filterObj = filter;
                        $scope.scopeModel.expression = expression;
                    };
                    var fields = [];
                    for (var i = 0; i < dimensions.length; i++) {
                        var dimension = dimensions[i];

                        fields.push({
                            FieldName: dimension.Name,
                            FieldTitle: dimension.Title,
                            Type: dimension.Config.FieldType,
                        });
                    }
                    VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, filterObj, onDimensionFilterAdded);
                };

                $scope.scopeModel.resetFilter = function () {
                    $scope.scopeModel.expression = undefined;
                    filterObj = null;
                };

                $scope.search = function () {
                    if (!$scope.scopeModel.isLoadedData) {
                        $scope.scopeModel.isLoadedData = true;
                        return loadWidgets();
                    } else {
                        if ($scope.scopeModel.widgets.length > 0) {
                            var promiseDeffer = UtilsService.createPromiseDeferred();
                            for (var i = 0; i < $scope.scopeModel.widgets.length ; i++) {
                                var widget = $scope.scopeModel.widgets[i];
                                var setLoader = function (value) { $scope.scopeModel.isLoadingDimensionDirective = value, !value ? promiseDeffer.resolve() : undefined };
                                var payload = getQuery(widget.settings);
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, widget.directiveAPI, payload, setLoader);
                            }
                            return promiseDeffer.promise;
                        }
                    }

                };

                defineColumnWidth();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    var loadPromiseDeffer = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultipleAsyncOperations([getWidgetsTemplateConfigs, getFieldTypeConfigs, loadMeasures, loadDimensions, loadTimeRangeDirective]).then(function () {
                        UtilsService.waitMultipleAsyncOperations([loadFilters]).finally(function () {
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

            function defineColumnWidth() {
                $scope.scopeModel.columnWidth = [];
                for (var td in ColumnWidthEnum)
                    $scope.scopeModel.columnWidth.push(ColumnWidthEnum[td]);
                $scope.scopeModel.selectedColumnWidth = $scope.scopeModel.columnWidth[0];
            }

            function loadTimeRangeDirective() {
                var loadTimeDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                timeRangeReadyPromiseDeferred.promise.then(function () {
                    var timeRangePeriod = {
                        period: PeriodEnum.Today.value
                    };

                    VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, timeRangePeriod, loadTimeDimentionPromiseDeferred);

                });
                return loadTimeDimentionPromiseDeferred.promise;
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
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    fieldTypes.length = 0;
                    for (var i = 0; i < response.length; i++) {
                        fieldTypes.push(response[i]);
                    }
                });
            }

            function loadFilters() {
                var filterPromises = [];

                if (settings == undefined || settings.SearchSettings == undefined)
                    return filterPromises;

                $scope.scopeModel.isGroupingRequired = settings.SearchSettings.IsRequiredGroupingDimensions;

                if (settings.SearchSettings.Legends != undefined) {
                    $scope.scopeModel.legends = settings.SearchSettings.Legends;
                }

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
                        var dimension = UtilsService.getItemByVal(dimensions, groupingDimention.DimensionName, "Name");
                        if (dimension != undefined && dimension.Config.RequiredParentDimension == undefined) {
                            var dimensionObj = {
                                DimensionName: groupingDimention.DimensionName,
                                IsSelected: groupingDimention.IsSelected,
                                Title: dimension.Title
                            };
                            $scope.scopeModel.groupingDimentions.push(dimensionObj);
                            if (groupingDimention.IsSelected) {
                                $scope.scopeModel.selectedGroupingDimentions.push(dimensionObj);
                            }
                        }
                    }
                }

                if (settings.SearchSettings.ShowCurrency) {
                    $scope.scopeModel.showCurrency = true;
                    var loadCurrencySelectorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    currencySelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, undefined, loadCurrencySelectorDirectivePromiseDeferred);
                    });
                    filterPromises.push(loadCurrencySelectorDirectivePromiseDeferred.promise);
                }

                function getFilter(filterConfiguration) {
                    var dimension = UtilsService.getItemByVal(dimensions, filterConfiguration.DimensionName, 'Name');
                    var filter;
                    var filterEditor;
                    var fieldType;
                    if (dimension != undefined) {
                        fieldType = UtilsService.getItemByVal(fieldTypes, dimension.Config.FieldType.ConfigId, 'ExtensionConfigurationId');
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
                    //filter.title = filterConfiguration.Title;
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

                return UtilsService.waitMultiplePromises(filterPromises);
            }

            function loadMeasures() {
                var input = {
                    TableIds: settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Measure.value,
                };
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    measures = response;
                });
            }

            function loadDimensions() {
                var input = {
                    TableIds: settings.AnalyticTableIds,
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                };
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
                        settings: widgetItem.payload,
                        columnWidth: UtilsService.getItemByVal($scope.scopeModel.columnWidth, widgetItem.payload.ColumnWidth, "value")
                    };
                    var dataItemPayload = getQuery(widgetItem.payload);

                    widget.onDirectiveReady = function (api) {

                        widget.directiveAPI = api;
                        widgetItem.readyPromiseDeferred.resolve();
                    };

                    widgetItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(widget.directiveAPI, dataItemPayload, widgetItem.loadPromiseDeferred);
                        });
                    $scope.scopeModel.widgets.push(widget);
                }

                return UtilsService.waitMultiplePromises(promises);

            }

            function getQuery(widgetPayload) {
                var dimensionFilters = [];
                if ($scope.scopeModel.filters != undefined) {
                    for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                        var filter = $scope.scopeModel.filters[i];
                        if (filter.directiveAPI != undefined && filter.directiveAPI.getData() != undefined) {
                            dimensionFilters.push({
                                Dimension: filter.dimesnionName,
                                FilterValues: filter.directiveAPI.getValuesAsArray()
                            });
                        }

                    }
                }

                var groupingDimensions = [];
                if ($scope.scopeModel.selectedGroupingDimentions != undefined && $scope.scopeModel.selectedGroupingDimentions.length > 0) {
                    for (var i = 0; i < $scope.scopeModel.selectedGroupingDimentions.length; i++) {
                        var dimension = $scope.scopeModel.selectedGroupingDimentions[i];
                        groupingDimensions.push({ DimensionName: dimension.DimensionName });

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
                    FilterGroup: buildFilterGroupObj(filterObj, widgetPayload.RecordFilter),
                    ToTime: $scope.scopeModel.todate,
                    Period: $scope.selectedPeriod.value,
                    CurrencyId: currencySelectorAPI != undefined?currencySelectorAPI.getSelectedIds():undefined
                };
                return query;
            };

            function buildFilterGroupObj(filterObj, widgetRecordFilter) {
                if (widgetRecordFilter == undefined)
                    return filterObj;

                if (filterObj == undefined)
                    return widgetRecordFilter;

                return {
                    $type: 'Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities',
                    LogicalOperator: 0,
                    Filters: [filterObj, widgetRecordFilter]
                };
            };
        }
    }

    app.directive('vrAnalyticAnalyticreportHistoryRuntime', HistoryAnalyticReportDirective);
})(app);