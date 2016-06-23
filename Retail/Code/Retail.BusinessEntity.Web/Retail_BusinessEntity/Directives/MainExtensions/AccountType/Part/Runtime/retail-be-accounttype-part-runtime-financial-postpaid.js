'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancialPostpaid', ['UtilsService', function (UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypFinancialPostPaidPartRuntime = new AccountTypFinancialPostPaidPartRuntime($scope, ctrl, $attrs);
            accountTypFinancialPostPaidPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeFinancialPostpaidPartRuntimeTemplate.html'
    };

    function AccountTypFinancialPostPaidPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if(payload !=undefined)
                {
                    $scope.scopeModel.duePeriodInDays = payload.DuePeriodInDays;
                }
            };

            api.getData = function () {
                return {
                    DuePeriodInDays: $scope.scopeModel.duePeriodInDays
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);