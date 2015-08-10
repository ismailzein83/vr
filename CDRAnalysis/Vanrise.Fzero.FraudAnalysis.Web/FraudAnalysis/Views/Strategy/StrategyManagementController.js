StrategyManagementController.$inject = ['$scope', 'StrategyAPIService','UserAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function StrategyManagementController($scope, StrategyAPIService,UserAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

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

        $scope.isDefault = [{ value: null, name: 'All' }, { value: false, name: 'Not Default' }, { value: true, name: 'Default' }];
        $scope.selectedIsDefault = '';

        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriods = [];


        $scope.users = [];
        loadUsers();
        $scope.selectedUsers = [];


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

    function removeLastComma(strng) {
        var n = strng.lastIndexOf(",");
        var a = strng.substring(0, n)
        return a;
    }

    function retrieveData() {

        var name = $scope.name != undefined ? $scope.name : '';
        var description = $scope.description != undefined ? $scope.description : '';

        var periodsList = '';

        angular.forEach($scope.selectedPeriods, function (itm) {
            periodsList = periodsList + itm.Id + ','
        });


        var usersList = '';

        angular.forEach($scope.selectedUsers, function (itm) {
            usersList = usersList + itm.Id + ','
        });


        var query = {
            Name: name,
            Description: description,
            PeriodsList: removeLastComma(periodsList),
            UsersList: removeLastComma(usersList),
            IsDefault: $scope.selectedIsDefault.value
        };

        return mainGridAPI.retrieveData(query);
    }

    function loadUsers() {
        return UserAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
        });
    }

    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }

    function addNewStrategy() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Strategy";
            modalScope.onStrategyAdded = function (strategy) {
                strategy.IsDefaultText = strategy.IsDefault ? "Default" : "Not Default";
                strategy.StrategyType = UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id").Name;
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
                strategy.StrategyType = UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id").Name;
                mainGridAPI.itemUpdated(strategy);
            };
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html", params, settings);
    }
}
appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
