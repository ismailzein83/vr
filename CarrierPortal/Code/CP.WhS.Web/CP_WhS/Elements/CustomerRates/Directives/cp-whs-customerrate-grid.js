"use strict";

app.directive("cpWhsCustomerrateGrid", ["VRNotificationService", "CP_WhS_CustomerRateAPIService", "VRUIUtilsService",'CP_WhS_RateChangeTypeEnum',
	function (vrNotificationService, CP_WhS_CustomerRateAPIService, vruiUtilsService,CP_WhS_RateChangeTypeEnum) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new SaleRateGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/CP_WhS/Elements/CustomerRates/Directives/Templates/CustomerRateGridTemplate.html'

		};

		function SaleRateGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridQuery;
			var gridDrillDownTabsObj;
			var primarySaleEntity;

			function initializeController() {
				$scope.showGrid = false;

				$scope.salerates = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					var drillDownDefinitions = getDrillDownDefinitions();
					gridDrillDownTabsObj = vruiUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
					defineAPI();
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return CP_WhS_CustomerRateAPIService.GetFilteredCustomerRates(dataRetrievalInput)
						.then(function (response) {
							if (response && response.Data) {
								for (var i = 0; i < response.Data.length; i++) {
									var item = response.Data[i];
									SetRateChangeIcon(item);
									gridDrillDownTabsObj.setDrillDownExtensionObject(item);
								}
							}
							$scope.showGrid = true;
							onResponseReady(response);
						})
						.catch(function (error) {
							vrNotificationService.notifyException(error, $scope);
						});
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						gridQuery = payload.query;
						primarySaleEntity = payload.primarySaleEntity;
					}
					return gridAPI.retrieveData(gridQuery);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function getDrillDownDefinitions() {

				var drillDownDefinitions = [];

				drillDownDefinitions.push({
					title: 'History',
					directive: 'cp-whs-customerrate-history-grid',
					loadDirective: function (directiveAPI, saleRate) {
						var directivePayload = {
							query: {
								OwnerType: gridQuery.OwnerType,
								OwnerId: gridQuery.OwnerId,
								ZoneName: saleRate.ZoneName,
								CountryId: saleRate.CountryId,
								CurrencyId: gridQuery.CurrencyId
							}
						};
						directivePayload.primarySaleEntity = primarySaleEntity;
						return directiveAPI.load(directivePayload);
					}
				});

				drillDownDefinitions.push({
					title: 'Other Rates',
					directive: 'cp-whs-customerotherrate-grid',
					loadDirective: function (directiveAPI, saleRate) {
						saleRate.otherRateGridAPI = directiveAPI;

						var otherSaleRateGridPayload = {
							query: {
								ZoneName: saleRate.ZoneName,
								ZoneId: saleRate.Entity.ZoneId,
								CountryId: saleRate.CountryId
							}
						};

						if (gridQuery != undefined) {
							otherSaleRateGridPayload.query.OwnerType = gridQuery.OwnerType;
							otherSaleRateGridPayload.query.OwnerId = gridQuery.OwnerId;
							otherSaleRateGridPayload.query.CurrencyId = gridQuery.CurrencyId;
							otherSaleRateGridPayload.query.EffectiveOn = gridQuery.EffectiveOn;
						}
						return directiveAPI.load(otherSaleRateGridPayload);
					}
				});

				drillDownDefinitions.push({
					title: 'Sale Codes',
					directive: 'cp-whs-customercode-grid',
					loadDirective: function (directiveAPI, saleCodes) {
						saleCodes.saleCodesGridAPI = directiveAPI;
						var Query = {
							CustomerId: gridQuery.OwnerId,
							SaleCodeQueryHandlerInfo: {
								ZonesIds: [saleCodes.Entity.ZoneId],
								EffectiveOn: gridQuery.EffectiveOn,
							}
						};
						var saleCodeGridPayload = {
							queryHandler: Query
						};
						return saleCodes.saleCodesGridAPI.loadGrid(saleCodeGridPayload);
					}
				});

				return drillDownDefinitions;
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
		}

		return directiveDefinitionObject;

	}]);
