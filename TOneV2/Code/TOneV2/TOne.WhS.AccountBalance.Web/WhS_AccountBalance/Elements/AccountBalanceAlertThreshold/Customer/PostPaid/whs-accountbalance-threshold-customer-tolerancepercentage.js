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
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceAlertThreshold/Customer/PostPaid/Template/CustomerTolerancePercentageThreshold.html'
    };

    function CustomerTolerancePercentageThreshold($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.percentage = payload.Percentage;
                }
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds.PostPaid.CustTolerancePercThreshold, TOne.WhS.AccountBalance.MainExtensions',
                    Percentage: $scope.scopeModel.percentage,
                    ThresholdDescription: $scope.scopeModel.percentage + " %"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);