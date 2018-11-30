'use strict';

app.directive('cpWhsCustomerrateHistoryGrid', ['CP_WhS_CustomerRateHistoryAPIService', 'CP_WhS_RateChangeTypeEnum', 'CP_WhS_SalePriceListOwnerTypeEnum','VRNotificationService', function (CP_WhS_CustomerRateHistoryAPIService, CP_WhS_RateChangeTypeEnum, CP_WhS_SalePriceListOwnerTypeEnum,VRNotificationService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var saleRateHistoryGrid = new SaleRateHistoryGrid($scope, ctrl, $attrs);
			saleRateHistoryGrid.initializeController();
		},
		controllerAs: 'ctrl', 
		bindToController: true,
		templateUrl: '/Client/Modules/CP_WhS/Elements/CustomerRates/Directives/Templates/CustomerRateHistoryGridTemplate.html'
	};

	function SaleRateHistoryGrid($scope, ctrl, $attrs) {
		var gridAPI;
		var ownerType;

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.records = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
				return CP_WhS_CustomerRateHistoryAPIService.GetFilteredCustomerRateHistoryRecords(dataRetrievalInput).then(function (response) {
					if (response != undefined && response.Data != null) {
						for (var i = 0; i < response.Data.length; i++) {
							var record = response.Data[i];
							extendRecord(record);
							$scope.scopeModel.records.push(record);
						}
					}
					onResponseReady(response);
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				});
			};
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {
				var query;

				if (payload != undefined) {
					query = payload.query;
				}

				ownerType = (query != undefined) ? query.OwnerType : null;

				$scope.scopeModel.isOwnerCustomer = (ownerType == CP_WhS_SalePriceListOwnerTypeEnum.Customer.value);

				return gridAPI.retrieveData(query);
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function extendRecord(record) {

			setChangeTypeIconProperties();

			function setChangeTypeIconProperties() {
				switch (record.Entity.ChangeType) {
					case CP_WhS_RateChangeTypeEnum.New.value:
						record.changeTypeIconType = CP_WhS_RateChangeTypeEnum.New.iconType;
						record.changeTypeIconUrl = CP_WhS_RateChangeTypeEnum.New.iconUrl;
						record.changeTypeIconDescription = CP_WhS_RateChangeTypeEnum.New.description;
						break;
					case CP_WhS_RateChangeTypeEnum.Increase.value:
						record.changeTypeIconType = CP_WhS_RateChangeTypeEnum.Increase.iconType;
						record.changeTypeIconUrl = CP_WhS_RateChangeTypeEnum.Increase.iconUrl;
						record.changeTypeIconDescription = CP_WhS_RateChangeTypeEnum.Increase.description;
						break;

					case CP_WhS_RateChangeTypeEnum.Decrease.value:
						record.changeTypeIconType = CP_WhS_RateChangeTypeEnum.Decrease.iconType;
						record.changeTypeIconUrl = CP_WhS_RateChangeTypeEnum.Decrease.iconUrl;
						record.changeTypeIconDescription = CP_WhS_RateChangeTypeEnum.Decrease.description;
						break;
					case CP_WhS_RateChangeTypeEnum.NotChanged.value:
						record.changeTypeIconType = CP_WhS_RateChangeTypeEnum.NotChanged.iconType;
						record.changeTypeIconUrl = CP_WhS_RateChangeTypeEnum.NotChanged.iconUrl;
						record.changeTypeIconDescription = CP_WhS_RateChangeTypeEnum.NotChanged.description;
						break;
				}
			}
		}
	}

}]);