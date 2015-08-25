HourlyReportController.$inject = ['$scope', 'UtilsService', 'HourlyReportAPIService', '$q', 'BusinessEntityAPIService_temp', 'HourlyReportMeasureEnum',
        'CarrierTypeEnum', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'AnalyticsService', 'ZonesService', 'ChartTypeEnum', 'CarrierAccountConnectionAPIService', 'CarrierTypeEnum'];

function HourlyReportController($scope, UtilsService, HourlyReportAPIService, $q, BusinessEntityAPIService, HourlyReportMeasureEnum,
        CarrierTypeEnum, VRNotificationService, DataRetrievalResultTypeEnum, analyticsService, ZonesService, ChartTypeEnum, CarrierAccountConnectionAPIService,CarrierTypeEnum ) {

    //var chartSelectedMeasureAPI;
    var chart1SelectedEntityAPI;
    var chart2SelectedEntityAPI;
    var mainGridAPI;
    var overallSelectedMeasure;
    var measures = [];
    var currentData;
    var selectedPeriod;
    var groupKeys = analyticsService.getTrafficStatisticGroupKeys();
    defineScope();
    load();

    function defineScope() {
        $scope.groupKeys = groupKeys;
        $scope.selectedGroupKeys = analyticsService.getDefaultTrafficStatisticGroupKeys();
        $scope.chartType = UtilsService.getArrayEnum(ChartTypeEnum);
        $scope.selectedChartType = ChartTypeEnum.Bar;
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.codeGroups = [];
        $scope.selectedCodeGroups = [];
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $scope.suppliers = [];
        $scope.selectedSuppliers = [];
        $scope.zones = [];
        $scope.selectedZones = [];
        $scope.data = [];
        $scope.connections = [];
        $scope.selectedConnections = [];
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        $scope.onSelectionChanged = function () {
           
            var value;
            switch ($scope.selectedConnectionIndex) {
                case 0: $scope.selectedConnections.length = 0; $scope.connections.length = 0; return;
                case 1: value = CarrierTypeEnum.Customer.value;
                    break;
                case 2: value = CarrierTypeEnum.Supplier.value;
                    break;
            }
            return CarrierAccountConnectionAPIService.GetConnectionByCarrierType(value).then(function (response) {
                $scope.selectedConnections.length = 0;
                $scope.connections.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.connections.push(itm);
                });

            });
          //  return $scope.selectedConnectionIndex!=0;
        }
        $scope.selectedConnectionIndex;
        $scope.periods = analyticsService.getPeriods();
        $scope.selectedPeriod = $scope.periods[0];
        $scope.periodSelectionChanged = function () {
            if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                var date = $scope.selectedPeriod.getInterval();
                $scope.fromDate = date.from;
                $scope.toDate = date.to;
            }

        }
        $scope.customvalidateTestFrom = function (fromDate) {
            return UtilsService.validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTestTo = function (toDate) {
            return UtilsService.validateDates($scope.fromDate, toDate);
        };

        $scope.showResult = false;       
        $scope.gridAllMeasuresScope = {};
        $scope.measures = measures;
        defineMenuActions();
        $scope.currentSearchCriteria = {
            groupKeys: []
        };
        $scope.onValueChanged = function () {
            if ($scope.selectedPeriod != selectedPeriod) {
                var customize = {
                    value: -1,
                    description: "Customize"
                }
                selectedPeriod = $scope.selectedPeriod;
                $scope.selectedPeriod = customize;
            }
        }
              
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return HourlyReportAPIService.GetHourlyReportData(dataRetrievalInput).then(function (response) {
                if (dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultTypeEnum.Normal.value) {
                    currentData = [];
                    angular.forEach(response.Data, function (itm) {
                        currentData.push(itm);
                    });
                    if (response.Summary) {
                        $scope.trafficStatisticSummary = response.Summary;
                    }
                    mainGridAPI.setSummary(response.Summary);
                   // renderOverallChart();
                    showEntityStatisticsChart1(response);
                    if (chart2SelectedEntityAPI != undefined)
                    showEntityStatisticsChart2(response.Data, HourlyReportMeasureEnum.ASR);
                }
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.asrSelected;
        $scope.acdSelected;
        $scope.onChartSelectionChanged = function () {
            if ($scope.asrSelected)
                showEntityStatisticsChart2(currentData, HourlyReportMeasureEnum.ASR);
            else if($scope.acdSelected)
                showEntityStatisticsChart2(currentData, HourlyReportMeasureEnum.ACD);
        }
        //$scope.chartSelectedMeasureReady = function (api) {
        //    chartSelectedMeasureAPI = api;
        //};
        $scope.customvalidateSelectGroup = function () {
        };
        $scope.chart1SelectedEntityReady = function (api) {
            chart1SelectedEntityAPI = api;

        };
        $scope.chart2SelectedEntityReady = function (api) {
            chart2SelectedEntityAPI = api;
            if( $scope.showResult)
                showEntityStatisticsChart2(currentData, HourlyReportMeasureEnum.ASR);

        };
        $scope.searchClicked = function () {
            $scope.currentSearchCriteria.groupKeys.length = 0;
            angular.forEach($scope.selectedGroupKeys, function (group) {
                $scope.currentSearchCriteria.groupKeys.push(group);
            });
            if (mainGridAPI != undefined)
                return retrieveData(true);
        };
        $scope.onGroupKeyClicked = function (dataItem, colDef) {
            var group = colDef.tag;
            var groupIndex = $scope.currentSearchCriteria.groupKeys.indexOf(group);
            if (dataItem.GroupKeyValues[groupIndex].Id != "N/A" || dataItem.GroupKeyValues[groupIndex].Id != null)
                $scope.selectEntity(group, dataItem.GroupKeyValues[groupIndex].Id, dataItem.GroupKeyValues[groupIndex].Name);
        };
        $scope.selectEntity = function (groupKey, entityId, entityName) {
            $scope.selectedEntityType = groupKey.description;
            $scope.selectedEntityId = entityId;
            $scope.selectedEntityName = entityName;
            getAndShowEntityStatistics(groupKey);
        };
        $scope.isOverallItemClickable = function (dataItem) {
            return (dataItem.measure.isSum == true);
        };
        $scope.onOverallItemClicked = function (dataItem) {
            overallSelectedMeasure = dataItem.measure;
            renderOverallChart();
        };
        $scope.getChartMenuActions = function (dataItem) {
            var menuActions = [];
            angular.forEach($scope.currentSearchCriteria.groupKeys, function (groupKey) {
                var valueIndex = $scope.currentSearchCriteria.groupKeys.indexOf(groupKey);
                if (dataItem.groupKeyValues != undefined && dataItem.groupKeyValues[valueIndex].Name != null) {
                    menuActions.push({
                        name: groupKey.description + ' (' + dataItem.groupKeyValues[valueIndex].Name + ')',
                        clicked: function (dataItem) {
                            $scope.selectEntity(groupKey, dataItem.groupKeyValues[valueIndex].Id, dataItem.groupKeyValues[valueIndex].Name)
                        }
                    });
                }
            });
            return menuActions;
        };

    }

    function retrieveData(withSummary) {
        //if (!chartSelectedMeasureAPI)
        //    return;
        if (chart1SelectedEntityAPI)
            chart1SelectedEntityAPI.hideChart();
        if (chart2SelectedEntityAPI)
            chart2SelectedEntityAPI.hideChart();
        var filter = buildFilter();
        var groupKeys = [];

        angular.forEach($scope.selectedGroupKeys, function (group) {
            groupKeys.push(group.value);
        });
        var connectionList=[];
        var carrierType;
        
        switch ($scope.selectedConnectionIndex) {
            case 1: carrierType = CarrierTypeEnum.Customer.value; connectionList = UtilsService.getPropValuesFromArray($scope.selectedConnections, "Value");

                break;
            case 2: carrierType = CarrierTypeEnum.Supplier.value; connectionList = UtilsService.getPropValuesFromArray($scope.selectedConnections, "Value");
                break;
        }
       
        $scope.filter = {
            filter: filter,
            groupKeys: groupKeys,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate,
        };
        var query = {
            CarrierType: carrierType,
            ConnectionList: connectionList,
            Filter: filter,
            WithSummary: withSummary,
            GroupKeys: groupKeys,
            From: $scope.fromDate,
            To: $scope.toDate,
        };
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadMeasures();
        overallSelectedMeasure = HourlyReportMeasureEnum.Attempts;
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCodeGroups]).finally(function () {
            $scope.isInitializing = false;
            if (mainGridAPI != undefined) {
                retrieveData(true);
            }
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }
    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "CDRs",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%",
                };
                var parameters = {
                    fromDate: $scope.filter.fromDate,
                    toDate: $scope.filter.toDate
                };
                analyticsService.showCdrLogModal(parameters, dataItem.GroupKeyValues, $scope.currentSearchCriteria.groupKeys);
            }
        }];
    }
    function loadMeasures() {
        for (var prop in HourlyReportMeasureEnum) {
            measures.push(HourlyReportMeasureEnum[prop]);
        }
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = analyticsService.getFilterIds($scope.selectedSwitches, "SwitchId");
        filter.CustomerIds = analyticsService.getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.SupplierIds = analyticsService.getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
        filter.CodeGroups = analyticsService.getFilterIds($scope.selectedCodeGroups, "Code");
        return filter;
    }
    function showEntityStatisticsChart1(response) {
        $scope.isGettingEntityStatistics = true;
    
            
            var chartData = [];
            for (var i = 0; i < response.Data.length; i++) {
                var values = {
                    Attempts: response.Data[i].Data.Attempts,
                    DurationsInMinutes: response.Data[i].Data.DurationsInMinutes
                }
                chartData.push(values);
            }
       
            var title = "Traffic By Hour  ";
            var seriesDefinitions = [];
            angular.forEach(measures, function (measure) {
                if (measure == HourlyReportMeasureEnum.Attempts || measure == HourlyReportMeasureEnum.DurationsInMinutes) {
                    title += " - " + measure.description + " ";
                    seriesDefinitions.push({
                        title: measure.description,
                        valuePath: measure.propertyName,
                        //selected: (measure == HourlyReportMeasureEnum.Attempts)
                    });
                }
               
            });
            var xAxisDefinition = {
                titlePath: "Attempts",
                isDateTime: true
            };
            var chartDefinition = {
                type: "spline",
                title: title,
                yAxisTitle: "Value"
            };
            chart1SelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            $scope.isGettingEntityStatistics = false;

    }
    function showEntityStatisticsChart2(response,selectedEntity) {
        $scope.isGettingEntityStatistics = true;
        var chartData = [];
        for (var i = 0; i < response.length; i++) {
            var obj={
                key:selectedEntity.propertyName,
                value:response[i].Data[selectedEntity.propertyName]
            }
           
            chartData.push(obj);
        }
        var title="Traffic By Hour  ";
        var xAxisDefinition = {
            titlePath: "value",
            isDateTime: true
        };
        var seriesDefinitions = [];
        angular.forEach(measures, function (measure) {
            if (measure == HourlyReportMeasureEnum[selectedEntity.propertyName]) {
                title +=" - "+ measure.description + " ";
                seriesDefinitions.push({
                    title: measure.description,
                    valuePath: "value",
                    //selected: (measure == HourlyReportMeasureEnum.Attempts)
                });
            }
              
        });
        var chartDefinition = {
            type: "spline",
            title: title,
            yAxisTitle: "Value"
        };
        chart2SelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
        $scope.isGettingEntityStatistics = false;
    }
    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
            });
        });
    }

    function loadCodeGroups() {
        return BusinessEntityAPIService.GetCodeGroups().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.codeGroups.push(itm);
            });
        });

    }

};

appControllers.controller('Analytics_HourlyReportController', HourlyReportController);