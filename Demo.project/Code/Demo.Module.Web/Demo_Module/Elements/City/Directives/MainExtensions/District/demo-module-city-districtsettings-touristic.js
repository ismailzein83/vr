"use strict";

app.directive("demoModuleCityDistrictsettingsTouristic", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
	function (UtilsService, VRNotificationService, VRUIUtilsService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new TouristicDistrict($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/MainExtensions/District/Templates/TouristicDistrictTemplate.html"
		};

		function TouristicDistrict($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.districtSettings != undefined) {
						$scope.scopeModel.numberOfPalaces = payload.districtSettings.PalacesNumber;
					}
					var promises = [];
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Demo.Module.MainExtension.District.TouristicDistrict, Demo.Module.MainExtension",
						PalacesNumber: $scope.scopeModel.numberOfPalaces,
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}
]);