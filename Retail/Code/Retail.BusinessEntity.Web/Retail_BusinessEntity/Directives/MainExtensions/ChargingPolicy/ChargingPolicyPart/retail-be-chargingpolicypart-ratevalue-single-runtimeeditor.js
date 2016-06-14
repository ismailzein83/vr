'use strict';

app.directive('retailBeChargingpolicypartRatevalueSingleRuntimeeditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var singleRateValueChargingPolicyPart = new SingleRateValueChargingPolicyPart($scope, ctrl, $attrs);
            singleRateValueChargingPolicyPart.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ChargingPolicy/ChargingPolicyPart/Templates/SingleRateValueChargingPolicyPartTemplate.html'
    };

    function SingleRateValueChargingPolicyPart($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var directiveAPI;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var rateValueSettings;

                if (payload != undefined) {
                    rateValueSettings = payload.RateValueSettings;
                }

                var directivePayload = {
                    settings: rateValueSettings
                };

                return directiveAPI.load(directivePayload);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues.SingleRateValue, Retail.BusinessEntity.MainExtensions',
                    RateValueSettings: directiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
