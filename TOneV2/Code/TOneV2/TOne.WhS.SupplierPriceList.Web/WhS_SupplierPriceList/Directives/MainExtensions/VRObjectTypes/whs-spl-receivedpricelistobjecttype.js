(function (app) {

	'use strict';

	ReceivedPricelistObjectType.$inject = [];

	function ReceivedPricelistObjectType() {
		return {
			restrict: "E",
			scope: {
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var receivedPricelistObjectType = new ReceivedPricelistObjectType($scope, ctrl, $attrs);
				receivedPricelistObjectType.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/MainExtensions/VRObjectTypes/Templates/ReceivedPricelistObjectTypeTemplate.html"
		};
		function ReceivedPricelistObjectType($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {

				};

				api.getData = function () {
					var data = {
						$type: "TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistObjectType, TOne.WhS.SupplierPriceList.MainExtensions",
					};
					return data;
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsSplReceivedpricelistobjecttype', ReceivedPricelistObjectType);

})(app);