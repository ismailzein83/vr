"use strict";

app.directive("vrWhsDealDealevaluatorTask", ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},
			templateUrl: "/Client/Modules/WhS_Deal/Directives/ScheduleTask/Templates/DealEvaluatorTaskTemplate.html"
		};

		function DirectiveConstructor($scope, ctrl) {
			this.initializeController = initializeController;

			function initializeController() {

				defineAPI();
			}
			function defineAPI() {

				var api = {};

				api.getData = function () {
					return {
						$type: "TOne.WhS.Deal.BP.Arguments.DealEvaluatorProcessInput, TOne.WhS.Deal.BP.Arguments"
					};
				};

				api.getExpressionsData = function () {
					return { "ScheduleTime": "ScheduleTime", "DaysBack": $scope.daysBack };
				};

				api.load = function (payload) {
					var promises = [];
					if (payload != null && payload.rawExpressions != null) {
						$scope.daysBack = payload.rawExpressions.DaysBack;
					}
					return UtilsService.waitMultiplePromises(promises);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
