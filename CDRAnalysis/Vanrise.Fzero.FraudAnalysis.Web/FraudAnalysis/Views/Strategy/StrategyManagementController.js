StrategyManagementController.$inject = ['$scope', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function StrategyManagementController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();


    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            return retrieveData();
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }


        $scope.addNewStrategy = addNewStrategy;

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return StrategyAPIService.GetFilteredStrategies(dataRetrievalInput)
            .then(function (response) {
               
                angular.forEach(response.Data, function (itm) {
                    itm.IsDefaultText = itm.IsDefault ? "Default" : "Not Default"
                });

                onResponseReady(response);
            });
        }

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

    function retrieveData() {

        var name = $scope.name != undefined ? $scope.name : '';
        var description = $scope.description != undefined ? $scope.description : '';

        var query = {
            Name: name,
            Description: description
        };

        return mainGridAPI.retrieveData(query);
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
appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
