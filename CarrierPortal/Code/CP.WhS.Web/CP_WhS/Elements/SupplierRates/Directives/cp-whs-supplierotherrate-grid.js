"use strict";

app.directive("cpWhsSupplierotherrateGrid", ["VRNotificationService", "CP_WhS_SupplierOtherRateAPIService",
	function (VRNotificationService, CP_WhS_SupplierOtherRateAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new SupplierRateGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/CP_WhS/Elements/SupplierRates/Directives/Templates/SupplierOtherRateGridTemplate.html'
		};

		function SupplierRateGrid($scope, ctrl, $attrs) {

			var gridAPI;

			this.initializeController = initializeController;

			function initializeController() {

				$scope.supplierrates = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;

					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());

					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.loadGrid = function (query) {

							return gridAPI.retrieveData(query);
						};
						return directiveAPI;
					}
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return CP_WhS_SupplierOtherRateAPIService.GetFilteredSupplierOtherRates(dataRetrievalInput)
						.then(function (response) {
							onResponseReady(response);
						})
						.catch(function (error) {
							VRNotificationService.notifyException(error, $scope);
						});
				};
			}
		}
		return directiveDefinitionObject;
	}]);
