"use strict";

app.directive("vrWhsSalesRoutingproductZone", ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "="
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var zoneRoutingProduct = new ZoneRoutingProduct(ctrl, $scope);
			zoneRoutingProduct.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/ZoneRoutingProductTemplate.html"
	};

	function ZoneRoutingProduct(ctrl, $scope) {
		this.initializeController = initializeController;

		var zoneItem;

		var currentServiceViewerAPI;
		var currentServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

		var selectorAPI;
		var selectorReadyDeferred = UtilsService.createPromiseDeferred();

		var firstSelectionEventDeferred = UtilsService.createPromiseDeferred();
		var rpSelectedDeferred;

		function initializeController() {
			ctrl.onCurrentServiceViewerReady = function (api) {
				currentServiceViewerAPI = api;
				currentServiceViewerReadyDeferred.resolve();
			};

			ctrl.onSelectorReady = function (api) {
				selectorAPI = api;
				selectorReadyDeferred.resolve();
			};

			ctrl.onSelectionChanged = function () {
				var selectedId = selectorAPI.getSelectedIds();

				if (firstSelectionEventDeferred != undefined) {
					firstSelectionEventDeferred = undefined;
					return;
				}

				if (rpSelectedDeferred != undefined) {
					rpSelectedDeferred = undefined;
					return;
				}

				zoneItem.IsDirty = true;
				zoneItem.refreshZoneItem(zoneItem);
			};

			UtilsService.waitMultiplePromises([currentServiceViewerReadyDeferred.promise, selectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {

				var promises = [];
				firstSelectionEventDeferred = UtilsService.createPromiseDeferred();

				var selectedRoutingProductId;

				if (payload != undefined) {

					zoneItem = payload.zoneItem;

					ctrl.currentName = zoneItem.CurrentRoutingProductName;
					if (zoneItem.IsCurrentRoutingProductEditable === false)
						ctrl.currentName += ' (Inherited)';

					if (zoneItem.NewRoutingProduct != null) {
						selectedRoutingProductId = zoneItem.NewRoutingProduct.ZoneRoutingProductId;
						rpSelectedDeferred = UtilsService.createPromiseDeferred();
					}
					else if (zoneItem.ResetRoutingProduct != null) {
						selectedRoutingProductId = -1;
						rpSelectedDeferred = UtilsService.createPromiseDeferred();
					}
				}

				var loadCurrentServicesPromise = loadCurrentServices();
				promises.push(loadCurrentServicesPromise);

				var loadSelectorPromise = loadSelector(selectedRoutingProductId);
				promises.push(loadSelectorPromise);

				return UtilsService.waitMultiplePromises(promises);
			};

			api.applyChanges = function () {
			    setNewRoutingProduct();
				setRoutingProductChange();
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function loadCurrentServices() {
			var currentServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();

			var currentServiceViewerPayload = { selectedIds: zoneItem.CurrentServiceIds };
			VRUIUtilsService.callDirectiveLoad(currentServiceViewerAPI, currentServiceViewerPayload, currentServiceViewerLoadDeferred);

			return currentServiceViewerLoadDeferred.promise;
		}
		function loadSelector(selectedIds) {

			var selectorLoadDeferred = UtilsService.createPromiseDeferred();

			var selectorPayload = {
				selectedIds: selectedIds
			};

			selectorPayload.filter = {
				ExcludedRoutingProductId: zoneItem.CurrentRoutingProductId,
				AssignableToZoneId: zoneItem.ZoneId
			};

			if (zoneItem.IsCurrentRoutingProductEditable === true) {
				selectorPayload.defaultItems = [{
					RoutingProductId: -1,
					Name: '(Reset To Default)'
				}];
			}

			VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
			return selectorLoadDeferred.promise;
		}

		function setNewRoutingProduct() {
			var selectedId = selectorAPI.getSelectedIds();

			if (selectedId && selectedId != -1) {
				zoneItem.NewRoutingProductId = selectedId;
				zoneItem.NewRoutingProductBED = UtilsService.getDateFromDateTime(new Date());
				zoneItem.NewRoutingProductEED = null;
			}
			else {
				zoneItem.NewRoutingProductId = null;
				zoneItem.NewRoutingProductBED = null;
				zoneItem.NewRoutingProductEED = null;
			}
		}
		function setRoutingProductChange() {
			var selectedId = selectorAPI.getSelectedIds();
			zoneItem.RoutingProductChangeEED = (selectedId && selectedId == -1) ? UtilsService.getDateFromDateTime(new Date()) : null;
		}
	}
}]);