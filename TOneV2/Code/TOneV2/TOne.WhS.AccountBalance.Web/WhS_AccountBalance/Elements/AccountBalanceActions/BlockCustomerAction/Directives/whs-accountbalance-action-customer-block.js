'use strict';

app.directive('whsAccountbalanceActionCustomerBlock', [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new BlockCustomerAction($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/BlockCustomerAction/Directives/Templates/BlockCustomerAction.html'
    };

    function BlockCustomerAction($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions.BlockCustomerAction, TOne.WhS.AccountBalance.MainExtensions',
                    ActionName: 'Block Customer',

                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);