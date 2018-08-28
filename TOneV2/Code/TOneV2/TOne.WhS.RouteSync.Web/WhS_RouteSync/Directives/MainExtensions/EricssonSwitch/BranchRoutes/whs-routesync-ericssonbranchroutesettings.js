(function (app) {

	'use strict';

	whsRoutesyncEricssonbranchroutesettings.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonBranchRouteSettingsAPIService'];

	function whsRoutesyncEricssonbranchroutesettings(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonBranchRouteSettingsAPIService) {
		return {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new BranchRouteSettingsCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/BranchRoutes/Templates/EricssonSwitchBranchRoutesSettingsTemplate.html"

		};
		function BranchRouteSettingsCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var branchRouteSettingsAPI;
			var branchRouteSettingsReadyDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};
				var initPromises = [];

				$scope.scopeModel.onBranchRouteSettingsReady = function (api) {
					branchRouteSettingsAPI = api;
					branchRouteSettingsReadyDeferred.resolve();
				};

				var loadBranchRouteSettingsConfigsDeferred = UtilsService.createPromiseDeferred();
				initPromises.push(loadBranchRouteSettingsConfigsDeferred.promise);

				WhS_RouteSync_EricssonBranchRouteSettingsAPIService.GetEricssonBranchRouteSettingsConfigs().then(function (response) {
					$scope.scopeModel.branchRouteSettingConfigs = response;
					loadBranchRouteSettingsConfigsDeferred.resolve();
				});

				UtilsService.waitMultiplePromises(initPromises).then(function () {
					defineAPI();
				});
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					var branchRouteSettings;
					var loadBranchRouteSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
					promises.push(loadBranchRouteSettingsPromiseDeferred.promise);

					if (payload != undefined && payload.branchRouteSettings != undefined) {
						branchRouteSettings = payload.branchRouteSettings;
						$scope.scopeModel.selectedBranchRouteSettings = UtilsService.getItemByVal($scope.scopeModel.branchRouteSettingConfigs, branchRouteSettings.ConfigId, "ExtensionConfigurationId");
					}
					else
						$scope.scopeModel.selectedBranchRouteSettings = $scope.scopeModel.branchRouteSettingConfigs[0];

					branchRouteSettingsReadyDeferred.promise.then(function () {
						var payload = {
							branchRouteSettings: branchRouteSettings
						};
						VRUIUtilsService.callDirectiveLoad(branchRouteSettingsAPI, payload, loadBranchRouteSettingsPromiseDeferred);
					});

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					if (branchRouteSettingsAPI != undefined)
						return branchRouteSettingsAPI.getData();
					return null;
				};

				if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsRoutesyncEricssonbranchroutesettings', whsRoutesyncEricssonbranchroutesettings);

})(app);