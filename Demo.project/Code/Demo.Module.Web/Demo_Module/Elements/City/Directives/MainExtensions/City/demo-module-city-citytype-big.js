"use strict";

app.directive("demoModuleCityCitytypeBig", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
	function (UtilsService, VRNotificationService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new BigCity($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/MainExtensions/City/Templates/BigCityTemplate.html"
		};

		function BigCity($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.cityType != undefined) {
						$scope.scopeModel.numberOfCompanies = payload.cityType.NumberOfCompanies;
					}

					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Demo.Module.MainExtension.City.BigCityType, Demo.Module.MainExtension",
						NumberOfCompanies: $scope.scopeModel.numberOfCompanies,
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}
]);