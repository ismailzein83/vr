HourlyReportController.$inject = ['$scope', 'UtilsService', 'HourlyReportAPIService', 'uiGridConstants', '$q', 'BusinessEntityAPIService_temp', 'CarrierAccountAPIService', 'TrafficStatisticGroupKeysEnum', 'HourlyReportMeasureEnum', 'LabelColorsEnum',
        'CarrierTypeEnum', 'VRModalService', 'VRNotificationService', 'DataRetrievalResultTypeEnum', 'PeriodEnum', 'AnalyticsService','ZonesService','ChartTypeEnum'];

function HourlyReportController($scope, UtilsService, HourlyReportAPIService, uiGridConstants, $q, BusinessEntityAPIService, CarrierAccountAPIService, TrafficStatisticGroupKeysEnum, HourlyReportMeasureEnum, LabelColorsEnum,
        CarrierTypeEnum, VRModalService, VRNotificationService, DataRetrievalResultTypeEnum, PeriodEnum, analyticsService, ZonesService,ChartTypeEnum) {

    var chartSelectedMeasureAPI;
    var chartSelectedEntityAPI;
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
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        $scope.onSelectionChanged = function () {

            return $scope.selectedConnectionIndex!=0;
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
            console.log($scope.selectedPeriod);
            if ($scope.selectedPeriod != selectedPeriod) {
                var customize = {
                    value: -1,
                    description: "Customize"
                }
                selectedPeriod = $scope.selectedPeriod;
                $scope.selectedPeriod = customize;
            }
        }
        
       
        $scope.getColor = function (dataItem, coldef) {
            if (coldef.tag.value == HourlyReportMeasureEnum.ACD.value)
                return getACDColor(dataItem.ACD, dataItem.Attempts);
            else if (coldef.tag.value == HourlyReportMeasureEnum.ASR.value)
                return getASRColor(dataItem.ASR, dataItem.Attempts);
        }
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return HourlyReportAPIService.GetHourlyReportData(dataRetrievalInput).then(function (response) {
                console.log(response);
                if (dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultTypeEnum.Normal.value) {
                    currentData = [];
                    angular.forEach(response.Data, function (itm) {
                        currentData.push(itm);
                    });
                    if (response.Summary) {
                        $scope.trafficStatisticSummary = response.Summary;
                    }
                    mainGridAPI.setSummary(response.Summary);
                  //  renderOverallChart();
                }
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.chartSelectedMeasureReady = function (api) {
            chartSelectedMeasureAPI = api;
        };
        $scope.customvalidateSelectGroup = function () {
        };
        $scope.chartSelectedEntityReady = function (api) {
            chartSelectedEntityAPI = api;

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
        if (!chartSelectedMeasureAPI)
            return;
        if (chartSelectedEntityAPI)
            chartSelectedEntityAPI.hideChart();
        var filter = buildFilter();
        var groupKeys = [];

        angular.forEach($scope.selectedGroupKeys, function (group) {
            groupKeys.push(group.value);
        });
        $scope.filter = {
            filter: filter,
            groupKeys: groupKeys,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate,
        };

        var query = {
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
        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadCodeGroups, loadCustomers, loadSuppliers]).finally(function () {
            $scope.isInitializing = false;
            if (mainGridAPI != undefined) {
                retrieveData(true);
            }
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function getACDColor(acdValue, attemptsValue) {
        if (attemptsValue > $scope.attempts && acdValue < $scope.acd)
            return LabelColorsEnum.WarningLevel1.Color; 
    };
    function getASRColor(asrValue, attemptsValue) {
        if (attemptsValue > $scope.attempts && asrValue < $scope.asr)
            return LabelColorsEnum.WarningLevel2.Color;
    };

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
        },
        {
            name: "Show Confirmation",
            clicked: function (dataItem) {
                VRNotificationService.showConfirmation('Are you sure you want to delete?')
                .then(function (result) {
                    if (result)
                        console.log('Confirmed');
                    else
                        console.log('not confirmed');
                });
            }
        },
        {
            name: "Show Error",
            clicked: function (dataItem) {
                VRNotificationService.showError('Error Message');
            }
        },
        {
            name: "Show Warning",
            clicked: function (dataItem) {
                VRNotificationService.showWarning('Warning Message');
            }
        },
        {
            name: "Show Success",
            clicked: function (dataItem) {
                VRNotificationService.showSuccess('Success Message');
            }
        },
        {
            name: "Show Information",
            clicked: function (dataItem) {
                VRNotificationService.showInformation('Information Message');
            }
        }];
    }

    function loadMeasures() {
        for (var prop in HourlyReportMeasureEnum) {
            measures.push(HourlyReportMeasureEnum[prop]);
        }
    }

    function renderOverallChart() {
        var chartData = [];
        var measure = overallSelectedMeasure;
        var othersValue = $scope.trafficStatisticSummary[measure.propertyName];
        var index = 1;
        angular.forEach(currentData, function (itm) {
            if (index > 15)
                return;
            index++;
            var dataItem = {
                groupKeyValues: itm.GroupKeyValues,
                entityName: '',
                value: itm[measure.propertyName]
            };

            for (var i = 0; i < $scope.currentSearchCriteria.groupKeys.length; i++) {
                if (dataItem.entityName.length > 0)
                    dataItem.entityName += ' - ';
                dataItem.entityName += itm.GroupKeyValues[i].Name;
            };
            chartData.push(dataItem);
            othersValue -= itm[measure.propertyName];
        });
        chartData.sort(function (a, b) {
            if (a.value > b.value) {
                return 1;
            }
            if (a.value < b.value) {
                return -1;
            }
            // a must be equal to b
            return 0;
        });
        chartData.push({
            entityName: "Others",
            value: othersValue
        });
        var chartDefinition = {
            type: "pie",
            title: measure.description,
            yAxisTitle: "Value"
        };

        var seriesDefinitions = [{
            title: measure.description,
            titlePath: "entityName",
            valuePath: "value"
        }];
        //console.log(chartData.length);
        chartSelectedMeasureAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
    }

    function buildFilter() {
        var filter = {};
        filter.SwitchIds = analyticsService.getFilterIds($scope.selectedSwitches, "SwitchId");
        filter.CustomerIds = analyticsService.getFilterIds($scope.selectedCustomers, "CarrierAccountID");
        filter.SupplierIds = analyticsService.getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
        filter.CodeGroups = analyticsService.getFilterIds($scope.selectedCodeGroups, "Code");
        return filter;
    }


    function getAndShowEntityStatistics(groupKey) {
        $scope.isGettingEntityStatistics = true;
        AnalyticsAPIService.GetTrafficStatistics(groupKey.value, $scope.selectedEntityId, $scope.fromDate, $scope.toDate)
        .then(function (response) {
            console.log(response);
            var chartData = response;

            var chartDefinition = {
                type: "spline",
                title: $scope.selectedEntityName
            };
            var xAxisDefinition = {
                titlePath: "Attempts",
                isDateTime: true
            };
            var seriesDefinitions = [];
            angular.forEach(measures, function (measure) {
                seriesDefinitions.push({
                    title: measure.description,
                    valuePath: measure.propertyName,
                    selected: (measure == HourlyReportMeasureEnum.Attempts || measure == HourlyReportMeasureEnum.DurationsInMinutes)
                });
            });

            chartSelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
        })
        .finally(function () {
            $scope.isGettingEntityStatistics = false;
        });;
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

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }



};

appControllers.controller('Analytics_HourlyReportController', HourlyReportController);