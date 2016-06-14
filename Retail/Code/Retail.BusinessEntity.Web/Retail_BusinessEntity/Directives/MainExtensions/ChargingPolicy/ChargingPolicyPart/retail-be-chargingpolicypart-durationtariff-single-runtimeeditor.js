'use strict';

app.directive('retailBeChargingpolicypartDurationtariffSingleRuntimeeditor', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var singleDurationTariffChargingPolicyPart = new SingleDurationTariffChargingPolicyPart($scope, ctrl, $attrs);
            singleDurationTariffChargingPolicyPart.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ChargingPolicy/ChargingPolicyPart/Templates/SingleDurationTariffChargingPolicyPartTemplate.html'
    };

    function SingleDurationTariffChargingPolicyPart($scope, ctrl, $attrs) {
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
                var tariffSettings;

                if (payload != undefined) {
                    tariffSettings = payload.TariffSettings;
                }

                var directivePayload = {
                    settings: tariffSettings
                };

                return directiveAPI.load(directivePayload);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.DurationTariffs.SingleDurationTariff, Retail.BusinessEntity.MainExtensions',
                    TariffSettings: directiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
