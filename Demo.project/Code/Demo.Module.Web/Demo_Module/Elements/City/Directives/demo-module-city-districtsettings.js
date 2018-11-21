(function (app) {

	'use strict';

	districtSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_CityAPIService'];

	function districtSettingsDirective(UtilsService, VRUIUtilsService, Demo_Module_CityAPIService) {
		return {
			restrict: "E",
			scope: {
				onReady: "=",
				normalColNum: '@',
				label: '@',
				customvalidate: '=',
				value: '=',
				isrequired: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new DistrictSettings($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			template: function (element, attrs) {
				return getTamplate(attrs);
			}
		};

		function DistrictSettings($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var selectorAPI;

			var directiveAPI;
			var directiveReadyDeferred;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.templateConfigs = [];
				$scope.scopeModel.selectedTemplateConfig;

				$scope.scopeModel.onSelectorReady = function (api) {
					selectorAPI = api;
					defineAPI();
				};

				$scope.scopeModel.onDirectiveReady = function (api) {
					directiveAPI = api;
					var setLoader = function (value) {
						$scope.scopeModel.isLoadingDirective = value;
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					selectorAPI.clearDataSource();

					var promises = [];

					var districtSettings;

					if (payload != undefined) {
						districtSettings = payload.districtSettings;
					}

					if (districtSettings != undefined) {
						var loadDirectivePromise = loadDirective();
						promises.push(loadDirectivePromise);
					}

					var getDistrictSettingsConfigsPromise = getDistrictSettingsConfigs();
					promises.push(getDistrictSettingsConfigsPromise);

					function getDistrictSettingsConfigs() {
						return Demo_Module_CityAPIService.GetDistrictSettingsConfigs().then(function (response) {
							if (response != null) {
								for (var i = 0; i < response.length; i++) {
									$scope.scopeModel.templateConfigs.push(response[i]);
								}
								if (districtSettings != undefined) {
									$scope.scopeModel.selectedTemplateConfig =
										UtilsService.getItemByVal($scope.scopeModel.templateConfigs, districtSettings.ConfigId, 'ExtensionConfigurationId');
								}
							}
						});
					}

					function loadDirective() {
						directiveReadyDeferred = UtilsService.createPromiseDeferred();

						var directiveLoadDeferred = UtilsService.createPromiseDeferred();

						directiveReadyDeferred.promise.then(function () {
							directiveReadyDeferred = undefined;

							var directivePayload = {
								districtSettings: districtSettings
							};
							VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
						});

						return directiveLoadDeferred.promise;
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var data;
					if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
						data = directiveAPI.getData();
						if (data != undefined) {
							data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
						}
					}
					return data;
				};

				if (ctrl.onReady != null) {
					ctrl.onReady(api);
				}
			}
		}

		function getTamplate(attrs) {

			var hideremoveicon = '';
			if (attrs.hideremoveicon != undefined) {
				hideremoveicon = 'hideremoveicon';
			}

			var template =
				'<vr-columns width="1/2row">'
					+ ' <vr-select on-ready="scopeModel.onSelectorReady"'
						+ ' datasource="scopeModel.templateConfigs"'
						+ ' selectedvalues="scopeModel.selectedTemplateConfig"'
						+ ' datavaluefield="ExtensionConfigurationId"'
						+ ' datatextfield="Title"'
						+ ' ' + hideremoveicon + ' '
						+ '	label="District Type"'
						+ 'isrequired ="ctrl.isrequired">'
					+ '</vr-select>'
				+ '</vr-columns>'

				+ '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
				+ 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
				+ '</vr-directivewrapper>';

			return template;
		}
	}

	app.directive('demoModuleCityDistrictsettings', districtSettingsDirective);
})(app);