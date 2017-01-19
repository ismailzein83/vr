'use strict';

app.directive('vrWhsSalesBulkactionTypeBed', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var bedBulkActionType = new BEDBulkActionType($scope, ctrl, $attrs);
			bedBulkActionType.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function BEDBulkActionType($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionContext;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.onBEDChanged = function () {
				if (bulkActionContext != undefined && bulkActionContext.requireEvaluation != undefined)
					bulkActionContext.requireEvaluation();
			};

			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var beginEffectiveDate;

				if (payload != undefined) {
					bulkActionContext = payload.bulkActionContext;
					if (payload.bulkAction != undefined)
						beginEffectiveDate = payload.bulkAction.BED;
				}
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Sales.MainExtensions.BEDBulkActionType, TOne.WhS.Sales.MainExtensions',
					BED: $scope.scopeModel.beginEffectiveDate
				};
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}
	}

	function getTemplate(attrs) {
		return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-datetimepicker type="date" label="BED" value="scopeModel.beginEffectiveDate" onvaluechanged="scopeModel.onBEDChanged" isrequired="ctrl.isrequired"></vr-datetimepicker></vr-columns>';
	}
}]);