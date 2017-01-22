"use strict";

app.directive("vrWhsSalesFixedratecalculation", ['WhS_Sales_BulkActionUtilsService', function (WhS_Sales_BulkActionUtilsService) {

    return {
        restrict: "E",
        scope: {
        	onReady: "=",
        	isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var fixedRateCalculation = new FixedRateCalculation(ctrl, $scope);
            fixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/FixedRateCalculationTemplate.html"
    };

    function FixedRateCalculation(ctrl, $scope) {
        
    	this.initializeController = initializeController;

    	var bulkActionContext;

        function initializeController() {

        	ctrl.onFixedRateChanged = function () {
        		WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
        	};

        	defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

            	var rateCalculationMethod;

                if (payload != undefined) {
                	rateCalculationMethod = payload.rateCalculationMethod;
                	bulkActionContext = payload.bulkActionContext;
                }

                if (rateCalculationMethod != undefined) {
                	ctrl.fixedRate = rateCalculationMethod.FixedRate;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.FixedRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    FixedRate: ctrl.fixedRate
                };
            };

            api.isCostColumnRequired = function () {
                return false;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);