(function(appControllers) {

    "use strict";
	
    releaseCodeStatisticsController.$inject = ['$scope'];

    function releaseCodeStatisticsController($scope) {
        
		function defineScope(){
			$scope.onSearchClicked = function () {
                mainGridApi.clearDataAndContinuePaging();
                return getGridData();
            };
		}
		
		function defineGrid() {
            $scope.gridDataSource = [];
            $scope.loadMoreData = function () {
                return getGridData();
            };
            $scope.onGridReady = function (api) {
                mainGridApi = api;
            };
        }
		
		function defineFilters() {
			console.log("Define filters");
		}
		
		defineScope();
		defineGrid();
		defineFilters();
    }

    appControllers.controller('Analytics_ReleaseCodeStatisticsController', releaseCodeStatisticsController);

})(appControllers);