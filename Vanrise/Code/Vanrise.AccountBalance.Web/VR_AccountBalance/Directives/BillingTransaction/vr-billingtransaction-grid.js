﻿'use strict';

app.directive('vrBillingtransactionGrid', ['VR_AccountBalance_BillingTransactionAPIService', 'VRNotificationService',
    function (VR_AccountBalance_BillingTransactionAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var billingTransactionGrid = new BillingTransactionGrid($scope, ctrl, $attrs);
                billingTransactionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTranactionGridTemplate.html'
        };

        function BillingTransactionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.billingTransactions = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_AccountBalance_BillingTransactionAPIService.GetFilteredBillingTransactions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };
                api.onBillingTransactionAdded = function (billingTransaction)
                {
                   return gridAPI.itemAdded(billingTransaction);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
            }

        }
    }]);
