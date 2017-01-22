"use strict";

app.directive("vrWhsSalesMarginpercentageratecalculation", ['WhS_Sales_BulkActionUtilsService', function (WhS_Sales_BulkActionUtilsService) {

    return {
        restrict: "E",
        scope: {
        	onReady: "=",
			isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var marginPercentageRateCalculation = new MarginPercentageRateCalculation(ctrl, $scope);
            marginPercentageRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginPercentageRateCalculationTemplate.html"
    };

    function MarginPercentageRateCalculation(ctrl, $scope) {

    	this.initializeController = initializeController;

    	var bulkActionContext;

        function initializeController() {

        	ctrl.onMarginPercentageChanged = function () {
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
            		ctrl.marginPercentage = rateCalculationMethod.MarginPercentage;
            	}
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginPercentageRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    MarginPercentage: ctrl.marginPercentage
                };
            };

            api.isCostColumnRequired = function () {
                return true;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);