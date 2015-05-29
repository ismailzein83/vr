StrategyManagementController.$inject = ['$scope', 'StrategyAPIService'];


function StrategyManagementController($scope, StrategyAPIService) {

    var mainGridAPI;
      
    defineScope();
    

    function defineScope() {

        $scope.strategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

       

        $scope.searchClicked = function () {
            return getData();
        };

       
    }

    
    function getData() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            $scope.strategies.length = 0;
            angular.forEach(response, function (itm) {
                $scope.strategies.push(itm);
            });
        });
    }
}


appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
