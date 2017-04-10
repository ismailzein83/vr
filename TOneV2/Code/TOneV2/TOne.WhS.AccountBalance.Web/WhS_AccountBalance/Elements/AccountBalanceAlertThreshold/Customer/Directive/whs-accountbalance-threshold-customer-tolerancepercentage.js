'use strict';

app.directive('whsAccountbalanceThresholdCustomerTolerancepercentage', [function () {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var customerTolerancePercentageThreshold = new CustomerTolerancePercentageThreshold($scope, ctrl, $attrs);
            customerTolerancePercentageThreshold.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceAlertThreshold/Customer/Directive/Template/CustomerTolerancePercentageThreshold.html'
    };

    function CustomerTolerancePercentageThreshold($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.validatePercentage = function () {
                if ($scope.scopeModel.percentage == undefined)
                    return null;
                var percentageAsNumber = Number($scope.scopeModel.percentage);
                if (percentageAsNumber <= 0 || percentageAsNumber > 100)
                    return 'Percentage must be a positive number greater than 0 and less than 100';
                return null;
            };

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