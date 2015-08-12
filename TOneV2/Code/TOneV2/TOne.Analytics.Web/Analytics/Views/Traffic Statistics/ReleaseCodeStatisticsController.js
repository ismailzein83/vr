(function(appControllers) {

    "use strict";

    function releaseCodeStatisticsController($scope, analyticsService, businessEntityApiService, carrierAccountApiService,utilsService) {
        
        function defineScope() {

            $scope.showResult = false;

            $scope.searchClicked = function () {

            };
        }
		
        function defineGrid() {
         
            //$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            //    return AnalyticsAPIService.GetTrafficStatisticSummary(dataRetrievalInput).then(function(response) {
                    
            //        onResponseReady(response);
            //        $scope.showResult = true;
            //    });
            //};
            
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

        function loadCustomers() {
            return carrierAccountApiService.getCustomers().then(function (response) {
                $scope.customers = response;
            });
        }

        function loadSuppliers() {
            return carrierAccountApiService.getSuppliers().then(function (response) {
                $scope.suppliers = response;
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

            $scope.customers = [];
            $scope.selectedCustomers = [];

            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.periods = analyticsService.getPeriods();
            $scope.selectedPeriod = $scope.periods[0];

            $scope.onPeriodSelectionChanged = function () {
                if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value !== -1) {
                    var date = $scope.selectedPeriod.getInterval();
                    $scope.fromDate = date.from;
                    $scope.toDate = date.to;
                }

            }

            $scope.customvalidateDate = function (toDate) {
                return utilsService.validateDates($scope.fromDate, toDate);
            };

            loadSwitches();
            loadCodeGroups();
            loadCustomers();
            loadSuppliers();
        }

		
		
        defineScope();
        defineGrid();
        defineFilters();
    }

    releaseCodeStatisticsController.$inject = ['$scope', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'CarrierAccountAPIService', 'UtilsService'];
    appControllers.controller('Analytics_ReleaseCodeStatisticsController', releaseCodeStatisticsController);

})(appControllers);