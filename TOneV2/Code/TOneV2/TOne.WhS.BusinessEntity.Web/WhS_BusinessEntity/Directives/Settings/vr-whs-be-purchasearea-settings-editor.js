'use strict';

app.directive('vrWhsBePurchaseareaSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyAPIService', 'VRNotificationService', 'WhS_BE_PurchaseSettingsContextEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_CurrencyAPIService, VRNotificationService, WhS_BE_PurchaseSettingsContextEnum) {
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

            var priceListSettingsEditorAPI;
            var priceListSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var data;

            $scope.hintTextForIncrease = "(1 - OldRate / NewRate)*100 should be less than the specified percentage";
            $scope.hintTextForDecrease = "(1 - NewRate / OldRate)*100 should be less than the specified percentage";
            $scope.hintTextForClosing = "(ClosedZones  / TotalZones)*100 should be less than the specified percentage";


            function initializeController() {

                ctrl.onPriceListSettingsEditorReady = function (api) {
                    priceListSettingsEditorAPI = api;
                    priceListSettingsEditorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        data = payload.data;
                    }

                    load();

                    function load() {
                        loadAllControls();
                    }

                    function loadAllControls() {

                        var promises = [];

                        var loadStaticDataPromise = loadStaticData();
                        promises.push(loadStaticDataPromise);

                        var currencyPromise = getSystemCurrency();
                        promises.push(currencyPromise);

                        if (data != undefined) {
                            var loadPriceListSettingsPromise = loadPriceListSettings(data.PricelistSettings);
                            promises.push(loadPriceListSettingsPromise);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.PurchaseAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
                        EffectiveDateDayOffset: ctrl.effectiveDateDayOffset,
                        RetroactiveDayOffset: ctrl.retroactiveDayOffset,
                        MaximumRate: ctrl.maximumRate,
                        MaximumCodeRange: ctrl.maximumCodeRange,
                        AcceptableIncreasedRate: ctrl.acceptableIncreasedRate,
                        AcceptableDecreasedRate: ctrl.acceptableDecreasedRate,
                        AcceptableZoneClosingPercentage: ctrl.acceptableZoneClosingPercentage,
                        CodeGroupVerfifcation: ctrl.codeGroupVerification,
                        PricelistSettings: priceListSettingsEditorAPI.getData(),
                        AllowRateZero: ctrl.allowRateZero
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
                ctrl.acceptableIncreasedRate = data.AcceptableIncreasedRate;
                ctrl.acceptableDecreasedRate = data.AcceptableDecreasedRate;
                ctrl.acceptableZoneClosingPercentage = data.AcceptableZoneClosingPercentage;
                ctrl.codeGroupVerification = data.CodeGroupVerfifcation;
                ctrl.allowRateZero = data.AllowRateZero;
            }

            function loadPriceListSettings(data) {
                var priceListSettingsEditorLoadDeferred = UtilsService.createPromiseDeferred();
                priceListSettingsEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: data,
                        directiveContext: WhS_BE_PurchaseSettingsContextEnum.System.value
                    };
                    VRUIUtilsService.callDirectiveLoad(priceListSettingsEditorAPI, payload, priceListSettingsEditorLoadDeferred);
                });
                return priceListSettingsEditorLoadDeferred.promise;
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