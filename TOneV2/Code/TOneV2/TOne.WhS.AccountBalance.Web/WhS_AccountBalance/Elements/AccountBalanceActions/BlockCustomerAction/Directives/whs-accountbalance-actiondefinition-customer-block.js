'use strict';

app.directive('whsAccountbalanceActiondefinitionCustomerBlock', [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockCustomerActionDefinition = new BlockCustomerActionDefinition($scope, ctrl, $attrs);
            blockCustomerActionDefinition.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/BlockCustomerAction/Directives/Templates/BlockCustomerActionDefinition.html'
    };

    function BlockCustomerActionDefinition($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.BlockCustomerActionDefinition, TOne.WhS.AccountBalance.MainExtensions'
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);