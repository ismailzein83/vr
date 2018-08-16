'use strict';

app.directive('vrAccountbalanceBillingtransactionGrid', ['VR_AccountBalance_BillingTransactionAPIService', 'VRNotificationService','VR_AccountBalance_BillingTransactionService',
    function (VR_AccountBalance_BillingTransactionAPIService, VRNotificationService, VR_AccountBalance_BillingTransactionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var billingTransactionGrid = new BillingTransactionGrid($scope, ctrl, $attrs);
                billingTransactionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTransactionGridTemplate.html'
        };

        function BillingTransactionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

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

                api.loadGrid = function (payload) {
                    ctrl.showAccount = payload.showAccount != undefined ? payload.showAccount : true;
                    return gridAPI.retrieveData(payload.query);
                };
                api.onBillingTransactionAdded = function (billingTransaction) {
                    return gridAPI.itemAdded(billingTransaction);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "View",
                    clicked: viewBillingTransaction
                }];
            }
            function viewBillingTransaction(billingTransactionObj) {
                VR_AccountBalance_BillingTransactionService.viewBillingTransaction(billingTransactionObj.Entity, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) {
                    currentContext = {};
                }
                return currentContext;
            }

        }
    }]);
