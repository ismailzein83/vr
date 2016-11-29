'use strict';

app.directive('vrWhsSalesSoldcountryGrid', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRNotificationService', 'VRValidationService', function (WhS_Sales_RatePlanAPIService, WhS_Sales_RatePlanUtilsService, UtilsService, VRNotificationService, VRValidationService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var soldCountryGrid = new SoldCountryGrid($scope, ctrl, $attrs);
			soldCountryGrid.initializeController();
		},
		controllerAs: 'soldCountryCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Sales/Directives/Country/Templates/SoldCountryGridTemplate.html'
	};

	function SoldCountryGrid($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;
		var selectedCountryIds;
		var effectiveDateDayOffset;

		function initializeController() {

			$scope.scopeModel = {};
			$scope.scopeModel.soldCountries = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
				return WhS_Sales_RatePlanAPIService.GetFilteredSoldCountries(dataRetrievalInput).then(function (response) {
					if (response != null && response.Data != null) {
						for (var i = 0; i < response.Data.length; i++) {
							var soldCountry = response.Data[i];

							soldCountry.beginEffectiveDate = UtilsService.getShortDate(new Date(soldCountry.Entity.BED));
							if (soldCountry.Entity.EED != null)
								soldCountry.endEffectiveDate = UtilsService.getShortDate(new Date(soldCountry.Entity.EED));

							if (UtilsService.contains(selectedCountryIds, soldCountry.Entity.CountryId))
								soldCountry.isSelected = true;
						}
					}
					onResponseReady(response);
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			};

			$scope.scopeModel.isEEDRequired = function () {
				return (selectedCountryIds != undefined && selectedCountryIds.length > 0);
			};

			$scope.scopeModel.isEEDValid = function () {
				if (!$scope.scopeModel.isEEDRequired())
					return null;
				if ($scope.scopeModel.endEffectiveDate == undefined)
					return 'EED is a required field';
				var today = UtilsService.getDateFromDateTime(new Date());
				if ($scope.scopeModel.endEffectiveDate < today)
					return 'EED must be >= today';
				return null;
			};

			$scope.scopeModel.onCheckValueChanged = function (soldCountry) {

				if (soldCountry.isSelected === true) {
					selectedCountryIds.push(soldCountry.Entity.CountryId);
				}
				else if (soldCountry.isSelected === false) {
					var index = selectedCountryIds.indexOf(soldCountry.Entity.CountryId);
					selectedCountryIds.splice(index, 1);
				}
			};

			$scope.scopeModel.validateTimeRange = function (soldCountry) {
				if (!soldCountry.isSelected)
					return null;
				var soldCountryBED = UtilsService.createDateFromString(soldCountry.Entity.BED);
				return VRValidationService.validateTimeRange(soldCountryBED, $scope.scopeModel.endEffectiveDate);
			};
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var query;
				var changedCountries;
				var settings;

				if (payload != undefined) {
					query = payload.query;
					changedCountries = payload.changedCountries;
					settings = payload.settings;
				}

				effectiveDateDayOffset = 0;
				if (settings != undefined) {
					if (settings.effectiveDateDayOffset != undefined) {
						var effectiveDateDayOffsetValue = Number(settings.effectiveDateDayOffset);
						if (!isNaN(effectiveDateDayOffsetValue))
							effectiveDateDayOffset = effectiveDateDayOffsetValue;
					}
				}

				if (changedCountries != undefined) {
					$scope.scopeModel.endEffectiveDate = changedCountries.EED;
					selectedCountryIds = changedCountries.CountryIds;
				}
				else {
					$scope.scopeModel.endEffectiveDate = WhS_Sales_RatePlanUtilsService.getNowPlusDays(effectiveDateDayOffset);
					selectedCountryIds = [];
				}

				return gridAPI.retrieveData(query);
			};

			api.getData = function () {

				if (selectedCountryIds.length == 0)
					return null;

				return {
					CountryIds: selectedCountryIds,
					EED: $scope.scopeModel.endEffectiveDate
				};
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}
	}
}]);