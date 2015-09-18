"use strict";

StrategyManagementController.$inject = ['$scope', 'StrategyAPIService', 'UsersAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum'];

function StrategyManagementController($scope, StrategyAPIService, UsersAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {


        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;

            if (!$scope.isInitializing) // if the filters are loaded
                return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.isDefault = [];
        angular.forEach(KindEnum, function (kind) {
            $scope.isDefault.push({ value: kind.value, name: kind.name })
        });

        $scope.selectedIsDefault = [];

        $scope.isEnabled = [];
        angular.forEach(StatusEnum, function (itm) {
            $scope.isEnabled.push({ value: itm.value, name: itm.name })
        });

        $scope.selectedIsEnabled = [];

        $scope.periods = [];

        $scope.selectedPeriods = [];

        $scope.users = [];
        $scope.selectedUsers = [];

        $scope.addNewStrategy = addNewStrategy;

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return StrategyAPIService.GetFilteredStrategies(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (strategy) {
                        fillStrategy(strategy);
                    });

                    onResponseReady(response);
                });
        }

        defineMenuActions();
    }


    function fillStrategy(strategy) {
        strategy.IsDefaultText = strategy.IsDefault ? KindEnum.SystemBuiltIn.name : KindEnum.UserDefined.name;
        strategy.StrategyType = (UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id") != null ? UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id").Name : null)
        strategy.Analyst = (UtilsService.getItemByVal($scope.users, strategy.UserId, "UserId") != null ? UtilsService.getItemByVal($scope.users, strategy.UserId, "UserId").Name : null)
    }

    function load() {
        $scope.isInitializing = true;

        return UtilsService.waitMultipleAsyncOperations([loadUsers, loadPeriods])
            .then(function () {
                if (mainGridAPI != undefined) // i.e. the grid has been waiting for the periods to load before it gets the data
                    return retrieveData();
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editStrategy,
            permissions: "Root/Strategy Module:Edit"
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

        var isDefaultsList = '';

        angular.forEach($scope.selectedIsDefault, function (itm) {
            isDefaultsList = isDefaultsList + itm.value + ','
        });

        var isEnabledList = '';

        angular.forEach($scope.selectedIsEnabled, function (itm) {
            isEnabledList = isEnabledList + itm.value + ','
        });

        var query = {
            Name: name,
            Description: description,
            PeriodIDs: ($scope.selectedPeriods.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedPeriods, "Id") : null,
            UserIDs: ($scope.selectedUsers.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedUsers, "UserId") : null,
            IsDefaultList: removeLastComma(isDefaultsList),
            IsEnabledList: removeLastComma(isEnabledList),
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return mainGridAPI.retrieveData(query);
    }

    function loadUsers() {
        return UsersAPIService.GetUsers()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.users.push(item);
                });
            });
    }

    function loadPeriods() {
        return StrategyAPIService.GetPeriods()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.periods.push(item);
                });
            });
    }

    function addNewStrategy() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Strategy";
            modalScope.onStrategyAdded = function (strategy) {
                fillStrategy(strategy);
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
                fillStrategy(strategy);
                mainGridAPI.itemUpdated(strategy);
            };
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html", params, settings);
    }
}

appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
