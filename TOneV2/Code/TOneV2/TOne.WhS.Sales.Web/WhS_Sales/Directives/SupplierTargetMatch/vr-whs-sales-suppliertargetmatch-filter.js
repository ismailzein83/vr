'use strict';

app.directive('vrWhsSalesSuppliertargetmatchFilter', ['WhS_Sales_MarginTypesEnum', 'UtilsService', 'VRUIUtilsService', 'VRValidationService','VRDateTimeService',
	function (WhS_Sales_MarginTypesEnum, UtilsService, VRUIUtilsService, VRValidationService, VRDateTimeService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var ctor = new SupplierTargetMatchFilter(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/WhS_Sales/Directives/SupplierTargetMatch/Templates/SupplierTargetMatchFilterTemplate.html"
		};

		function SupplierTargetMatchFilter(ctrl, $scope, $attrs) {

			var sellingNumberPlanDirectiveAPI;
			var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var routingProductSelectorAPI;
			var routingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var databaseSelectorAPI;
			var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var countryDirectiveApi;
			var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var calculationMethodDirectiveAPI;
			var calculationMethodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var marginTypeSelectorAPI;
			var marginTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
			var policySelectorAPI;

			function initializeController() {

				$scope.scopeModel = {};

				$scope.scopeModel.numberOfOptions = 3;
				$scope.scopeModel.margin = 0;
				$scope.scopeModel.volumeMultiplier = 1;
				$scope.scopeModel.onSellingNumberPlansDirectiveReady = function (api) {
					sellingNumberPlanDirectiveAPI = api;
					sellingNumberPlanReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onRoutingProductSelectorReady = function (api) {
					routingProductSelectorAPI = api;
					routingProductReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onCountryDirectiveReady = function (api) {
					countryDirectiveApi = api;
					countryReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onDatabaseSelectorReady = function (api) {
					databaseSelectorAPI = api;
					databaseSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onMarginTypeSelectorReady = function (api) {
					marginTypeSelectorAPI = api;
					marginTypeReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onRoutingDatabaseChanged = function () {

					var selectedId = databaseSelectorAPI.getSelectedIds();

					if (selectedId == undefined)
						return;

					var policySelectorPayload = {
						filter: {
							RoutingDatabaseId: selectedId
						},
						selectDefaultPolicy: true
					};

					var setLoader = function (value) {
						$scope.isLoadingFilterSection = value;
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, policySelectorAPI, policySelectorPayload, setLoader, undefined);
				};

				$scope.scopeModel.onPolicySelectorReady = function (api) {
					policySelectorAPI = api;
				};

				$scope.scopeModel.onCalculationMethodDirectiveReady = function (api) {
					calculationMethodDirectiveAPI = api;
					calculationMethodReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onSelectSellingNumberPlan = function (option) {
					if (option != undefined) {
						var direcivePayload = {
							filter: {
								SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
							}
						};
						var setLoader = function (value) { $scope.scopeModel.isRoutingProductSelectorLoading = value; };
						VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routingProductSelectorAPI, direcivePayload, setLoader, undefined);
					}
					else {
						routingProductSelectorAPI.clearDataSource();
					}
				};
				$scope.scopeModel.onMarginTypeSelectorChanged = function (option) {
					if (option != undefined) {
						$scope.scopeModel.isPercentage = option.value == WhS_Sales_MarginTypesEnum.Percentage.value ? true : false;
					}

				};
				$scope.scopeModel.validateToDate = function () {
					var result = VRValidationService.validateTimeRange($scope.scopeModel.from, $scope.scopeModel.to);
					if (result != null)
						return result;

					//if ($scope.scopeModel.to != undefined && $scope.scopeModel.from != undefined && $scope.scopeModel.to.getTime() == $scope.scopeModel.from.getTime()) {
					//	return 'From date should be less than To date';
					//}
					return null;
				};

				$scope.scopeModel.validateFromDate = function () {
					return VRValidationService.validateTimeRange($scope.scopeModel.from, $scope.scopeModel.to);
				};
				defineAPI();

			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {


					var promises = [];

					promises.push(loadSellingNPSelector());
					promises.push(routingProductReadyPromiseDeferred.promise);
					promises.push(loadRoutingDatabaseSelector());
					promises.push(loadCountrySelector());
					promises.push(loadCalculationMethodSelective());
					promises.push(loadMarginTypeSelector());

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var obj = {
						$type: "TOne.WhS.Sales.Entities.SupplierTargetMatchQuery, TOne.WhS.Sales.Entities",
						SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
						RoutingProductId: routingProductSelectorAPI.getSelectedIds(),
						RoutingDataBaseId: databaseSelectorAPI.getSelectedIds(),
						CountryIds: countryDirectiveApi.getSelectedIds(),
						PolicyId: policySelectorAPI.getSelectedIds(),
						NumberOfOptions: $scope.scopeModel.numberOfOptions,
						From: $scope.scopeModel.from,
						To: $scope.scopeModel.to,
						DefaultVolume: $scope.scopeModel.defaultVolume,
						DefaultASR: $scope.scopeModel.defaultASR,
						DefaultACD: $scope.scopeModel.defaultACD,
						IncludeACD_ASR: $scope.scopeModel.withACD_ASR,
						VolumeMultiplier: $scope.scopeModel.volumeMultiplier,
						MarginType: $scope.scopeModel.selectedMarginType.value,
						MarginValue: $scope.scopeModel.margin,
						CalculationMethod: calculationMethodDirectiveAPI.getData()
					};

					return obj;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function loadSellingNPSelector() {
				var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();

				sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
					VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSNPPromiseDeferred);
				});

				return loadSNPPromiseDeferred.promise;
			}

			function loadRoutingDatabaseSelector() {
				var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

				databaseSelectorReadyDeferred.promise.then(function () {
					VRUIUtilsService.callDirectiveLoad(databaseSelectorAPI, undefined, loadRoutingDatabasePromiseDeferred);
				});

				return loadRoutingDatabasePromiseDeferred.promise;
			}

			function loadMarginTypeSelector() {
				var loadMarginTypePromiseDeferred = UtilsService.createPromiseDeferred();
				marginTypeReadyPromiseDeferred.promise.then(function () {
					$scope.scopeModel.marginTypes = UtilsService.getArrayEnum(WhS_Sales_MarginTypesEnum);
					$scope.scopeModel.selectedMarginType = UtilsService.getItemByVal($scope.scopeModel.marginTypes, WhS_Sales_MarginTypesEnum.Fixed.value, 'value');
					loadMarginTypePromiseDeferred.resolve();
				});
				return loadMarginTypePromiseDeferred.promise;
			}

			function loadCountrySelector() {
				var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
				countryReadyPromiseDeferred.promise.then(function () {
					VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, undefined, countryLoadPromiseDeferred);
				});
				return countryLoadPromiseDeferred.promise;
			}

			function loadCalculationMethodSelective() {
				var loadPromiseDeferred = UtilsService.createPromiseDeferred();
				calculationMethodReadyPromiseDeferred.promise.then(function () {
					VRUIUtilsService.callDirectiveLoad(calculationMethodDirectiveAPI, undefined, loadPromiseDeferred);
				});
				return loadPromiseDeferred.promise;
			}


			this.initializeController = initializeController;
		}

		return directiveDefinitionObject;
	}]);