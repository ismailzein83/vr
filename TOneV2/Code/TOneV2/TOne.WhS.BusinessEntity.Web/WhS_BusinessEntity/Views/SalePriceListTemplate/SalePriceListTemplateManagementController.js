(function (appControllers) {

	'use strict';

	SalePriceListTemplateManagementController.$inject = ['$scope', 'WhS_BE_SalePriceListTemplateService', 'WhS_BE_SalePriceListTemplateAPIService', 'UtilsService', 'VRNotificationService'];

	function SalePriceListTemplateManagementController($scope, WhS_BE_SalePriceListTemplateService, WhS_BE_SalePriceListTemplateAPIService, UtilsService, VRNotificationService) {

		var gridAPI;

		defineScope();
		load();

		function defineScope() {

			$scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridAPI.load(getGridQuery());
			};

			$scope.scopeModel.search = function () {
				return gridAPI.load(getGridQuery());
			};

			$scope.scopeModel.add = function () {
				var onSalePriceListTemplateAdded = function (addedSalePriceListTemplate) {
					gridAPI.onSalePriceListTemplateAdded(addedSalePriceListTemplate);
				};
				WhS_BE_SalePriceListTemplateService.addSalePriceListTemplate(onSalePriceListTemplateAdded);
			};

			$scope.scopeModel.hasAddPermission = function () {
				return WhS_BE_SalePriceListTemplateAPIService.HasAddSalePriceListTemplatePermission();
			};
		}

		function load() {

		}

		function getGridQuery() {
			return {
				Name: $scope.scopeModel.name
			};
		}
	}

	appControllers.controller('WhS_BE_SalePriceListTemplateManagementController', SalePriceListTemplateManagementController);

})(appControllers);