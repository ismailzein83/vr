'use strict';

app.directive('vrWhsSalesBulkactionTypeRoutingproduct', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var routingProductBulkActionType = new RoutingProductBulkActionType($scope, ctrl, $attrs);
			routingProductBulkActionType.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function RoutingProductBulkActionType($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionContext;

		var routingProductSelectorAPI;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.onSelectorReady = function (api) {
				routingProductSelectorAPI = api;
				defineAPI();
			};

			$scope.scopeModel.onRoutingProductSelected = function (selectedRoutingProduct) {
				if (bulkActionContext != undefined && bulkActionContext.requireEvaluation != undefined)
					bulkActionContext.requireEvaluation();
			};
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var routingProductId;

				if (payload != undefined) {
					bulkActionContext = payload.bulkActionContext;
					if (payload.bulkAction != undefined)
						routingProductId = payload.bulkAction.RoutingProductId;
				}

				function loadRoutingProductSelector() {
					var routingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
					var routingProductSelectorPayload = {
						filter: null,
						selectedIds: routingProductId
					};
					VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, routingProductSelectorPayload, routingProductSelectorLoadDeferred);
					return routingProductSelectorLoadDeferred.promise;
				}

				return loadRoutingProductSelector();
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Sales.MainExtensions.RoutingProductBulkActionType, TOne.WhS.Sales.MainExtensions',
					RoutingProductId: routingProductSelectorAPI.getSelectedIds()
				};
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}
	}

	function getTemplate(attrs) {
		return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-whs-be-routingproduct-selector on-ready="scopeModel.onSelectorReady" onselectitem="scopeModel.onRoutingProductSelected" isrequired="ctrl.isrequired" hideremoveicon="ctrl.isrequired"></vr-whs-be-routingproduct-selector></vr-columns>';
	}
}]);