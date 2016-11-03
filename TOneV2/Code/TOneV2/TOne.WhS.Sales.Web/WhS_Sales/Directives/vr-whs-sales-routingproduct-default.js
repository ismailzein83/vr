"use strict";

app.directive("vrWhsSalesRoutingproductDefault", ["WhS_BE_SalePriceListOwnerTypeEnum", "UtilsService", "VRUIUtilsService",
function (WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService) {

	return {
		restrict: "E",
		scope: {
			onReady: "="
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var defaultRoutingProduct = new DefaultRoutingProduct(ctrl, $scope);
			defaultRoutingProduct.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: "/Client/Modules/WhS_Sales/Directives/Templates/DefaultRoutingProductTemplate.html"
	};

	function DefaultRoutingProduct(ctrl, $scope) {

		this.initializeController = initializeController;

		var defaultItem;
		var context;

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

				defaultItem.IsDirty = true;
				context.saveDraft(true);
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

					defaultItem = payload.defaultItem;
					context = payload.context;

					ctrl.currentName = defaultItem.CurrentRoutingProductName;
					if (defaultItem.IsCurrentRoutingProductEditable === false)
						ctrl.currentName += ' (Inherited)';

					if (defaultItem.NewRoutingProduct != null) {
						selectedRoutingProductId = defaultItem.NewRoutingProduct.DefaultRoutingProductId;
						rpSelectedDeferred = UtilsService.createPromiseDeferred();
					}
					else if (defaultItem.ResetRoutingProduct != null) {
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

			api.applyChanges = function (defaultChanges) {
				setNewDefaultRoutingProduct(defaultChanges);
				setDefaultRoutingProductChange(defaultChanges);

				function setNewDefaultRoutingProduct(defaultChanges) {
					var selectedId = selectorAPI.getSelectedIds();

					if (selectedId && selectedId != -1) {
						defaultChanges.NewDefaultRoutingProduct = {
							DefaultRoutingProductId: selectedId,
							BED: UtilsService.getDateFromDateTime(new Date()),
							EED: null
						};
					}
				}

				function setDefaultRoutingProductChange(defaultChanges) {
					var selectedId = selectorAPI.getSelectedIds();

					defaultChanges.DefaultRoutingProductChange = (selectedId && selectedId == -1) ? {
						DefaultRoutingProductId: defaultItem.CurrentRoutingProductId,
						EED: UtilsService.getDateFromDateTime(new Date())
					} : null;
				}
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function loadCurrentServices() {
			var currentServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();

			var currentServiceViewerPayload = { selectedIds: defaultItem.CurrentServiceIds };
			VRUIUtilsService.callDirectiveLoad(currentServiceViewerAPI, currentServiceViewerPayload, currentServiceViewerLoadDeferred);

			return currentServiceViewerLoadDeferred.promise;
		}
		function loadSelector(selectedIds) {

			var selectorLoadDeferred = UtilsService.createPromiseDeferred();

			var selectorPayload = {
				selectedIds: selectedIds
			};

			selectorPayload.filter = {
				ExcludedRoutingProductId: defaultItem.CurrentRoutingProductId,
				AssignableToOwnerType: defaultItem.OwnerType,
				AssignableToOwnerId: defaultItem.OwnerId
			};

			if (defaultItem.OwnerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value && defaultItem.IsCurrentRoutingProductEditable === true) {
				selectorPayload.defaultItems = [{
					RoutingProductId: -1,
					Name: '(Reset To Default)'
				}];
			}

			VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
			return selectorLoadDeferred.promise;
		}
	}
}]);
