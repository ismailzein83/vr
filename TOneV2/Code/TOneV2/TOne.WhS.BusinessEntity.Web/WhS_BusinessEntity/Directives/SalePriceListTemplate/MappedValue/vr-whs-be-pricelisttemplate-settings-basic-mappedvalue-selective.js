'use strict';

app.directive('vrWhsBeSalepricelisttemplateSettingsBasicMappedvalueSelective', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var basicSettingsMappedValueSelective = new BasicSettingsMappedValueSelective($scope, ctrl, $attrs);
			basicSettingsMappedValueSelective.initializeController();
		},
		controllerAs: "mappedValueCtrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/MappedValue/Templates/BasicSalePriceListTemplateSettingsMappedValueSelectiveTemplate.html'
	};

	function BasicSettingsMappedValueSelective($scope, ctrl, $attrs) {

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
				var mappedValue;

				if (payload != undefined) {
					mappedValue = payload.mappedValue;
				}

				if (mappedValue != undefined) {
					configId = mappedValue.ConfigId;
				}

				var getExtensionConfigsPromise = getBasicSettingsMappedValueExtensionConfigs(configId);
				promises.push(getExtensionConfigsPromise);

				var loadDirectivePromise = loadDirective(mappedValue);
				promises.push(loadDirectivePromise);

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function getData() {

				// Require both directiveData and selectedExtensionConfigId for valid output

				if (directiveAPI == undefined)
					return undefined;

				var directiveData = directiveAPI.getData();
				if (directiveData == undefined || $scope.selectedExtensionConfig == undefined)
					return undefined;

				directiveData.ConfigId = $scope.selectedExtensionConfig.ExtensionConfigurationId;
				return directiveData;
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function getBasicSettingsMappedValueExtensionConfigs(selectedId) {
			return WhS_BE_SalePriceListTemplateAPIService.GetBasicSettingsMappedValueExtensionConfigs().then(function (response) {
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
		function loadDirective(mappedValue) {

			var directiveLoadDeferred = UtilsService.createPromiseDeferred();

			directiveReadyDeferred.promise.then(function () {
				directiveReadyDeferred = undefined;
				var directivePayload = { mappedValue: mappedValue };
				VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
			});

			return directiveLoadDeferred.promise;
		}
	}
}]);