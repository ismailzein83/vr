(function(appControllers) {

    "use strict";

    function releaseCodeStatisticsController($scope, analyticsService, businessEntityApiService, carrierAccountApiService, utilsService, analyticsApiService) {

        var mainGridApi , selectedFilter;

        function buildFilter() {
            return {
                SwitchIds: utilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
                CustomerIds: utilsService.getPropValuesFromArray($scope.selectedCustomers, "CarrierAccountID"),
                SupplierIds: utilsService.getPropValuesFromArray($scope.selectedSuppliers, "CarrierAccountID"),
                CodeGroups: utilsService.getPropValuesFromArray($scope.selectedCodeGroups, "Code")
            };
        }

        function retrieveData() {
            var filter = buildFilter();
            var groupKeys = [];

            for (var i = 0, len = $scope.selectedGroupKeys.length; i < len; i++) {
                groupKeys.push($scope.selectedGroupKeys[i]);
            }

            selectedFilter = {
                filter: filter,
                groupKeys: groupKeys,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate
            };

            return mainGridApi.retrieveData({
                Filter: filter,
                GroupKeys: groupKeys,
                From: $scope.fromDate,
                To: $scope.toDate
            });
        }

        function defineScope() {

            $scope.searchClicked = function () {
                $scope.currentSearchCriteria.groupKeys.length = 0;
                for (var i = 0, len = $scope.selectedGroupKeys.length; i < len; i++) {
                    $scope.currentSearchCriteria.groupKeys.push($scope.selectedGroupKeys[i]);
                }
                return retrieveData();
            };

            $scope.currentSearchCriteria = {
                groupKeys: []
            };
        }

        function defineGrid() {
         
            $scope.gridData = [];

            $scope.onMainGridReady = function(api) {
                mainGridApi = api;
            };

            $scope.onGroupKeyClicked = function (dataItem, colDef) {
                //var group = colDef.tag;
                //var groupIndex = $scope.currentSearchCriteria.groupKeys.indexOf(group);
                //selectEntity(group, dataItem.GroupKeyValues[groupIndex].Id, dataItem.GroupKeyValues[groupIndex].Name);
            };

            $scope.showResult = true;

            $scope.measures = analyticsService.getReleaseCodeMeasureEnum();

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                //return analyticsApiService.getReleaseCodeStatistics(dataRetrievalInput).then(function (response) {
                //    onResponseReady(response);
                //    $scope.showResult = true;
                //});

                //$scope.showResult = true;
            };
            
        }

        function defineMenuActions() {

            $scope.gridMenuActions = [{
                name: "CDRs",
                clicked: function (dataItem) {
                    
                    var parameters = {
                        fromDate: selectedFilter.fromDate,
                        toDate: selectedFilter.toDate
                    };
                    analyticsService.showCdrLogModal(parameters, dataItem.GroupKeyValues, $scope.currentSearchCriteria.groupKeys);
                }
            }];
        }
		
        function loadSwitches() {
            return businessEntityApiService.GetSwitches().then(function (response) {
                $scope.switches = response;
            });
        }

        function loadCodeGroups() {
            return businessEntityApiService.GetCodeGroups().then(function (response) {
                $scope.codeGroups = response;
            });
        }

        
        function defineFilters() {

            $scope.fromDate = '';
            $scope.toDate = '';

            $scope.groupKeys = analyticsService.getTrafficStatisticGroupKeys();
            $scope.selectedGroupKeys = analyticsService.getDefaultTrafficStatisticGroupKeys();

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];
            
            $scope.selectedCustomers = [];

            $scope.selectedSuppliers = [];

            $scope.periods = analyticsService.getPeriods();
            $scope.selectedPeriod = $scope.periods[0];

            $scope.onPeriodSelectionChanged = function() {
                if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value !== -1) {
                    var date = $scope.selectedPeriod.getInterval();
                    $scope.fromDate = date.from;
                    $scope.toDate = date.to;
                }
            };

            $scope.customvalidateDate = function (toDate) {
                return utilsService.validateDates($scope.fromDate, toDate);
            };

            loadSwitches();
            loadCodeGroups();
        }

        defineScope();
        defineFilters();
        defineGrid();
        defineMenuActions();
    }

    releaseCodeStatisticsController.$inject = ['$scope', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'CarrierAccountAPIService', 'UtilsService', 'AnalyticsAPIService'];
    appControllers.controller('Analytics_ReleaseCodeStatisticsController', releaseCodeStatisticsController);

})(appControllers);