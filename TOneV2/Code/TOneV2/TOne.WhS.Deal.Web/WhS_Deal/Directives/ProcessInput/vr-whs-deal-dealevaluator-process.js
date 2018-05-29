"use strict";

app.directive("vrWhsDealDealevaluatorProcess", ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService',
	function (UtilsService, VRUIUtilsService, VRDateTimeService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var ctor = new DealEvaluatorProcessDirectiveConstructor($scope, ctrl);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},
			templateUrl: "/Client/Modules/WhS_Deal/Directives/ProcessInput/Templates/DealEvaluatorProcessTemplate.html"
		};

		function DealEvaluatorProcessDirectiveConstructor($scope, ctrl) {
			this.initializeController = initializeController;
			var today = VRDateTimeService.getNowDateTime();

			function initializeController() {
				defineAPI();
				$scope.validateDealEffectiveAfter = function () {
					if ($scope.dealEffectiveAfter != null && $scope.dealEffectiveAfter > today)
						return "Date can not be greater than Today.";
					return null;
				};
			}
			function defineAPI() {

				var api = {};

				api.load = function (payload) {

				};

				api.getData = function () {

					return {
						InputArguments: {
							$type: "TOne.WhS.Deal.BP.Arguments.DealEvaluatorProcessInput, TOne.WhS.Deal.BP.Arguments",
							DealEffectiveAfter: $scope.dealEffectiveAfter
						}
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
