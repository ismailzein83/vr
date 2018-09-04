'use strict';

app.directive('vrWhsBeCostwithpercentagemarginrateevaluator', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService',
	function (UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService) {
	    return {
	        restrict: 'E',
	        scope: {
	            onReady: '=',
	            normalColNum: '@'
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new CostWithMarginRateEvaluatorCtor(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'CostWithMarginRateEvaluatorCtrl',
	        bindToController: true,
	        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/PassThroughCustomerRateEvaluators/Templates/CostWithMarginRateEvaluatorTemplate.html"
	    };

	    function CostWithMarginRateEvaluatorCtor(ctrl, $scope, attrs) {
	        this.initializeController = initializeController;

	        function initializeController() {
	            $scope.scopeModel = {};

	            defineAPI();
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {

	                if (payload != undefined) {
	                    $scope.scopeModel.percentage = payload.Percentage;
	                }

	                var promises = [];

	                return UtilsService.waitMultiplePromises(promises);
	            };

	            api.getData = function () {
	                return {
	                    $type: "TOne.WhS.BusinessEntity.MainExtensions.PassThroughCustomerRateEvaluator.CostWithPercentageMarginRateEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
	                    Percentage: $scope.scopeModel.percentage
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	}]);