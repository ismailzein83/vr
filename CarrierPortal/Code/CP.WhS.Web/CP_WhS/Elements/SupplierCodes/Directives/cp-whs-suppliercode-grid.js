"use strict";

app.directive("cpWhsSuppliercodeGrid", ["VRNotificationService", "CP_WhS_SupplierCodeAPIService",
	function (VRNotificationService, CP_WhS_SupplierCodeAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new SupplierCodeGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/CP_WhS/Elements/SupplierCodes/Directives/Templates/SupplierCodeGridTemplate.html'

		};

		function SupplierCodeGrid($scope, ctrl, $attrs) {

			var gridAPI;
			this.initializeController = initializeController;

			function initializeController() {

				$scope.suppliercodes = [];

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
					return CP_WhS_SupplierCodeAPIService.GetFilteredSupplierCodes(dataRetrievalInput)
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
