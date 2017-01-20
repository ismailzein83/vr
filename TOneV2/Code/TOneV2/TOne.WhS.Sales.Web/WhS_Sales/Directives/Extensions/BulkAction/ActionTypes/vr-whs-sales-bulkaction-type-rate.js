'use strict';

app.directive('vrWhsSalesBulkactionTypeRate', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var rateBulkActionTypeCtrl = this;
			var rateBulkActionType = new RateBulkActionType($scope, rateBulkActionTypeCtrl, $attrs);
			rateBulkActionType.initializeController();
		},
		controllerAs: "rateBulkActionTypeCtrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionTypes/Templates/RateBulkActionTypeTemplate.html'
	};

	function RateBulkActionType($scope, rateBulkActionTypeCtrl, $attrs) {

		this.initializeController = initializeController;

		var bulkActionContext;

		var costCalculationMethodSelectorAPI;
		var costCalculationMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var rateCalculationMethodSelectorAPI;
		var rateCalculationMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var directiveAPI;
		var directiveReadyDeferred;

		function initializeController() {

			$scope.scopeModel = {};
			$scope.scopeModel.costCalculationMethods = [];
			$scope.scopeModel.rateCalculationMethods = [];

			$scope.scopeModel.onCostCalculationMethodSelectorReady = function (api) {
				costCalculationMethodSelectorAPI = api;
				costCalculationMethodSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.isCostCalculationMethodRequired = function () {
				return (directiveAPI != undefined && directiveAPI.isCostColumnRequired());
			};

			$scope.scopeModel.onRateCalculationMethodSelectorReady = function (api) {
				rateCalculationMethodSelectorAPI = api;
				rateCalculationMethodSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onDirectiveReady = function (api) {
				directiveAPI = api;
				var directivePayload = undefined;
				var setLoader = function (value) {
					$scope.scopeModel.isLoadingDirective = value;
				};
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
			};

			$scope.scopeModel.requireEvaluation = function () {
				if (bulkActionContext != undefined && bulkActionContext.requireEvaluation != undefined)
					bulkActionContext.requireEvaluation();
			};

			UtilsService.waitMultiplePromises([costCalculationMethodSelectorReadyDeferred.promise, rateCalculationMethodSelectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				$scope.scopeModel.isLoading = true;

				costCalculationMethodSelectorAPI.clearDataSource();
				rateCalculationMethodSelectorAPI.clearDataSource();

				var costCalculationMethod;
				var rateCalculationMethod;

				if (payload != undefined) {

					bulkActionContext = payload.bulkActionContext;

					if (payload.bulkAction != undefined) {
						costCalculationMethod = payload.bulkAction.CostCalculationMethod;
						rateCalculationMethod = payload.bulkAction.RateCalculationMethod;
						$scope.scopeModel.beginEffectiveDate = payload.bulkAction.BED;
						$scope.scopeModel.overwrite = payload.bulkAction.OverwriteDraftNewNormalRate;
					}

					if (bulkActionContext.costCalculationMethods != undefined) {
						for (var i = 0; i < bulkActionContext.costCalculationMethods.length; i++)
							$scope.scopeModel.costCalculationMethods.push(bulkActionContext.costCalculationMethods[i]);
					}

					if (costCalculationMethod != undefined)
						$scope.selectedCostCalculationMethod = UtilsService.getItemByVal($scope.scopeModel.costCalculationMethods, costCalculationMethod.ConfigId, 'ConfigId');
				}

				var promises = [];

				var loadRateCalculationMethodExtensionConfigsPromise = loadRateCalculationMethodExtensionConfigs();
				promises.push(loadRateCalculationMethodExtensionConfigsPromise);

				if (rateCalculationMethod != undefined) {
					directiveReadyDeferred = UtilsService.createPromiseDeferred();
					var loadDirectivePromise = loadDirective();
					promises.push(loadDirectivePromise);
				}

				function loadRateCalculationMethodExtensionConfigs() {
					return WhS_Sales_RatePlanAPIService.GetRateCalculationMethodTemplates().then(function (response) {
						if (response != undefined) {
							for (var i = 0; i < response.length; i++) {
								$scope.scopeModel.rateCalculationMethods.push(response[i]);
							}
							if (rateCalculationMethod != undefined)
								$scope.scopeModel.selectedRateCalculationMethod = UtilsService.getItemByVal($scope.scopeModel.rateCalculationMethods, rateCalculationMethod.ConfigId, 'ExtensionConfigurationId');
						}
					});
				}
				function loadDirective() {
					var directiveLoadDeferred = UtilsService.createPromiseDeferred();
					directiveReadyDeferred.promise.then(function () {
						directiveReadyDeferred = undefined;
						var directivePayload = rateCalculationMethod;
						VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
					});
					return directiveLoadDeferred.promise;
				}

				return UtilsService.waitMultiplePromises(promises).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Sales.MainExtensions.RateBulkAction, TOne.WhS.Sales.MainExtensions',
					CostCalculationMethod: $scope.scopeModel.selectedCostCalculationMethod,
					RateCalculationMethod: directiveAPI.getData(),
					BED: UtilsService.getDateFromDateTime($scope.scopeModel.beginEffectiveDate),
					OverwriteDraftNewNormalRate: $scope.scopeModel.overwrite
				};
			};

			if (rateBulkActionTypeCtrl.onReady != null) {
				rateBulkActionTypeCtrl.onReady(api);
			}
		}
	}
}]);