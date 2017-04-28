(function (app) {

    'use strict';

    FinancialAccountsViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function FinancialAccountsViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FinancialAccountsViewTemplateCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/FinancialAccountsViewTemplate.html'
        };

        function FinancialAccountsViewTemplateCtor($scope, ctrl) {
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
                var FinancialAccountsGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountId: parentAccountId,
                };
                return FinancialAccountsGridPayload;
            }
        }
    }

    app.directive('retailBeFinancialaccountsView', FinancialAccountsViewDirective);

})(app);