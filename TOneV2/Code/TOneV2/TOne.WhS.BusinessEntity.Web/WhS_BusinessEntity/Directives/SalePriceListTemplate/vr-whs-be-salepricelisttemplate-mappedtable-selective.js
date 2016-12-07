'use strict';

app.directive('vrWhsBeSalepricelisttemplateMappedtableSelective', ['WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var mappedTableSelective = new MappedTableSelective($scope, ctrl, $attrs);
			mappedTableSelective.initializeController();
		},
		controllerAs: "mappedTableCtrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/Templates/SalePriceListMappedTableSelectiveTemplate.html'
	};

	function MappedTableSelective($scope, ctrl, $attrs) {

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
				var mappedTable;

				if (payload != undefined) {
					mappedTable = payload.mappedTable;
				}

				if (mappedTable != undefined) {
				    configId = mappedTable.ConfigId;

				   
				}

				var getMappedTablesExtensionConfigsPromise = getMappedTablesExtensionConfigs(configId);
				promises.push(getMappedTablesExtensionConfigsPromise);

				return UtilsService.waitMultiplePromises(promises);
				
			};

			api.getData = function getData() {

			    return $scope.selectedExtensionConfig;
				
			};

			api.clearSelectedValue = function () {
			    $scope.selectedExtensionConfig = undefined;
			}

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function getMappedTablesExtensionConfigs(selectedId) {
			return WhS_BE_SalePriceListTemplateAPIService.GetMappedTablesExtensionConfigs().then(function (response) {
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
	}
}]);