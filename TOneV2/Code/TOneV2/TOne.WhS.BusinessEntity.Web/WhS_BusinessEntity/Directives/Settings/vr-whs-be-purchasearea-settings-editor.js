'use strict';

app.directive('vrWhsBePurchaseareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyAPIService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRCommon_CurrencyAPIService, VRNotificationService) {
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

        var pricelistTypeMappingAPI;
        var pricelistTypeMappingReadyDeferred = UtilsService.createPromiseDeferred();

        var data;

        function initializeController() {

            defineAPI();

            ctrl.onPricelistTypeMappingReady = function (api) {
                pricelistTypeMappingAPI = api;
                pricelistTypeMappingReadyDeferred.resolve();
            };

        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                    data = payload.data;
                }

                var promises = [];
                load();

                function load() {
                    loadAllControls();
                }

                function loadAllControls() {
                    return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadPricelistMapping, getSystemCurrency])
                       .catch(function (error) {
                           VRNotificationService.notifyExceptionWithClose(error, $scope);
                       })
                      .finally(function () {
                      });
                }

            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.BusinessEntity.Entities.PurchaseAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
                    EffectiveDateDayOffset: ctrl.effectiveDateDayOffset,
                    RetroactiveDayOffset: ctrl.retroactiveDayOffset,
                    MaximumRate: ctrl.maximumRate,
                    MaximumCodeRange: ctrl.maximumCodeRange,
                    PricelistTypeMappingList: pricelistTypeMappingAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadStaticData() {
            if (data == undefined)
                return;
            ctrl.effectiveDateDayOffset = data.EffectiveDateDayOffset;
            ctrl.retroactiveDayOffset = data.RetroactiveDayOffset;
            ctrl.maximumRate = data.MaximumRate;
            ctrl.maximumCodeRange = data.MaximumCodeRange;
        }

        function loadPricelistMapping() {
            var pricelistTypeMappingloadDeferred = UtilsService.createPromiseDeferred();
            pricelistTypeMappingReadyDeferred.promise.then(function () {
                    var payload;
                    if (data != undefined && data.PricelistTypeMappingList != undefined) {
                        payload = {
                            pricelistTypeMappingList: data.PricelistTypeMappingList
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(pricelistTypeMappingAPI, payload, pricelistTypeMappingloadDeferred);
                });
            return pricelistTypeMappingloadDeferred.promise;
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