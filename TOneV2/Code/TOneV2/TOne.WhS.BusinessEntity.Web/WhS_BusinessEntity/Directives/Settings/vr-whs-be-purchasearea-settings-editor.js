'use strict';

app.directive('vrWhsBePurchaseareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyAPIService', 'VRNotificationService', function (UtilsService, VRUIUtilsService, VRCommon_CurrencyAPIService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var purchaseAreaSettings = new PurchaseAreaSettings(ctrl, $scope, $attrs);
            purchaseAreaSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PurchaseAreaSettingsTemplate.html"
    };

    function PurchaseAreaSettings(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var data;

                if (payload != undefined) {
                    data = payload.data;
                }

                var promises = [];

                loadStaticData(data);

                var getSystemCurrencyPromise = getSystemCurrency();
                promises.push(getSystemCurrencyPromise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.BusinessEntity.Entities.PurchaseAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
                    EffectiveDateDayOffset: ctrl.effectiveDateDayOffset,
                    RetroactiveDayOffset: ctrl.retroactiveDayOffset,
                    MaximumRate: ctrl.maximumRate,
                    MaximumCodeRange: ctrl.maximumCodeRange
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadStaticData(data) {
            if (data == undefined)
                return;
            ctrl.effectiveDateDayOffset = data.EffectiveDateDayOffset;
            ctrl.retroactiveDayOffset = data.RetroactiveDayOffset;
            ctrl.maximumRate = data.MaximumRate;
            ctrl.maximumCodeRange = data.MaximumCodeRange;
        }
        function getSystemCurrency() {
            return VRCommon_CurrencyAPIService.GetSystemCurrency().then(function (response) {
                if (response != undefined) {
                    ctrl.systemCurrencySymbol = response.Symbol;
                }
            });
        }
    }
}]);