DashboardController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DashboardController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

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

        $scope.isGettingStrategies = true;

        var name = $scope.name != undefined ? $scope.name : '';
        var description = $scope.description != undefined ? $scope.description : '';
        var pageInfo = mainGridAPI.getPageInfo();

        return StrategyAPIService.GetFilteredStrategies(pageInfo.fromRow, pageInfo.toRow, name, description).then(function (response) {
            angular.forEach(response, function (itm) {

                itm.IsDefaultText = itm.IsDefault ? "Default" : "Not Default";
                $scope.strategies.push(itm);
            });
        }).finally(function () {
            $scope.isGettingStrategies = false;
        });
    }

    function addNewStrategy() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Strategy";
            modalScope.onStrategyAdded = function (strategy) {
                strategy.IsDefaultText = strategy.IsDefault ? "Default" : "Not Default";
                mainGridAPI.itemAdded(strategy);
            };
        };
        VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html', null, settings);
    }

    function editStrategy(strategy) {
        var params = {
            strategyId: strategy.Id
        };

        var settings = {

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Strategy";
            modalScope.onStrategyUpdated = function (strategy) {
                strategy.IsDefaultText = strategy.IsDefault ? "Default" : "Not Default";
                mainGridAPI.itemUpdated(strategy);
            };
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html", params, settings);
    }
}
appControllers.controller('FraudAnalysis_DashboardController', DashboardController);
