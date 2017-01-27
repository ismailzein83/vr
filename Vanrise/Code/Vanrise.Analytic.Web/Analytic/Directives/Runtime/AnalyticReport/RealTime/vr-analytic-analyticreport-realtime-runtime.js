(function (app) {

    'use strict';

    RealtimeAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VRTimerService', 'VR_Analytic_TimeGroupingUnitEnum', 'VR_GenericData_DataRecordTypeService', 'VR_Analytic_SinceTimeEnum', 'ColumnWidthEnum', 'VR_Analytic_TimeUnitEnum'];

    function RealtimeAnalyticReportDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldAPIService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticTypeEnum, VRTimerService, VR_Analytic_TimeGroupingUnitEnum, VR_GenericData_DataRecordTypeService, VR_Analytic_SinceTimeEnum, ColumnWidthEnum, VR_Analytic_TimeUnitEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var realTimeAnalyticReport = new RealTimeAnalyticReport($scope, ctrl, $attrs);
                realTimeAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RealTime/Templates/RealTimeAnalyticReportRuntimeTemplates.html"

        };
        function RealTimeAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var filterObj;
            var job;
            var fieldTypes = [];
            var dimensions = [];
            var measures = [];
            var settings;
            var currentFromDate = new Date();
            currentFromDate.setHours(0, 0, 0, 0);
            var currentToDate = new Date();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.timeGroupingUnits = UtilsService.getArrayEnum(VR_Analytic_TimeGroupingUnitEnum);
                $scope.scopeModel.sinceTimes = UtilsService.getArrayEnum(VR_Analytic_SinceTimeEnum);


                $scope.scopeModel.timeUnits = UtilsService.getArrayEnum(VR_Analytic_TimeUnitEnum);

                
                $scope.scopeModel.selectedSinceTime = VR_Analytic_SinceTimeEnum.Time;
                $scope.scopeModel.showSinceTime = true;
                $scope.scopeModel.selectedTimeGroupingUnit = VR_Analytic_TimeGroupingUnitEnum.Hour;
                $scope.scopeModel.selectedTimeUnit = VR_Analytic_TimeUnitEnum.Days;
                $scope.scopeModel.last = 1;
                $scope.scopeModel.onSinceSelectionChanged = function () {

                    if ($scope.scopeModel.selectedSinceTime != undefined) {
                        switch ($scope.scopeModel.selectedSinceTime.value) {
                            case VR_Analytic_SinceTimeEnum.Time.value: $scope.scopeModel.showSinceLast = false; $scope.scopeModel.showSinceTime = true; break;
                            case VR_Analytic_SinceTimeEnum.Last.value: $scope.scopeModel.showSinceLast = true; $scope.scopeModel.showSinceTime = false; break;
                        }
                    }
                };
                $scope.scopeModel.validateDateTime =function()
                {
                    if ($scope.scopeModel.fromdate > new Date())
                        return "The date should not be greater than date of today.";
                    return null;
                }

                $scope.scopeModel.addFilter = function () {
                    var onFilterAdded = function (filter, expression) {
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
                    VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, filterObj, onFilterAdded);
                };
                $scope.scopeModel.resetFilter = function () {
                    $scope.scopeModel.expression = undefined;
                    filterObj = null;
                };


                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.widgets = [];
                $scope.scopeModel.filters = [];
                $scope.scopeModel.fromdate = new Date();
                $scope.scopeModel.fromdate.setHours(0, 0, 0, 0);
                $scope.scopeModel.search = function () {
                    if ($scope.scopeModel.selectedSinceTime != undefined) {
                        switch ($scope.scopeModel.selectedSinceTime.value) {
                            case VR_Analytic_SinceTimeEnum.Time.value:
                                currentFromDate = $scope.scopeModel.fromdate;
                                currentToDate = new Date();
                                break;
                            case VR_Analytic_SinceTimeEnum.Last.value:

                                if ($scope.scopeModel.selectedTimeUnit != undefined) {
                                    currentFromDate = new Date();
                                    switch ($scope.scopeModel.selectedTimeUnit.value) {
                                        case VR_Analytic_TimeUnitEnum.Days.value:
                                            currentFromDate.setDate(currentFromDate.getDate() - $scope.scopeModel.last);
                                            break;

                                        case VR_Analytic_TimeUnitEnum.Hours.value:
                                            currentFromDate.setHours(currentFromDate.getHours() - $scope.scopeModel.last);
                                            break;
                                    }
                                }

                                currentToDate = new Date();
                                break;
                        }
                    }

                    if (job != undefined)
                        VRTimerService.unregisterJob(job);
                    job = VRTimerService.registerJob(search, $scope, $scope.scopeModel.timeInterval * 60);
                };
               
                defineColumnWidth();
                defineAPI();
            }

            function search() {
                var promises = [];
                if ($scope.scopeModel.widgets.length > 0) {
                  
                
                    for (var i = 0; i < $scope.scopeModel.widgets.length ; i++) {
                        var widget = $scope.scopeModel.widgets[i];
                        widget.promiseDeffer = UtilsService.createPromiseDeferred();
                        promises.push(widget.promiseDeffer.promise);
                        loadWidgetDirective(widget)
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            }
            function loadWidgetDirective(widget)
            {
                var setLoader = function (value) { $scope.scopeModel.isLoadingDimensionDirective = value, !value ? widget.promiseDeffer.resolve() : undefined; };
                var payload = getQuery(widget.settings);
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, widget.directiveAPI, payload, setLoader);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload.settings;
                        $scope.scopeModel.timeInterval = settings.SearchSettings.TimeIntervalInMin;
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
                return VR_Analytic_AnalyticConfigurationAPIService.GetRealTimeWidgetsTemplateConfigs().then(function (response) {

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

                return UtilsService.waitMultiplePromises(filterPromises);

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

                            // widget.directiveAPI.loadGrid(dataItemPayload);
                            //  widgetItem.loadPromiseDeferred.resolve();

                            VRUIUtilsService.callDirectiveLoad(widget.directiveAPI, dataItemPayload, widgetItem.loadPromiseDeferred);
                        });
                    $scope.scopeModel.widgets.push(widget);
                }



            }
            function defineColumnWidth() {
                $scope.scopeModel.columnWidth = [];
                for (var td in ColumnWidthEnum)
                    $scope.scopeModel.columnWidth.push(ColumnWidthEnum[td]);
                $scope.scopeModel.selectedColumnWidth = $scope.scopeModel.columnWidth[0];
            }
            function getQuery(widgetPayload) {
                var dimensionFilters = [];
                //if ($scope.scopeModel.filters != undefined) {
                //    for (var i = 0; i < $scope.scopeModel.filters.length; i++) {
                //        var filter = $scope.scopeModel.filters[i];
                //        if (filter.directiveAPI != undefined && filter.directiveAPI.getData() != undefined) {
                //            dimensionFilters.push({
                //                Dimension: filter.dimesnionName,
                //                FilterValues: filter.directiveAPI.getValuesAsArray()
                //            })
                //        }

                //    }
                //}
                var query = {
                    Measures: measures,
                    Settings: widgetPayload,
                    DimensionFilters: dimensionFilters,
                    TableId: widgetPayload.AnalyticTableId,
                    FromTime: currentFromDate,
                    ToTime: currentToDate,
                    FilterGroup: filterObj,
                    TimeGroupingUnit: $scope.scopeModel.selectedTimeGroupingUnit != undefined ? $scope.scopeModel.selectedTimeGroupingUnit.value : undefined
                };
                return query;
            }
        }

    }
    app.directive('vrAnalyticAnalyticreportRealtimeRuntime', RealtimeAnalyticReportDirective);
})(app);