(function (app) {

	'use strict';

	ReceivedPricelistPropertyEvaluator.$inject = ['WhS_SupPL_ReceivedPricelistObjectPropertyEnum', 'UtilsService'];

	function ReceivedPricelistPropertyEvaluator(WhS_SupPL_ReceivedPricelistObjectPropertyEnum, UtilsService) {
		return {
			restrict: "E",
			scope: {
				onReady: "=",
				normalColNum: '@',
				label: '@',
				customvalidate: '=',
				isrequired: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var receivedPricelistPropertyEvaluator = new ReceivedPricelistPropertyEvaluator($scope, ctrl, $attrs);
				receivedPricelistPropertyEvaluator.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/MainExtensions/VRObjectTypes/Templates/ReceivedPricelistPropertyEvaluatorTemplate.html"
		};

		function ReceivedPricelistPropertyEvaluator($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.propertyEnums = UtilsService.getArrayEnum(WhS_SupPL_ReceivedPricelistObjectPropertyEnum);
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					if (payload != undefined && payload.objectPropertyEvaluator != undefined) {
						$scope.scopeModel.selectedPropertyEnum = UtilsService.getItemByVal($scope.scopeModel.propertyEnums, payload.objectPropertyEvaluator.ReceivedPricelistField, "value");
					}
				};

				api.getData = function () {

					var data = {
						$type: "TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes.ReceivedPricelistPropertyEvaluator, TOne.WhS.SupplierPriceList.MainExtensions",
						ReceivedPricelistField: $scope.scopeModel.selectedPropertyEnum.value,
					};
					return data;
				};

				if (ctrl.onReady != null) {
					ctrl.onReady(api);
				}
			}

		}
	}

	app.directive('whsSplReceivedpricelistpropertyevaluator', ReceivedPricelistPropertyEvaluator);

})(app);
