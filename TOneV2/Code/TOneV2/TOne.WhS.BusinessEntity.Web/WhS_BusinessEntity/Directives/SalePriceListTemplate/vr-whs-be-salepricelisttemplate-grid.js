'use strict';

app.directive('vrWhsBeSalepricelisttemplateGrid', ['WhS_BE_SalePriceListTemplateAPIService', 'WhS_BE_SalePriceListTemplateService', 'VRNotificationService', function (WhS_BE_SalePriceListTemplateAPIService, WhS_BE_SalePriceListTemplateService, VRNotificationService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var salePriceListTemplateGrid = new SalePriceListTemplateGrid($scope, ctrl, $attrs);
			salePriceListTemplateGrid.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListTemplate/Templates/SalePriceListTemplateGridTemplate.html'
	};

	function SalePriceListTemplateGrid($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;

		function initializeController() {

			$scope.scopeModel = {};
			$scope.scopeModel.salePriceListTemplates = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
				return WhS_BE_SalePriceListTemplateAPIService.GetFilteredSalePriceListTemplates(dataRetrievalInput).then(function (response) {
					onResponseReady(response);
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			};

			defineMenuActions();
		}
		function defineAPI() {

			var api = {};

			api.load = function (query) {
				return gridAPI.retrieveData(query);
			};

			api.onSalePriceListTemplateAdded = function (addedSalePriceListTempate) {
				gridAPI.itemAdded(addedSalePriceListTempate);
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function defineMenuActions() {
			$scope.scopeModel.menuActions = [{
				name: 'Edit',
				clicked: editSalePriceListTemplate,
				haspermission: hasEditPermission
			}];
		}
		function editSalePriceListTemplate(dataItem) {
			var onSalePriceListTemplateUpdated = function (updatedSalePriceListTemplate) {
				gridAPI.itemUpdated(updatedSalePriceListTemplate);
			};
			WhS_BE_SalePriceListTemplateService.editSalePriceListTemplate(dataItem.Entity.SalePriceListTemplateId, onSalePriceListTemplateUpdated);
		}
		function hasEditPermission() {
			return WhS_BE_SalePriceListTemplateAPIService.HasEditSalePriceListTemplatePermission();
		}
	}
}]);