StrategyManagementController.$inject = ['$scope', 'StrategyAPIService', 'VRModalService'];


function StrategyManagementController($scope, StrategyAPIService, VRModalService) {

    var mainGridAPI;
    var arrMenuAction = [];
    
    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.loadMoreData = function () {
           return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.resetClicked = function () {            
            $scope.name = '';
            $scope.description = '';
            return getData();
        };

        $scope.addNewStrategy = addNewStrategy;

        defineMenuActions();
    }

    function load() {
        
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editStrategy
        }];
    }

    function getData() {
       

        var name = $scope.name != undefined ? $scope.name : '';
        var description = $scope.description != undefined ? $scope.description : '';
        var pageInfo = mainGridAPI.getPageInfo();


        return StrategyAPIService.GetFilteredStrategies(pageInfo.fromRow, pageInfo.toRow, name, description).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push(itm);
            });
        });
    }

    function addNewStrategy() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Strategy";
            modalScope.onStrategyAdded = function (strategy) {
                mainGridAPI.itemAdded(strategy);
            };
        };
        VRModalService.showModal('/Client/Modules/Main/Views/StrategyEditor.html', null, settings);
    }

    function editStrategy(strategy) {
        var params = {
            strategyId: strategy.StrategyId
        };

        var settings = {
           
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Strategy";
            modalScope.onStrategyUpdated = function (strategy) {
                mainGridAPI.itemUpdated(strategy);
            };
        };
        VRModalService.showModal("/Client/Modules/Main/Views/StrategyEditor.html", params, settings);
    }
}
appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
