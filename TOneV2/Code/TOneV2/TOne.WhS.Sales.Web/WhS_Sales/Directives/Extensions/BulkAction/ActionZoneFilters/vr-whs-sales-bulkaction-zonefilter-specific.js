'use strict';

app.directive('vrWhsSalesBulkactionZonefilterSpecific', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var specificBulkActionZoneFilter = new SpecificBulkActionZoneFilter($scope, ctrl, $attrs);
			specificBulkActionZoneFilter.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		template: function (element, attrs) {
			return getTemplate(attrs);
		}
	};

	function SpecificBulkActionZoneFilter($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionContext;

		var selectorAPI;

		var applicableSaleZoneFilter;

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.onSelectorReady = function (api) {
				selectorAPI = api;
				defineAPI();
			};

			$scope.scopeModel.onSelectionChanged = function (selectedSaleZones) {
				if (bulkActionContext != undefined && bulkActionContext.requireEvaluation != undefined)
					bulkActionContext.requireEvaluation();
			};
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var zoneFilter;

				if (payload != undefined) {
					zoneFilter = payload.zoneFilter;
					bulkActionContext = payload.bulkActionContext;
				}

				if (bulkActionContext != undefined)
					extendBulkActionContext();

				return loadSaleZoneSelector();

				function loadSaleZoneSelector() {
					var saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

					applicableSaleZoneFilter = getApplicableSaleZoneFilter();

					var selectorPayload = {};
					selectorPayload.filter = {};
					selectorPayload.filter.Filters = [];
					selectorPayload.filter.Filters.push(applicableSaleZoneFilter);

					if (bulkActionContext != undefined) {
						selectorPayload.sellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
					}

					if (zoneFilter != undefined) {
						selectorPayload.selectedIds = zoneFilter.SelectedZoneIds;
					}
					VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, saleZoneSelectorLoadDeferred);

					function getApplicableSaleZoneFilter() {
						var applicableSaleZoneFilter = {
							$type: 'TOne.WhS.Sales.Business.ApplicableSaleZoneFilter, TOne.WhS.Sales.Business'
						};
						if (bulkActionContext != undefined) {
							applicableSaleZoneFilter.OwnerType = bulkActionContext.ownerType;
							applicableSaleZoneFilter.OwnerId = bulkActionContext.ownerId;
							if (bulkActionContext.getSelectedBulkAction != undefined)
								applicableSaleZoneFilter.ActionType = bulkActionContext.getSelectedBulkAction();
						}
						return applicableSaleZoneFilter;
					}

					return saleZoneSelectorLoadDeferred.promise;
				}
			};

			api.getData = function () {
				var data = {
					$type: 'TOne.WhS.Sales.MainExtensions.SpecificApplicableZones, TOne.WhS.Sales.MainExtensions',
					SelectedZoneIds: selectorAPI.getSelectedIds()
				};
				return data;
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}

		function extendBulkActionContext() {
			if (bulkActionContext == undefined)
				return;

			bulkActionContext.onBulkActionChanged = function () {
				var bulkAction;
				if (bulkActionContext.getSelectedBulkAction != undefined)
					bulkAction = bulkActionContext.getSelectedBulkAction();
				
				applicableSaleZoneFilter.ActionType = bulkAction;
				var selectorPayload = {
					sellingNumberPlanId: bulkActionContext.ownerSellingNumberPlanId,
					filter: {
						Filters: []
					}
				};
				selectorPayload.filter.Filters.push(applicableSaleZoneFilter);
				return selectorAPI.load(selectorPayload);
			};
		}
	}

	function getTemplate(attrs) {
		return '<vr-whs-be-salezone-selector on-ready="scopeModel.onSelectorReady" ismultipleselection onselectionchanged="scopeModel.onSelectionChanged" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" hideremoveicon="ctrl.isrequired"></vr-whs-be-salezone-selector>';
	}
}]);