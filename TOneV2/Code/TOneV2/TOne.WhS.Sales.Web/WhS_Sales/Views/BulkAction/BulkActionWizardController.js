(function (appControllers) {

	"use strict";

	BulkActionWizardController.$inject = ["$scope", 'WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

	function BulkActionWizardController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

		var ownerType;
		var ownerId;
		var ownerSellingNumberPlanId;

		var bulkActionContext;

		var actionStepAPI;
		var actionStepReadyDeferred = UtilsService.createPromiseDeferred();

		var filterStepAPI;
		var filterStepReadyDeferred = UtilsService.createPromiseDeferred();

		var gridAPI;
		var gridReadyDeferred = UtilsService.createPromiseDeferred();
		var gridQuery;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined) {
				ownerType = parameters.ownerType;
				ownerId = parameters.ownerId;
				ownerSellingNumberPlanId = parameters.ownerSellingNumberPlanId;
				gridQuery = parameters.gridQuery;
			}
		}
		function defineScope() {

			$scope.title = "Bulk Actions";

			$scope.scopeModel = {};

			$scope.scopeModel.onActionStepReady = function (api) {
				actionStepAPI = api;
				actionStepReadyDeferred.resolve();
			};

			$scope.scopeModel.onFilterStepReady = function (api) {
				filterStepAPI = api;
				filterStepReadyDeferred.resolve();
			};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridReadyDeferred.resolve();
			};

			$scope.scopeModel.evaluate = function () {
				$scope.scopeModel.showPreview = true;
				$scope.scopeModel.isApplyButtonDisabled = false;

				gridReadyDeferred.promise.then(function () {

					var actionStepData = actionStepAPI.getData();
					if (actionStepData != undefined)
						gridQuery.BulkAction = actionStepData.bulkAction;

					if (gridQuery.Filter == null)
						gridQuery.Filter = {};

					var filterStepData = filterStepAPI.getData();
					if (filterStepData != undefined)
						gridQuery.Filter.BulkActionFilter = filterStepData.zoneFilter;

					return gridAPI.load(gridQuery);
				});
			};

			$scope.scopeModel.apply = function () {

				$scope.scopeModel.isLoading = true;
				var applyInput = getApplyInput();

				return WhS_Sales_RatePlanAPIService.ApplyBulkActionToDraft(applyInput).then(function () {
					if ($scope.onBulkActionAppliedToDraft != undefined)
						$scope.onBulkActionAppliedToDraft();
					$scope.modalContext.closeModal();
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});

				function getApplyInput()
				{
					var applyInput =
					{
						OwnerType: gridQuery.OwnerType,
						OwnerId: gridQuery.OwnerId,
						CurrencyId: gridQuery.CurrencyId,
						RoutingDatabaseId: gridQuery.RoutingDatabaseId,
						PolicyConfigId: gridQuery.PolicyConfigId,
						NumberOfOptions: gridQuery.NumberOfOptions,
						CostCalculationMethods: gridQuery.CostCalculationMethods,
						BulkAction: gridQuery.BulkAction,
						EffectiveOn: gridQuery.EffectiveOn,
					};
					if (gridQuery.Filter != null) {
						applyInput.BulkActionFilter = gridQuery.Filter.BulkActionFilter;
					}
					return applyInput;
				}
			};

			$scope.scopeModel.cancel = function () {
				$scope.modalContext.closeModal();
			};

			setContext();
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}
		function loadAllControls() {
			UtilsService.waitMultipleAsyncOperations([loadActionStep, loadFilterStep]).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function loadActionStep() {
			var actionStepLoadDeferred = UtilsService.createPromiseDeferred();
			actionStepReadyDeferred.promise.then(function () {
				var actionStepPayload = {
					bulkActionContext: bulkActionContext
				};
				VRUIUtilsService.callDirectiveLoad(actionStepAPI, actionStepPayload, actionStepLoadDeferred);
			});
			return actionStepLoadDeferred.promise;
		}
		function loadFilterStep() {
			var filterStepLoadDeferred = UtilsService.createPromiseDeferred();
			filterStepReadyDeferred.promise.then(function () {
				var filterStepPayload = {
					bulkActionContext: bulkActionContext
				};
				VRUIUtilsService.callDirectiveLoad(filterStepAPI, filterStepPayload, filterStepLoadDeferred);
			});
			return filterStepLoadDeferred.promise;
		}

		function setContext() {
			bulkActionContext = {};
			bulkActionContext.ownerType = ownerType;
			bulkActionContext.ownerId = ownerId;
			bulkActionContext.ownerSellingNumberPlanId = ownerSellingNumberPlanId;
			bulkActionContext.getSelectedBulkAction = function () {
				var actionStepData = actionStepAPI.getData();
				return (actionStepData != undefined) ? actionStepData.bulkAction : undefined;
			};
			bulkActionContext.requireEvaluation = function () {
				$scope.scopeModel.isApplyButtonDisabled = true;
			};
		}
	}

	appControllers.controller("WhS_Sales_BulkActionWizardController", BulkActionWizardController);

})(appControllers);