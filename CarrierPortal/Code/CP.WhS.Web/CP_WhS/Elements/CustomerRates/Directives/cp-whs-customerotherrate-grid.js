'use strict';

app.directive('cpWhsCustomerotherrateGrid', ['CP_WhS_CustomerOtherRateAPIService', function (CP_WhS_CustomerOtherRateAPIService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var otherSaleRateGrid = new OtherSaleRateGrid($scope, ctrl, $attrs);
			otherSaleRateGrid.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/CP_WhS/Elements/CustomerRates/Directives/Templates/OtherCustomerRateGridTemplate.html'
	};

	function OtherSaleRateGrid($scope, ctrl, $attrs) {

		var gridAPI;

		this.initializeController = initializeController;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.otherSaleRates = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {

				var query;

				if (payload != undefined) {
					query = payload.query;
				}

				return CP_WhS_CustomerOtherRateAPIService.GetCustomerOtherRates(query).then(function (response) {
					if (response != undefined) {
						for (var i = 0; i < response.length; i++)
							$scope.scopeModel.otherSaleRates.push(response[i]);
					}
				});
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}
}]);