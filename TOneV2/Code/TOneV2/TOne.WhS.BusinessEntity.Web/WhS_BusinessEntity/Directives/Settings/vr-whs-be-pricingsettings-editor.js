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

                ctrl.showDefaultRate = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showMaxSaleRate = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showEffectiveDateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showRetroactiveDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.showNewRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.SellingProduct.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
                ctrl.showEndCountryDayOffset = (directiveContext != WhS_BE_SaleAreaSettingsContextEnum.SellingProduct.value);
                ctrl.showIncreasedRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.SellingProduct.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
                ctrl.showDecreasedRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.SellingProduct.value || directiveContext == WhS_BE_SaleAreaSettingsContextEnum.Customer.value);
            }

            function prepareDirectivesRequiredForContext(directiveContext) {

                ctrl.isDefaultRateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isMaxSaleRateRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isEffectiveDateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isRetroactiveDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isNewRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isEndCountryDayOffsetRequired = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isIncreasedRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
                ctrl.isDecreasedRateDayOffset = (directiveContext == WhS_BE_SaleAreaSettingsContextEnum.System.value);
            }
        }

        return directiveDefinitionObject;
    }]);