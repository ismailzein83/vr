'use strict';

app.directive('vrWhsBePricingsettingsEditor', ['UtilsService', 'VRCommon_CurrencyAPIService', 'WhS_BE_SaleAreaSettingsContextEnum',
    function (UtilsService, VRCommon_CurrencyAPIService, WhS_BE_SaleAreaSettingsContextEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new pricingSettingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PricingSettingsTemplate.html"
        };

        function pricingSettingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var data;
                    var directiveContext;

                    if (payload != undefined) {
                        data = payload.data;
                        directiveContext = payload.directiveContext;
                    }

                    prepareDirectivesViewForContext(directiveContext);
                    prepareDirectivesRequiredForContext(directiveContext);

                    promises.push(getSystemCurrency());

                    loadStaticData(data);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        DefaultRate: ctrl.defaultRate,
                        EffectiveDateDayOffset: ctrl.effectiveDateDayOffset,
                        RetroactiveDayOffset: ctrl.retroactiveDayOffset,
                        MaximumRate: ctrl.maximumRate,
                        NewRateDayOffset: ctrl.newRateDayOffset,
                        EndCountryDayOffset: ctrl.endCountryDayOffset,
                        IncreasedRateDayOffset: ctrl.increasedRateDayOffset,
                        DecreasedRateDayOffset: ctrl.decreasedRateDayOffset
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadStaticData(data) {
                if (data == undefined)
                    return;

                ctrl.defaultRate = data.DefaultRate;
                ctrl.effectiveDateDayOffset = data.EffectiveDateDayOffset;
                ctrl.retroactiveDayOffset = data.RetroactiveDayOffset;
                ctrl.maximumRate = data.MaximumRate;
                ctrl.newRateDayOffset = data.NewRateDayOffset;
                ctrl.endCountryDayOffset = data.EndCountryDayOffset;
                ctrl.increasedRateDayOffset = data.IncreasedRateDayOffset;
                ctrl.decreasedRateDayOffset = data.DecreasedRateDayOffset;
            }

            function getSystemCurrency() {
                return VRCommon_CurrencyAPIService.GetSystemCurrency().then(function (response) {
                    if (response != undefined) {
                        ctrl.systemCurrencySymbol = response.Symbol;
                    }
                });
            }

            function prepareDirectivesViewForContext(directiveContext) {
                var systemEnumValue = WhS_BE_SaleAreaSettingsContextEnum.System.value;
                var sellingProductEnumValue = WhS_BE_SaleAreaSettingsContextEnum.SellingProduct.value;
                var customerEnumValue = WhS_BE_SaleAreaSettingsContextEnum.Customer.value;


                ctrl.showDefaultRate = (directiveContext == systemEnumValue);
                ctrl.showMaxSaleRate = (directiveContext == systemEnumValue);
                ctrl.showEffectiveDateDayOffset = (directiveContext == systemEnumValue);
                ctrl.showRetroactiveDayOffset = (directiveContext == systemEnumValue);
                ctrl.showNewRateDayOffset = (directiveContext == systemEnumValue || directiveContext == sellingProductEnumValue || directiveContext == customerEnumValue);
                ctrl.showEndCountryDayOffset = (directiveContext == systemEnumValue || directiveContext == sellingProductEnumValue || directiveContext == customerEnumValue);
                ctrl.showIncreasedRateDayOffset = (directiveContext == systemEnumValue || directiveContext == sellingProductEnumValue || directiveContext == customerEnumValue);
                ctrl.showDecreasedRateDayOffset = (directiveContext == systemEnumValue || directiveContext == sellingProductEnumValue || directiveContext == customerEnumValue);
            }

            function prepareDirectivesRequiredForContext(directiveContext) {
                var systemEnumValue = WhS_BE_SaleAreaSettingsContextEnum.System.value;

                ctrl.isDefaultRateRequired = (directiveContext == systemEnumValue);
                ctrl.isMaxSaleRateRequired = (directiveContext == systemEnumValue);
                ctrl.isEffectiveDateDayOffset = (directiveContext == systemEnumValue);
                ctrl.isRetroactiveDayOffset = (directiveContext == systemEnumValue);
                ctrl.isNewRateDayOffset = (directiveContext == systemEnumValue);
                ctrl.isEndCountryDayOffsetRequired = (directiveContext == systemEnumValue);
                ctrl.isIncreasedRateDayOffset = (directiveContext == systemEnumValue);
                ctrl.isDecreasedRateDayOffset = (directiveContext == systemEnumValue);
            }
        }

        return directiveDefinitionObject;
    }]);