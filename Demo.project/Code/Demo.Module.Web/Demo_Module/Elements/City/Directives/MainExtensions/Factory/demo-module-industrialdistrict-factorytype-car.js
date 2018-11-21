"use strict";

app.directive("demoModuleIndustrialdistrictFactorytypeCar", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
	function (UtilsService, VRNotificationService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new CarFactory($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/MainExtensions/Factory/Templates/CarFactoryTemplate.html"
		};

		function CarFactory($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.factoryType != undefined) {
						$scope.scopeModel.carType = payload.factoryType.CarType;
					}
					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Demo.Module.MainExtension.Factory.CarFactory, Demo.Module.MainExtension",
						CarType: $scope.scopeModel.carType,
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}
]);