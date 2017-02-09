'use strict';

app.directive('vrWhsSalesBulkactionStepAction', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var actionBulkActionStep = new ActionBulkActionStep($scope, ctrl, $attrs);
			actionBulkActionStep.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Sales/Directives/BulkAction/Steps/Templates/ActionBulkActionStepTemplate.html'
	};

	function ActionBulkActionStep($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionTypeSelectiveAPI;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.onBulkActionTypeSelectiveReady = function (api) {
				bulkActionTypeSelectiveAPI = api;
				defineAPI();
			};
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				return bulkActionTypeSelectiveAPI.load(payload);
			};

			api.getData = function () {
				return {
					bulkAction: bulkActionTypeSelectiveAPI.getData()
				};
			};

			api.getValidationDirectiveName = function () {
			    return bulkActionTypeSelectiveAPI.getValidationDirectiveName();
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}
	}
}]);