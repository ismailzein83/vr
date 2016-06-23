'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancialPrepaid', ['UtilsService', function (UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypFinancialPrePaidPartRuntime = new AccountTypFinancialPrePaidPartRuntime($scope, ctrl, $attrs);
            accountTypFinancialPrePaidPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeFinancialPrepaidPartRuntimeTemplate.html'
    };

    function AccountTypFinancialPrePaidPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
            };

            api.getData = function () {
                return {
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);