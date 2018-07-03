'use strict';

app.directive('vrWhsBeAutoimportSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyAPIService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, VRCommon_CurrencyAPIService, VRNotificationService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var purchaseAreaSettings = new PurchaseAreaSettings(ctrl, $scope, $attrs);
				purchaseAreaSettings.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/AutoImportSPLSettingsTemplate.html"
		};

		function PurchaseAreaSettings(ctrl, $scope, $attrs) {
			this.initializeController = initializeController;

			var pricelistTypeMappingAPI;
			var pricelistTypeMappingReadyDeferred = UtilsService.createPromiseDeferred();

			var supplierAutoImportTemplateSettingsAPI;
			var supplierAutoImportTemplateSettingsReadyDeferred = UtilsService.createPromiseDeferred();

			var internalAutoImportTemplateSettingsAPI;
			var internalAutoImportTemplateSettingsReadyDeferred = UtilsService.createPromiseDeferred();

			var data;

			function initializeController() {

				defineAPI();

				ctrl.onPricelistTypeMappingReady = function (api) {
					pricelistTypeMappingAPI = api;
					pricelistTypeMappingReadyDeferred.resolve();
				};

				ctrl.onSupplierAutoImportTemplateSettingsReady = function (api) {
					supplierAutoImportTemplateSettingsAPI = api;
					supplierAutoImportTemplateSettingsReadyDeferred.resolve();
				};

				ctrl.onInternalAutoImportTemplateSettingsReady = function (api) {
					internalAutoImportTemplateSettingsAPI = api;
					internalAutoImportTemplateSettingsReadyDeferred.resolve();
				};
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						data = payload.data;
					}

					var promises = [];
					load();

					function load() {
						loadAllControls();
					}

					function loadAllControls() {
						return UtilsService.waitMultipleAsyncOperations([loadPricelistMapping, loadAutoImportTemplateSettings, loadInternalAutoImportTemplateSettings])
							.catch(function (error) {
								VRNotificationService.notifyExceptionWithClose(error, $scope);
							})
							.finally(function () {
							});
					}

				};

				api.getData = function () {
					return {

						$type: "TOne.WhS.BusinessEntity.Entities.AutoImportSPLSettings, TOne.WhS.BusinessEntity.Entities",
						PricelistTypeMappingList: pricelistTypeMappingAPI.getData(),
						SupplierAutoImportTemplateList: supplierAutoImportTemplateSettingsAPI.getData(),
						InternalAutoImportTemplateList: internalAutoImportTemplateSettingsAPI.getData(),
					};
				};
				function loadAutoImportTemplateSettings() {
					var supplierAutoImportTemplateSettingsLoadDeferred = UtilsService.createPromiseDeferred();
					supplierAutoImportTemplateSettingsReadyDeferred.promise.then(function () {
						var payload = {};
						if (data != undefined) {
							payload.AutoImportTemplateSettings = data.SupplierAutoImportTemplateList;
						}
						VRUIUtilsService.callDirectiveLoad(supplierAutoImportTemplateSettingsAPI, payload, supplierAutoImportTemplateSettingsLoadDeferred);
					});
					return supplierAutoImportTemplateSettingsLoadDeferred.promise;
				}

				function loadInternalAutoImportTemplateSettings() {
					var internalAutoImportTemplateSettingsLoadDeferred = UtilsService.createPromiseDeferred();
					internalAutoImportTemplateSettingsReadyDeferred.promise.then(function () {
						var payload = {};
						if (data != undefined) {
							payload.AutoImportTemplateSettings = data.InternalAutoImportTemplateList;
						}
						VRUIUtilsService.callDirectiveLoad(internalAutoImportTemplateSettingsAPI, payload, internalAutoImportTemplateSettingsLoadDeferred);
					});
					return internalAutoImportTemplateSettingsLoadDeferred.promise;
				}

				function loadPricelistMapping() {
					var pricelistTypeMappingloadDeferred = UtilsService.createPromiseDeferred();
					pricelistTypeMappingReadyDeferred.promise.then(function () {
						var payload;
						if (data != undefined) {
							payload = {
								pricelistTypeMappingList: data.PricelistTypeMappingList
							};
						}
						VRUIUtilsService.callDirectiveLoad(pricelistTypeMappingAPI, payload, pricelistTypeMappingloadDeferred);
					});
					return pricelistTypeMappingloadDeferred.promise;
				}
				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
	}]);