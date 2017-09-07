'use strict';

app.directive('vrWhsBePricingsettingsEditor', ['UtilsService', 'VRCommon_CurrencyAPIService',
    function (UtilsService, VRCommon_CurrencyAPIService) {

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
                        IncreasedRateDayOffset: ctrl.increasedRateDayOffset,
                        DecreasedRateDayOffset: ctrl.decreasedRateDayOffset,
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
                ctrl.showDefaultRate = (directiveContext == "System");
                ctrl.showMaxSaleRate = (directiveContext == "System");
                ctrl.showEffectiveDateDayOffset = (directiveContext == "System");
                ctrl.showRetroactiveDayOffset = (directiveContext == "System");
                ctrl.showNewRateDayOffset = (directiveContext == "System" || directiveContext == "Customer" || directiveContext=="SellingProduct");
                ctrl.showIncreasedRateDayOffset = (directiveContext == "System" || directiveContext == "Customer" || directiveContext == "SellingProduct");
                ctrl.showDecreasedRateDayOffset = (directiveContext == "System" || directiveContext == "Customer" || directiveContext == "SellingProduct");
            }

            function prepareDirectivesRequiredForContext(directiveContext) {
                ctrl.isDefaultRateRequired = (directiveContext == "System");
                ctrl.isMaxSaleRateRequired = (directiveContext == "System");
                ctrl.isEffectiveDateDayOffset = (directiveContext == "System");
                ctrl.isRetroactiveDayOffset = (directiveContext == "System");
                ctrl.isNewRateDayOffset = (directiveContext == "System");
                ctrl.isIncreasedRateDayOffset = (directiveContext == "System");
                ctrl.isDecreasedRateDayOffset = (directiveContext == "System");
            }
        }

        return directiveDefinitionObject;
    }]);