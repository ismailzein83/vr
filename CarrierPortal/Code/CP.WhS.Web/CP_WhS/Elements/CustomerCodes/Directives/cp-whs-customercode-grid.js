"use strict";

app.directive("cpWhsCustomercodeGrid", ["UtilsService", "VRNotificationService", "CP_WhS_SaleCodeAPIService",
	function (UtilsService, VRNotificationService, CP_WhS_SaleCodeAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new CustomerCodeGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/CP_WhS/Elements/CustomerCodes/Directives/Templates/CustomerCodeGridTemplate.html'

		};

		function CustomerCodeGrid($scope, ctrl, $attrs) {

			var gridAPI;

			this.initializeController = initializeController;

			function initializeController() {

				$scope.showGrid = false;

				$scope.salecodes = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					$scope.hidesalezonecolumn = false;
					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.loadGrid = function (payload) {
							$scope.hidesalezonecolumn = payload.hidesalezonecolumn;
							return gridAPI.retrieveData(payload.queryHandler);
						};

						return directiveAPI;
					}
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return CP_WhS_SaleCodeAPIService.GetRemoteFilteredCustomerCodes(dataRetrievalInput)
						.then(function (response) {
							$scope.showGrid = true;
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
