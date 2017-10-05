(function (app) {

    'use strict';

    CostMarginRateCalculation.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_Sales_RatePlanAPIService'];

    function CostMarginRateCalculation(UtilsService, VRUIUtilsService, WhS_Sales_RatePlanAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CostMarginRateCalculationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "costMarginRateCalculationCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/PricingTemplate/Templates/CostMarginRateCalculationTemplate.html"

        };
        function CostMarginRateCalculationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var costColumnSelectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.costColumns = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var marginRateCalculation;

                    if (payload != undefined) {
                        marginRateCalculation = payload.marginRateCalculation;
                    }

                    var loadCostColumnSelectorPromise = loadCostColumnSelector();
                    promises.push(loadCostColumnSelectorPromise);

                    function loadCostColumnSelector() {
                        return WhS_Sales_RatePlanAPIService.GetRatePlanSettingsData().then(function (response) {
                            if (response != undefined && response.CostCalculationsMethods != null) {
                                for (var i = 0; i < response.CostCalculationsMethods.length; i++) {
                                    $scope.scopeModel.costColumns.push(response.CostCalculationsMethods[i]);
                                }
                            }
                            if (marginRateCalculation != undefined) {
                                $scope.scopeModel.selectedCostColumn = UtilsService.getItemByVal($scope.scopeModel.costColumns, marginRateCalculation.CostCalculationMethodConfigId, "ConfigId")
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function getData() {
                    var data = {
                        $type: "TOne.WhS.Sales.MainExtensions.PricingTemplateRate.CostMarginRateCalculation, TOne.WhS.Sales.MainExtensions",
                        CostCalculationMethodConfigId: $scope.scopeModel.selectedCostColumn ? $scope.scopeModel.selectedCostColumn.ConfigId : undefined
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsSalesMarginratecalculationCost', CostMarginRateCalculation);

})(app);