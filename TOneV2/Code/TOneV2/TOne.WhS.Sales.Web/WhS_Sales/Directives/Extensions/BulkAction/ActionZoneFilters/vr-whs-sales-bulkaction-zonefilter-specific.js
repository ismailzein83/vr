'use strict';

app.directive('vrWhsSalesBulkactionZonefilterSpecific', ['WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService) {
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
		var saleZoneCountrySoldToCustomerFilter;

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

						if (bulkActionContext.ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
							saleZoneCountrySoldToCustomerFilter = getSaleZoneCountrySoldToCustomerFilter();
							selectorPayload.filter.Filters.push(saleZoneCountrySoldToCustomerFilter);
						}
					}

					if (zoneFilter != undefined) {
						selectorPayload.selectedIds = zoneFilter.SelectedZoneIds;
					}
					VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, saleZoneSelectorLoadDeferred);

					function getApplicableSaleZoneFilter() {
						var applicableZoneFilter = {
							$type: 'TOne.WhS.Sales.Business.ApplicableSaleZoneFilter, TOne.WhS.Sales.Business'
						};
						if (bulkActionContext != undefined) {
							applicableZoneFilter.OwnerType = bulkActionContext.ownerType;
							applicableZoneFilter.OwnerId = bulkActionContext.ownerId;
							if (bulkActionContext.getSelectedBulkAction != undefined)
								applicableZoneFilter.ActionType = bulkActionContext.getSelectedBulkAction();
						}
						return applicableZoneFilter;
					}
					function getSaleZoneCountrySoldToCustomerFilter() {
						return {
							$type: 'TOne.WhS.Sales.Business.SaleZoneCountrySoldToCustomerFilter, TOne.WhS.Sales.Business',
							CustomerId: bulkActionContext.ownerId,
							EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
							IsEffectiveInFuture: false
						};
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
				if (saleZoneCountrySoldToCustomerFilter != undefined)
					selectorPayload.filter.Filters.push(saleZoneCountrySoldToCustomerFilter);
				return selectorAPI.load(selectorPayload);
			};
		}
	}

	function getTemplate(attrs) {
		return '<vr-whs-be-salezone-selector on-ready="scopeModel.onSelectorReady" ismultipleselection onselectionchanged="scopeModel.onSelectionChanged" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" hideremoveicon="ctrl.isrequired"></vr-whs-be-salezone-selector>';
	}
}]);