"use strict";

app.directive("cpWhsSupplierrateGrid", ["VRNotificationService", "VRUIUtilsService","CP_WhS_SupplierRateAPIService", "CP_WhS_SupplierRateHistoryAPIService", "CP_WhS_RateChangeTypeEnum",
	function (VRNotificationService, VRUIUtilsService,CP_WhS_SupplierRateAPIService, CP_WhS_SupplierRateHistoryAPIService, CP_WhS_RateChangeTypeEnum) {
		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new SupplierRateGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/CP_WhS/Elements/SupplierRates/Directives/Templates/SupplierRateGridTemplate.html'

		};

		function SupplierRateGrid($scope, ctrl, $attrs) {

			var gridAPI;
			var drillDownManager;
			var effectiveOn;
			var supplierId;
			var isSystemCurrency;
			var isExpandable;
			var isHistory;
			var supplierZoneName;
			var countriesIds;

			this.initializeController = initializeController;

			function initializeController() {

				$scope.supplierrates = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;

					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.load = function (payload) {
							isHistory = payload.IsHistory;
							supplierId = payload.SupplierId;
							isExpandable = payload.IsChild;
							effectiveOn = payload.EffectiveOn;
							countriesIds = payload.CountriesIds;
							supplierZoneName = payload.SupplierZoneName;

							var supplierRate = {
								EffectiveOn: effectiveOn,
								SupplierId: supplierId,
								SupplierZoneName: supplierZoneName,
							};

							if (!isHistory)
								supplierRate.CountriesIds = countriesIds;

						drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(getDirectiveTabs(), gridAPI);
							return gridAPI.retrieveData(supplierRate);
						};
						return directiveAPI;
					}
				};

				$scope.isExpandable = function () {
					return !isExpandable;
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					if (!isHistory) {
						return CP_WhS_SupplierRateAPIService.GetSupplierRateQueryHandlerInfo(dataRetrievalInput)
							.then(function (response) {
								if (response && response.Data) {
									for (var i = 0; i < response.Data.length; i++) {
										var item = response.Data[i];
										SetRateChangeIcon(item);
										drillDownManager.setDrillDownExtensionObject(item);
									}
								}
								onResponseReady(response);
							})
							.catch(function (error) {
								VRNotificationService.notifyException(error, $scope);
							});
					}
					else {
						return CP_WhS_SupplierRateHistoryAPIService.GetSupplierRateHistoryQueryHandlerInfo(dataRetrievalInput)
							.then(function (response) {
								if (response && response.Data) {
									for (var i = 0; i < response.Data.length; i++) {
										var item = response.Data[i];
										SetRateChangeIcon(item);
									}
								}
								onResponseReady(response);
							})
							.catch(function (error) {
								VRNotificationService.notifyException(error, $scope);
							});
					}
				};
			}

			function SetRateChangeIcon(dataItem) {
				switch (dataItem.Entity.RateChange) {
					case CP_WhS_RateChangeTypeEnum.New.value:
						dataItem.RateChangeTypeIcon = CP_WhS_RateChangeTypeEnum.New.iconUrl;
						dataItem.RateChangeTypeIconTooltip = CP_WhS_RateChangeTypeEnum.New.description;
						dataItem.RateChangeTypeIconType = CP_WhS_RateChangeTypeEnum.New.iconType;
						break;
					case CP_WhS_RateChangeTypeEnum.Increase.value:
						dataItem.RateChangeTypeIcon = CP_WhS_RateChangeTypeEnum.Increase.iconUrl;
						dataItem.RateChangeTypeIconTooltip = CP_WhS_RateChangeTypeEnum.Increase.description;
						dataItem.RateChangeTypeIconType = CP_WhS_RateChangeTypeEnum.Increase.iconType;
						break;

					case CP_WhS_RateChangeTypeEnum.Decrease.value:
						dataItem.RateChangeTypeIcon = CP_WhS_RateChangeTypeEnum.Decrease.iconUrl;
						dataItem.RateChangeTypeIconTooltip = CP_WhS_RateChangeTypeEnum.Decrease.description;
						dataItem.RateChangeTypeIconType = CP_WhS_RateChangeTypeEnum.Decrease.iconType;
						break;
					case CP_WhS_RateChangeTypeEnum.NotChanged.value:
						dataItem.RateChangeTypeIcon = CP_WhS_RateChangeTypeEnum.NotChanged.iconUrl;
						dataItem.RateChangeTypeIconTooltip = CP_WhS_RateChangeTypeEnum.NotChanged.description;
						dataItem.RateChangeTypeIconType = CP_WhS_RateChangeTypeEnum.NotChanged.iconType;
						break;
				}
			}

			function getDirectiveTabs() {
				var directiveTabs = [];

				var historyRatesTab = {
					title: "History",
					directive: "cp-whs-supplierrate-grid",
					loadDirective: function (directiveApi, rateDataItem) {
						rateDataItem.historygRateGridAPI = directiveApi;
						var historyRateGridPayload = {
							IsHistory: true,
							EffectiveOn: effectiveOn,
							SupplierZoneName: rateDataItem.SupplierZoneName,
							SupplierId: supplierId,
							IsChild: true
						};
						return rateDataItem.historygRateGridAPI.load(historyRateGridPayload);
					}
				};
				directiveTabs.push(historyRatesTab);

				var otherRatesTab = {
					title: "Other Rates",
					directive: "cp-whs-supplierotherrate-grid",
					loadDirective: function (directiveAPI, rateDataItem) {
						rateDataItem.otherRateGridAPI = directiveAPI;

						var otherRateGridPayload = {
							SupplierId: supplierId,
							SupplierOtherRateQuery: {
								ZoneId: rateDataItem.Entity.ZoneId,
								EffectiveOn: effectiveOn,
								IsSystemCurrency: isSystemCurrency
							}
						};
						return rateDataItem.otherRateGridAPI.loadGrid(otherRateGridPayload);
					}
				};
				directiveTabs.push(otherRatesTab);

				var supplierCodeTab = {
					title: "Supplier Codes",
					directive: "cp-whs-suppliercode-grid",
					loadDirective: function (directiveAPI, codeDataItem) {
						codeDataItem.codeGridAPI = directiveAPI;
						var codeGridPayload = {
							SupplierId: codeDataItem.SupplierId,
							ZoneIds: [codeDataItem.Entity.ZoneId],
							EffectiveOn: effectiveOn
						};
						return codeDataItem.codeGridAPI.loadGrid(codeGridPayload);
					}
				};
				directiveTabs.push(supplierCodeTab);

				return directiveTabs;
			}
		}

		return directiveDefinitionObject;

	}]);
