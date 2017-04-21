(function (app) {

    'use strict';

    InvoiceRecurChargePackageSettingsDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PricingPackageSettingsService'];

    function InvoiceRecurChargePackageSettingsDirective(UtilsService, VRNotificationService, Retail_BE_PricingPackageSettingsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceRecurChargePackageSettings($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/InvoiceRecurChargePackageSettingsTemplate.html'
        };

        function InvoiceRecurChargePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([currencySelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var extendedSettings;
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                        if(extendedSettings != undefined)
                        {
                            ctrl.price = extendedSettings.Price;
                        }
                    }
                    var currencySelectorPayload = extendedSettings != undefined ? { selectedIds: extendedSettings.CurrencyId } : undefined;
                    promises.push( currencySelectorAPI.load(currencySelectorPayload));
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageTypes.InvoiceRecurChargePackageSettings, Retail.BusinessEntity.MainExtensions",
                        Price: ctrl.price,
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        
        }
    }

    app.directive('retailBePackagesettingsExtendedsettingsInvoicerecurcharge', InvoiceRecurChargePackageSettingsDirective);

})(app);