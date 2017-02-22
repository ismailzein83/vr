"use strict";

app.directive("vrWhsSalesMarginratecalculation", ['WhS_Sales_BulkActionUtilsService', function (WhS_Sales_BulkActionUtilsService) {

    return {
        restrict: "E",
        scope: {
        	onReady: "=",
			isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mixedRateCalculation = new MarginRateCalculation(ctrl, $scope);
            mixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginRateCalculationTemplate.html"
    };

    function MarginRateCalculation(ctrl, $scope) {

        this.initializeController = initializeController;

        var bulkActionContext;

        function initializeController() {

        	ctrl.onMarginChanged = function () {
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
            		ctrl.margin = rateCalculationMethod.Margin;
            	}
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    Margin: ctrl.margin
                };
            };

            api.isCostColumnRequired = function () {
                return true;
            };

            api.getDescription = function () {
                return ctrl.margin;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);