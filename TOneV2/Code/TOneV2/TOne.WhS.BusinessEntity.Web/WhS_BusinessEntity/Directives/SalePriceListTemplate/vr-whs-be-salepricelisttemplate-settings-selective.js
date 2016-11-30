'use strict';

app.directive('vrWhsBeSalepricelisttemplateSettingsSelective', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var salePriceListTemplateSettingsSelective = new SalePriceListTemplateSettingsSelective($scope, ctrl, $attrs);
			salePriceListTemplateSettingsSelective.initializeController();
		},
		controllerAs: "selectiveCtrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/Templates/SalePriceListTemplateSettingsSelectiveTemplate.html'
	};

	function SalePriceListTemplateSettingsSelective($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var selectorAPI;

		var directiveAPI;
		var directiveReadyDeferred = UtilsService.createPromiseDeferred();

 

		function initializeController() {

			$scope.extensionConfigs = [];

			$scope.onSelectorReady = function (api) {
				selectorAPI = api;
				defineAPI();
			};

			$scope.onDirectiveReady = function (api) {
				directiveAPI = api;
				var setLoader = function (value) { $scope.isLoadingDirective = value; };
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
			};
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var promises = [];

				var configId;
				var settings;

				if (payload != undefined) {
					settings = payload.settings;
				}

				if (settings != undefined) {
					configId = settings.ConfigId;
				}

				var getExtensionConfigsPromise = getSalePriceListTemplateSettingsExtensionConfigs(configId);
				promises.push(getExtensionConfigsPromise);

				var loadDirectivePromise = loadDirective(settings);
				promises.push(loadDirectivePromise);

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function getData() {

				if (directiveAPI == undefined)
					return undefined;

				return directiveAPI.getData();
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function getSalePriceListTemplateSettingsExtensionConfigs(selectedId) {
			return WhS_BE_SalePriceListTemplateAPIService.GetSalePriceListTemplateSettingsExtensionConfigs().then(function (response) {
				if (response != null) {
					for (var i = 0; i < response.length; i++)
						$scope.extensionConfigs.push(response[i]);
				}
				if (selectedId != undefined) {
					$scope.selectedExtensionConfig = UtilsService.getItemByVal($scope.extensionConfigs, selectedId, 'ExtensionConfigurationId');
				}
				else if ($scope.extensionConfigs.length > 0) {
					$scope.selectedExtensionConfig = $scope.extensionConfigs[0];
				}
			});
		}
		function loadDirective(settings) {

			var directiveLoadDeferred = UtilsService.createPromiseDeferred();

			directiveReadyDeferred.promise.then(function () {
				directiveReadyDeferred = undefined;
				var directivePayload = { settings: settings };
				VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
			});

			return directiveLoadDeferred.promise;
		}
	}
}]);