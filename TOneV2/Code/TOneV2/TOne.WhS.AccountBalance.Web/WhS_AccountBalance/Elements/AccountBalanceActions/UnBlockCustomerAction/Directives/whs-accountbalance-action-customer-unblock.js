'use strict';

app.directive('whsAccountbalanceActionCustomerUnblock', [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new UnBlockCustomerAction($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/UnBlockCustomerAction/Directives/Templates/UnBlockCustomerAction.html'
    };

    function UnBlockCustomerAction($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions.UnBlockCustomerAction, TOne.WhS.AccountBalance.MainExtensions'
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);