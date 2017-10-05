(function (app) {

    'use strict';

    SupplierMarginRateCalculation.$inject = ["UtilsService", 'VRUIUtilsService'];

    function SupplierMarginRateCalculation(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SupplierMarginRateCalculationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "supplierMarginRateCalculationCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/PricingTemplate/Templates/SupplierMarginRateCalculationTemplate.html"

        };
        function SupplierMarginRateCalculationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var supplierSelectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSupplierSelectorReady = function (api) {
                    supplierSelectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var marginRateCalculation;

                    if (payload != undefined) {
                        marginRateCalculation = payload.marginRateCalculation;
                    }

                    var loadSupplierSelectorPromise = loadSupplierSelector();
                    promises.push(loadSupplierSelectorPromise);

                    function loadSupplierSelector() {
                        var supplierSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var supplierSelectorPayload;
                        if (marginRateCalculation != undefined) {
                            supplierSelectorPayload = { selectedIds: marginRateCalculation.SupplierId };
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, supplierSelectorPayload, supplierSelectorLoadDeferred);

                        return supplierSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function getData() {
                    var data = {
                        $type: "TOne.WhS.Sales.MainExtensions.PricingTemplateRate.SupplierMarginRateCalculation, TOne.WhS.Sales.MainExtensions",
                        SupplierId: supplierSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsSalesMarginratecalculationSupplier', SupplierMarginRateCalculation);

})(app);