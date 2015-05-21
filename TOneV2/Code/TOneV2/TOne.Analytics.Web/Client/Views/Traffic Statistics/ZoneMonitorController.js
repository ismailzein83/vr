ZoneMonitorController.$inject = ['$scope', 'AnalyticsAPIService', 'uiGridConstants', '$q', 'BusinessEntityAPIService', 'TrafficStatisticGroupKeysEnum', 'TrafficStatisticsMeasureEnum',
        'CarrierTypeEnum', 'VRModalService', 'VRNotificationService'];
appControllers.controller('ZoneMonitorController', ZoneMonitorController);

 function ZoneMonitorController($scope, AnalyticsAPIService, uiGridConstants, $q, BusinessEntityAPIService, TrafficStatisticGroupKeysEnum, TrafficStatisticsMeasureEnum,
        CarrierTypeEnum, VRModalService, VRNotificationService) {

        var chartSelectedMeasureAPI;
        var chartSelectedEntityAPI;
        var mainGridAPI;
        var overallSelectedMeasure;
        var resultKey;
        var sortColumn;
        var sortDescending = true;
        var currentSortedColDef;
        var measures = [];
        var currentData;

        defineScopeObjects();
        defineScopeMethods();
        load();
        function defineScopeObjects() {
            $scope.currentSearchCriteria = {
                groupKeys: []
            };
            $scope.customvalidateTestFrom = function (fromDate) {
                return validateDates(fromDate, $scope.toDate);
            };
            $scope.customvalidateTestTo = function (toDate) {
                return validateDates($scope.fromDate, toDate);
            };
            function validateDates(fromDate, toDate) {
                if (fromDate == undefined || toDate == undefined)
                    return null;
                var from = new Date(fromDate);
                var to = new Date(toDate);
                if (from.getTime() > to.getTime())
                    return "Start should be before end";
                else
                    return null;
            }
            $scope.testModel = 'ZoneMonitorController';
            
            $scope.groupKeys = [
                {
                    title: "Zone",
                    groupKeyEnumValue: TrafficStatisticGroupKeysEnum.OurZone.value,
                    gridHeader: "Zone"
                },
                {
                    title: "Customer",
                    groupKeyEnumValue: TrafficStatisticGroupKeysEnum.CustomerId.value,
                    gridHeader: "Customer"
                },
                {
                    title: "Supplier",
                    groupKeyEnumValue: TrafficStatisticGroupKeysEnum.SupplierId.value,
                    gridHeader: "Supplier"
                }];
            $scope.selectedGroupKeys = [];
            $scope.selectedGroupKeys.push($scope.groupKeys[0]);

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            $scope.customers = [];
            $scope.selectedCustomers = [];
           
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.gridAllMeasuresScope = {};
            $scope.showResult = false;
            $scope.measures = measures;
            $scope.data = [];
            $scope.overallData = [];

            $scope.mainGridPagerSettings = {
                currentPage: 1,
                totalDataCount: 0,
                pageChanged: function () {
                    getData();
                }
            };

            $scope.gridMenuActions = [{
                name: "CDRs",
                clicked: function (dataItem) {
                    VRNotificationService.showConfirmation('Are you sure you want to delete?')
                    .then(function (result) {
                        if (result)
                            console.log('Confirmed');
                        else
                            console.log('not confirmed');
                    });
                    //var modalSettings = {
                    //    useModalTemplate: true,
                    //    width: "80%",
                    //    maxHeight: "800px"
                    //};
                    //VRModalService.showModal('/Client/Modules/Analytics/Views/Traffic Statistics/ZoneMonitor.html', null, modalSettings);
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
        
        function defineScopeMethods() {

            $scope.chartSelectedMeasureReady = function (api) {
                chartSelectedMeasureAPI = api;
                //chartSelectedMeasureAPI.onDataItemClicked = function (selectedEntity) {
                //    if (selectedEntity.zoneId == undefined)
                //        return;
                //    $scope.selectedEntityId = selectedEntity.zoneId;
                //    $scope.selectedEntityName = selectedEntity.zoneName;
                //    //console.log($scope.selectedEntityName);
                //    getAndShowEntityStatistics();
                //};
            };

            $scope.chartSelectedEntityReady = function (api) {
                chartSelectedEntityAPI = api;

            };

            $scope.getData = function (asyncHandle) {
                $scope.mainGridPagerSettings.currentPage = 1;
                resultKey = null;
                mainGridAPI.reset();
                resetSorting();
                getData(asyncHandle, true);
            };

            $scope.pageChanged = function () {
                getData();
            };

            $scope.onMainGridReady = function (api) {
                mainGridAPI = api;
            }

            $scope.onGroupKeyClicked = function (dataItem, colDef) {
                var group = colDef.tag;
                var groupIndex = $scope.currentSearchCriteria.groupKeys.indexOf(group);
                $scope.selectEntity(group, dataItem.GroupKeyValues[groupIndex].Id, dataItem.GroupKeyValues[groupIndex].Name);
            };

            $scope.selectEntity = function (groupKey, entityId, entityName) {
                $scope.selectedEntityType = groupKey.title;
                $scope.selectedEntityId = entityId;
                $scope.selectedEntityName = entityName;
                getAndShowEntityStatistics(groupKey);
            };

            $scope.onMainGridSortChanged = function (colDef, sortDirection, handle) {
                sortColumn = colDef.tag;
                sortDescending = (sortDirection == "DESC");
                getData(handle);
            }

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
                            name: groupKey.title + ' (' + dataItem.groupKeyValues[valueIndex].Name + ')',
                            clicked: function (dataItem) {
                                $scope.selectEntity(groupKey, dataItem.groupKeyValues[valueIndex].Id, dataItem.groupKeyValues[valueIndex].Name)
                            }
                        });
                    }
                });
                return menuActions;
            };
        }

        function load() {
            loadMeasures();
            overallSelectedMeasure = TrafficStatisticsMeasureEnum.Attempts;
            $scope.fromDate = '2012-04-27';
            $scope.toDate = '2014-04-29';
            loadCodeGroups();
            loadSwitches();
            loadCustomers();
            loadSuppliers();
        }

        function resetSorting() {
            sortColumn = TrafficStatisticsMeasureEnum.Attempts;
            sortDescending = true;
        }

        function loadMeasures() {           
            for (var prop in TrafficStatisticsMeasureEnum) {
                measures.push(TrafficStatisticsMeasureEnum[prop]);
            }
        }
        
        function getData(asyncHandle, withSummary) {
            if (!chartSelectedMeasureAPI)
                return;
            if (sortColumn == undefined)
                return;
            if (withSummary == undefined)
                withSummary = false;
            
            
            if (chartSelectedEntityAPI)
                chartSelectedEntityAPI.hideChart();

            var count = $scope.mainGridPagerSettings.itemsPerPage;
            var groupKeys = [];
            
            angular.forEach($scope.selectedGroupKeys, function (group) {
                groupKeys.push(group.groupKeyEnumValue);
            });
           
            var fromRow = ($scope.mainGridPagerSettings.currentPage - 1) * count + 1;
            var toRow = fromRow + count - 1;
            var filter = buildFilter();
            var getTrafficStatisticSummaryInput = {
                TempTableKey: resultKey,
                Filter: filter,
                WithSummary: withSummary,
                GroupKeys: groupKeys,
                From: $scope.fromDate,
                To: $scope.toDate,
                FromRow: fromRow,
                ToRow: toRow,
                OrderBy: sortColumn.value,
                IsDescending: sortDescending
            };
            var isSucceeded;
            $scope.showResult = true;
            $scope.isGettingData = true;
            $scope.data.length = 0;
            $scope.currentSearchCriteria.groupKeys.length = 0;
            angular.forEach($scope.selectedGroupKeys, function (group) {
                $scope.currentSearchCriteria.groupKeys.push(group);
            });

            AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {

                                

                currentData = response.Data;
                if (currentSortedColDef != undefined)
                    currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';

                resultKey = response.ResultKey;
                $scope.mainGridPagerSettings.totalDataCount = response.TotalCount;
                if (withSummary) {
                    $scope.trafficStatisticSummary = response.Summary;
                    $scope.overallData[0] = response.Summary;
                }
                angular.forEach(response.Data, function (itm) {
                    $scope.data.push(itm);
                });
                renderOverallChart();
                isSucceeded = true;
            })
                .finally(function () {
                    if (asyncHandle)
                        asyncHandle.operationDone(isSucceeded);
                    $scope.isGettingData = false;
                });
        }

        function renderOverallChart(){
            var chartData = [];
            var measure = overallSelectedMeasure;
            var othersValue = $scope.trafficStatisticSummary[measure.propertyName];
            angular.forEach(currentData, function (itm) {
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
            filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
            filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
            filter.SupplierIds = getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
            filter.CodeGroups = getFilterIds($scope.selectedCodeGroups, "Code");
            return filter;
        }

        function getFilterIds(values, idProp) {
            var filterIds;
            if (values.length > 0) {
                filterIds = [];
                angular.forEach(values, function (val) {
                    filterIds.push(val[idProp]);
                });
            }
            return filterIds;
        }

        function getAndShowEntityStatistics(groupKey) {
            $scope.isGettingEntityStatistics = true;
            AnalyticsAPIService.GetTrafficStatistics(groupKey.groupKeyEnumValue, $scope.selectedEntityId, $scope.fromDate, $scope.toDate)
            .then(function (response) {
                var chartData = response;

                var chartDefinition = {
                    type: "spline",
                    title: $scope.selectedEntityName
                };
                var xAxisDefinition = {
                    titlePath: "FirstCDRAttempt",
                    isDateTime: true
                };
                var seriesDefinitions = [];
                angular.forEach(measures, function (measure) {
                    seriesDefinitions.push({
                        title: measure.description,
                        valuePath: measure.propertyName,
                        selected: (measure == TrafficStatisticsMeasureEnum.Attempts || measure == TrafficStatisticsMeasureEnum.DurationsInMinutes)
                    });
                });

                chartSelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            })
            .finally(function () {
                $scope.isGettingEntityStatistics = false;
            });;
        }

        function loadSwitches() {
            BusinessEntityAPIService.GetSwitches().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.switches.push(itm);
                });
            });
        }

        function loadCodeGroups()
        {            
            BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.codeGroups.push(itm);
                });
            });

        }

        function loadCustomers() {
            BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.customers.push(itm);
                });
            });
        }

        function loadSuppliers() {
            BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.suppliers.push(itm);
                });
            });
        }
        
    };