StrategyManagementController.$inject = ['$scope'];

function StrategyManagementController($scope) {

    defineScope();
    load();

    function defineScope() {
        $scope.testModel = "FraudAnalysis_StrategyManagementController";
    }

    function load() {
        
    }
};

appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);