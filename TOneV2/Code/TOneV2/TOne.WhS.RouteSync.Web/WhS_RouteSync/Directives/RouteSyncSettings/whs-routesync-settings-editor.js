'use strict';

app.directive('whsRoutesyncSettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_SwitchRouteSynchronizerAPIService',
	function (UtilsService, VRUIUtilsService, WhS_RouteSync_SwitchRouteSynchronizerAPIService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/RouteSyncSettings/Templates/RouteSyncSettingsTemplate.html"
		};

		function settingEditorCtor(ctrl, $scope, $attrs) {
			var switchSettingsGridReadyPromise = UtilsService.createPromiseDeferred();

			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.switchConfigs = [];
				$scope.scopeModel.switchSettings = [];
				$scope.scopeModel.selectedSwitchConfigs = [];
				$scope.scopeModel.switchConfigSettingsIsLoading = false;

				$scope.scopeModel.onSelectSwitchConfig = function (addedSwitchConfig) {
					$scope.scopeModel.switchConfigSettingsIsLoading = true;
					var switchSettingsDeferred = UtilsService.createPromiseDeferred();
					extendSwitchConfig(addedSwitchConfig, switchSettingsDeferred);
					$scope.scopeModel.switchSettings.push(addedSwitchConfig);

					switchSettingsDeferred.promise.then(function () {
						$scope.scopeModel.switchConfigSettingsIsLoading = false;
					});
				};

				$scope.scopeModel.onDeselectSwitchConfig = function (deletedSwitchConfig) {

					var index = UtilsService.getItemIndexByVal($scope.scopeModel.switchSettings, deletedSwitchConfig.ConfigId, 'ConfigId');
					$scope.scopeModel.switchSettings.splice(index, 1);
					deletedSwitchConfig.Settings = undefined;
				};

				$scope.scopeModel.onSwitchSettingsGridReady = function () {
					switchSettingsGridReadyPromise.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					var promises = [];
					var getSwitchSynchronizerSettingsTemplateConfigsPromise = getSwitchSynchronizerSettingsSwitchConfigs();
					promises.push(getSwitchSynchronizerSettingsTemplateConfigsPromise);

					var switchSettingsByConfigId;
					if (payload != undefined && payload.data != undefined) {
						$scope.scopeModel.routeBatchSize = payload.data.RouteSyncProcess.RouteBatchSize;
						$scope.scopeModel.differentialRoutesPerTransaction = payload.data.RouteSyncProcess.DifferentialRoutesPerTransaction;
						$scope.scopeModel.indexCommandTimeoutInMinutes = payload.data.RouteSyncProcess.IndexCommandTimeoutInMinutes;
						$scope.scopeModel.executeFullRouteSyncWhenPartialNotSupported = payload.data.RouteSyncProcess.ExecuteFullRouteSyncWhenPartialNotSupported;
						switchSettingsByConfigId = payload.data.SwitchSettingsByConfigId;
					}

					var switchSettingsGridLoadPromise = UtilsService.createPromiseDeferred();
					promises.push(switchSettingsGridLoadPromise.promise);

					function getSwitchSynchronizerSettingsSwitchConfigs() {
						return WhS_RouteSync_SwitchRouteSynchronizerAPIService.GetSwitchRouteSynchronizerHaveSettingsExtensionConfigs().then(function (response) {
							if (response != null) {
								for (var i = 0; i < response.length; i++) {
									$scope.scopeModel.switchConfigs.push(response[i]);
								}

								if (switchSettingsByConfigId != undefined)
									getSwitchSettingsGridLoadPromise(switchSettingsByConfigId).then(function () { switchSettingsGridLoadPromise.resolve(); });
								else switchSettingsGridLoadPromise.resolve();
							}
						});
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var switchSettingsByConfigId = {};
					if ($scope.scopeModel.switchSettings != undefined && $scope.scopeModel.switchSettings.length > 0) {
						for (var i = 0; i < $scope.scopeModel.switchSettings.length; i++) {
							var configId = $scope.scopeModel.switchSettings[i].ExtensionConfigurationId;
							var value = $scope.scopeModel.switchSettings[i].switchDirectiveWrapperAPI.getData();
							switchSettingsByConfigId[configId] = value;
						}
					}
					var data = {
						$type: "TOne.WhS.RouteSync.Entities.RouteSyncSettings, TOne.WhS.RouteSync.Entities",
						RouteSyncProcess: {
							RouteBatchSize: $scope.scopeModel.routeBatchSize,
							DifferentialRoutesPerTransaction: $scope.scopeModel.differentialRoutesPerTransaction,
							IndexCommandTimeoutInMinutes: $scope.scopeModel.indexCommandTimeoutInMinutes,
							ExecuteFullRouteSyncWhenPartialNotSupported: $scope.scopeModel.executeFullRouteSyncWhenPartialNotSupported
						},
						SwitchSettingsByConfigId: (Object.keys(switchSettingsByConfigId).length > 0) ? switchSettingsByConfigId : null,
					};
					return data;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function getSwitchSettingsGridLoadPromise(switchSettingsByConfigId) {
				var switchSettingsGridLoadPromise = UtilsService.createPromiseDeferred();

				switchSettingsGridReadyPromise.promise.then(function () {
					var promises = [];
					for (var configId in switchSettingsByConfigId) {
						if (configId != "$type") {
							var switchConfig = UtilsService.getItemByVal($scope.scopeModel.switchConfigs, configId, 'ExtensionConfigurationId');
							$scope.scopeModel.selectedSwitchConfigs.push(switchConfig);

							switchConfig.Settings = switchSettingsByConfigId[configId];

							var switchSettingsDeferred = UtilsService.createPromiseDeferred();
							promises.push(switchSettingsDeferred.promise);
							extendSwitchConfig(switchConfig, switchSettingsDeferred);
							$scope.scopeModel.switchSettings.push(switchConfig);
						}
					}

					UtilsService.waitMultiplePromises(promises).then(function () {
						switchSettingsGridLoadPromise.resolve();
					});
				});

				return switchSettingsGridLoadPromise.promise;
			}

			function extendSwitchConfig(switchConfig, switchConfigLoadDirectivesDeferred) {

				var switchConfigDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
				switchConfig.onDirectiveWrapperReady = function (api) {
					switchConfig.switchDirectiveWrapperAPI = api;
					var directiveWrapperPayload;
					if (switchConfig.Settings != undefined) {
						directiveWrapperPayload = { settings: switchConfig.Settings };
					}
					VRUIUtilsService.callDirectiveLoad(switchConfig.switchDirectiveWrapperAPI, directiveWrapperPayload, switchConfigDirectiveLoadDeferred);
				};

				switchConfigDirectiveLoadDeferred.promise.then(function () {
					switchConfigLoadDirectivesDeferred.resolve();
				});
			}
		}

		return directiveDefinitionObject;
	}]);