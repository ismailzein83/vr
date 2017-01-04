(function (app) {

    'use strict';

    FinancialTransactionsViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function FinancialTransactionsViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FinancialTransactionsViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/FinancialTransactionsViewTemplate.html'
        };

        function FinancialTransactionsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var parentAccountId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }

                    return gridAPI.loadDirective(buildGridPayload());
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload() {

                var billingTransactionGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    AccountsIds: [parentAccountId],
                    AccountTypeId: "20b0c83e-6f53-49c7-b52f-828a19e6dc2a"
                };
                return billingTransactionGridPayload;
            }
        }
    }

    app.directive('retailBeFinancialtransactionsView', FinancialTransactionsViewDirective);

})(app);