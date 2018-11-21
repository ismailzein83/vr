"use strict";

app.directive("demoModuleIndustrialdistrictFactorytypeMilitary", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
	function (UtilsService, VRNotificationService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope:{
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new MilitaryFactory($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/MainExtensions/Factory/Templates/MilitaryFactoryTemplate.html"
		};

		function MilitaryFactory($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.factoryType != undefined) {
						$scope.scopeModel.militaryType = payload.factoryType.MilitaryType;
					}
					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Demo.Module.MainExtension.Factory.MilitaryFactory, Demo.Module.MainExtension",
						MilitaryType: $scope.scopeModel.militaryType,
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;

	}
]);