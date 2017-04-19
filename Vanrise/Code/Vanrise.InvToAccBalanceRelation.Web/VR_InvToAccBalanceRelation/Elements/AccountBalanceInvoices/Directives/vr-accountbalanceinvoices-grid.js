'use strict';

app.directive('vrAccountbalanceinvoicesGrid', ['VR_InvToAccBalanceRelation_AccountBalanceInvoicesAPIService', 'VRNotificationService',
    function (VR_InvToAccBalanceRelation_AccountBalanceInvoicesAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var ctor = new BillingTransactionInvoicesGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_InvToAccBalanceRelation/Elements/AccountBalanceInvoices/Directives/Templates/AccountBalanceInvoicesGrid.html'
        };

        function BillingTransactionInvoicesGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountInvoices = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_InvToAccBalanceRelation_AccountBalanceInvoicesAPIService.GetFilteredAccountInvoices(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return gridAPI.retrieveData(payload);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
            }

        }
    }]);
