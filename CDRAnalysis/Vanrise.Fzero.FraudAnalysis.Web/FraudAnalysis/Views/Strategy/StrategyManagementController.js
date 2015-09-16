﻿"use strict";

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
                        itm.IsDefaultText = itm.IsDefault ? KindEnum.SystemBuiltIn.name : KindEnum.UserDefined.name;
                        itm.IsEnabledText = itm.IsEnabled ? StatusEnum.Enabled.name : StatusEnum.Disabled.name;
                        itm.StrategyType = UtilsService.getItemByVal($scope.periods, itm.PeriodId, "Id").Name;
                        itm.Analyst = UtilsService.getItemByVal($scope.users, itm.UserId, "UserId").Name
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
        
        console.log(query);
        return mainGridAPI.retrieveData(query);
    }

    function loadUsers() {
        return UsersAPIService.GetUsers().then(function (response) {
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
                strategy.IsDefaultText = strategy.IsDefault ? KindEnum.SystemBuiltIn.name : KindEnum.UserDefined.name;
                strategy.IsEnabledText = strategy.IsEnabled ? StatusEnum.Enabled.name : StatusEnum.Disabled.name;
                strategy.StrategyType = UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id").Name;
                strategy.Analyst = UtilsService.getItemByVal($scope.users, strategy.UserId, "UserId").Name
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
                strategy.IsDefaultText = strategy.IsDefault ? KindEnum.SystemBuiltIn.name : KindEnum.UserDefined.name;
                strategy.IsEnabledText = strategy.IsEnabled ? StatusEnum.Enabled.name : StatusEnum.Disabled.name;
                strategy.StrategyType = UtilsService.getItemByVal($scope.periods, strategy.PeriodId, "Id").Name;
                strategy.Analyst = UtilsService.getItemByVal($scope.users, strategy.UserId, "UserId").Name
               
                mainGridAPI.itemUpdated(strategy);
            };
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html", params, settings);
    }
}
appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
