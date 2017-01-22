'use strict';

app.directive('vrWhsSalesBulkactionTypeEed', ['WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var eedBulkActionType = new EEDBulkActionType($scope, ctrl, $attrs);
			eedBulkActionType.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function EEDBulkActionType($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionContext;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.onEEDChanged = function () {
				WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
			};

			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				if (payload != undefined) {
					bulkActionContext = payload.bulkActionContext;
					if (payload.bulkAction != undefined)
						$scope.scopeModel.endEffectiveDate = payload.bulkAction.EED;
				}
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Sales.MainExtensions.EEDBulkActionType, TOne.WhS.Sales.MainExtensions',
					EED: $scope.scopeModel.endEffectiveDate
				};
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}
	}

	function getTemplate(attrs) {
		return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-datetimepicker type="date" label="EED" value="scopeModel.endEffectiveDate" onvaluechanged="scopeModel.onEEDChanged" isrequired="ctrl.isrequired"></vr-datetimepicker></vr-columns>';
	}
}]);