'use strict';

app.directive('whsAccountbalanceBedefinitionEditor', [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var financialAccountBEDefinitionEditor = new FinancialAccountBEDefinitionEditor($scope, ctrl, $attrs);
            financialAccountBEDefinitionEditor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/BE_Definition/Templates/AccountDefinitionBEEditor.html'
    };

    function FinancialAccountBEDefinitionEditor($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountTypeSelectorAPI;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                return accountTypeSelectorAPI.load(payload);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.Business.FinancialAccountBESettings, TOne.WhS.AccountBalance.Business',
                    AccountTypeId: accountTypeSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);