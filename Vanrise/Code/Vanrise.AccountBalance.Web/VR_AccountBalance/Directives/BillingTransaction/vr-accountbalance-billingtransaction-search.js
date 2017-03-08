﻿"use strict";

app.directive("vrAccountbalanceBillingtransactionSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_AccountBalance_LiveBalanceAPIService', 'VR_AccountBalance_BillingTransactionService','VR_AccountBalance_BillingTransactionAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VR_AccountBalance_LiveBalanceAPIService, VR_AccountBalance_BillingTransactionService, VR_AccountBalance_BillingTransactionAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTransactionSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridAPI;
        var accountTypeId;
        var accountsIds;
        this.initializeController = initializeController;
        function initializeController() {

            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.loadDirective = function (payload) {
                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        accountsIds = payload.AccountsIds;

                    }
                    console.log(payload);
                    if (accountTypeId)
                        checkHasAddBillingTransaction(accountTypeId);
                    load();
                };
                return directiveAPI;
            }
        }

        function defineScope() {
            var fromTime = new Date();
            fromTime.setMonth(fromTime.getMonth() - 1);
            fromTime.setHours(0, 0, 0, 0);
            $scope.fromTime = fromTime;
            $scope.hasAddTransaction = false;
            $scope.searchClicked = function () {
                var payload = {
                    query: getFilterObject(),
                    showAccount: false,
                }
                return gridAPI.loadGrid(payload).then(function () {
                    load();
                });
            };
            $scope.addTransaction = function () {
                var onBillingTransacationAdded = function (obj) {
                    gridAPI.onBillingTransactionAdded(obj);
                };
                VR_AccountBalance_BillingTransactionService.addBillingTransaction(accountsIds[0], accountTypeId, onBillingTransacationAdded)
            }
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromTime, $scope.toTime);
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var payload = {
                    query: getFilterObject(),
                    showAccount: false,
                }
                gridAPI.loadGrid(payload);
            };
        }

        function load() {
            $scope.isLoading = true;
            VR_AccountBalance_LiveBalanceAPIService.GetCurrentAccountBalance(accountsIds[0], accountTypeId).then(function (response) {
                if (response) {
                    $scope.balance = response.CurrentBalance;
                    $scope.currency = response.CurrencyDescription;
                }

            }).finally(function () {
                $scope.isLoading = false;
            })
        }
        function getFilterObject() {
            var filter = {
                AccountTypeId: accountTypeId,
                AccountsIds: accountsIds,
                FromTime: $scope.fromTime,
                ToTime: $scope.toTime,
            };
            return filter;
        }

        function checkHasAddBillingTransaction(accountTypeId) {
            VR_AccountBalance_BillingTransactionAPIService.HasAddBillingTransactionPermission(accountTypeId).then(function (response) {
                $scope.hasAddTransaction = response;
            });
        }
    }

    return directiveDefinitionObject;

}]);
