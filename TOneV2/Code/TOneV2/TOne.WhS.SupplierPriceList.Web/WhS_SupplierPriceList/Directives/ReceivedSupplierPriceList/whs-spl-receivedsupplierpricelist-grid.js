"use strict";

app.directive("whsSplReceivedsupplierpricelistGrid", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_SupPL_ReceivedSupplierPricelistAPIService", "FileAPIService", "WhS_BE_SupplierPriceListService", "BusinessProcess_BPInstanceService", "WhS_SupPL_ReceivedPricelistStatusEnum", "WhS_SupPL_SupplierPriceListTemplateService",
	function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_SupPL_ReceivedSupplierPricelistAPIService, FileAPIService, WhS_BE_SupplierPriceListService, BusinessProcess_BPInstanceService, WhS_SupPL_ReceivedPricelistStatusEnum, WhS_SupPL_SupplierPriceListTemplateService) {
		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var grid = new ReceivedSupplierPricelistGrid($scope, ctrl, $attrs);
				grid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/WhS_SupplierPricelist/Directives/ReceivedSupplierPricelist/Templates/ReceivedSupplierPricelistGridTemplate.html"

		};

		function ReceivedSupplierPricelistGrid($scope, ctrl, $attrs) {

			var gridAPI;
			var gridDrillDownTabsObj;
			this.initializeController = initializeController;

			function initializeController() {

				$scope.receivedPricelists = [];
				$scope.onGridReady = function (api) {
					gridAPI = api;
					gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs([], gridAPI, $scope.menuActions, true);

					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.loadGrid = function (query) {
							return gridAPI.retrieveData(query);
						};
						return directiveAPI;
					}
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return WhS_SupPL_ReceivedSupplierPricelistAPIService.GetFilteredReceivedSupplierPricelist(dataRetrievalInput)
						.then(function (response) {
							for (var i = 0; i < response.Data.length; i++) {
								var receivedPricelist = response.Data[i];
								if (receivedPricelist.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToBusinessRuleError.value
									|| receivedPricelist.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToProcessingError.value
									|| receivedPricelist.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToConfigurationError.value
									|| receivedPricelist.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToReceivedMailError.value) {
									receivedPricelist.ErrorCol = "View Details";
								}
							}
							onResponseReady(response);
						})
						.catch(function (error) {
							VRNotificationService.notifyException(error, $scope);
						});
				};
				$scope.viewDetails = function (pricelistObj) {
					if (pricelistObj.ReceivedPricelist.ErrorDetails != null && pricelistObj.ReceivedPricelist.ErrorDetails.length > 0)
						WhS_SupPL_SupplierPriceListTemplateService.showReceivedPricelistErrorDetails(pricelistObj.ReceivedPricelist.ErrorDetails);
					else viewTracker(pricelistObj);
				};

				defineMenuActions();
			}

			function defineMenuActions() {
				$scope.gridMenuActions = function (dataItem) {
					var menuActions = [];
					var processInstanceId = dataItem.ReceivedPricelist.ProcessInstanceId;
					if (dataItem.ReceivedPricelist.FileId !== 0 && dataItem.ReceivedPricelist.FileId != null) {
						var downloadPricelistAction = {
							name: "Download",
							clicked: downloadPricelist
						};
						menuActions.push(downloadPricelistAction);
					}

					if (processInstanceId != null && dataItem.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.Succeeded.value) {
						var additionalMenuActions = WhS_BE_SupplierPriceListService.getAdditionalActionOfSupplierPricelistGrid();
						for (var i = 0, length = additionalMenuActions.length; i < length; i++) {
							var additionalMenuAction = additionalMenuActions[i];
							var menuAction = {
								name: additionalMenuAction.name,
								clicked: function (dataItem) {
									var payload = {
										processInstanceId: dataItem.ReceivedPricelist.ProcessInstanceId
									};
									additionalMenuAction.clicked(payload);
								}
							};
							menuActions.push(menuAction);
						}
					}

					if (processInstanceId != null) {
						var viewTrackerMenuAction = {
							name: "View Tracker",
							clicked: viewTracker,
						};
						menuActions.push(viewTrackerMenuAction);
					}

					if (dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.Succeeded.value
						&& dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.CompletedWithNoChanges.value
						&& dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToReceivedMailError.value) {
						var setAsCompletedMenuAction = {
							name: "Set As Completed",
							clicked: setReceivedPricelistAsCompleted,
						};
						menuActions.push(setAsCompletedMenuAction);
					}

					return menuActions;
				};
			}

			function downloadPricelist(pricelistObj) {
				FileAPIService.DownloadFile(pricelistObj.ReceivedPricelist.FileId)
					.then(function (response) {
						UtilsService.downloadFile(response.data, response.headers);
					});
			}

			function viewTracker(pricelistObj) {
				return BusinessProcess_BPInstanceService.openProcessTracking(pricelistObj.ReceivedPricelist.ProcessInstanceId);
			}

			function setReceivedPricelistAsCompleted(pricelistObj) {
				$scope.isLoading = true;
				WhS_SupPL_ReceivedSupplierPricelistAPIService.SetReceivedPricelistAsCompleted(pricelistObj)
					.then(function (response) {
						gridDrillDownTabsObj.setDrillDownExtensionObject(response);
						gridAPI.itemUpdated(response);
						$scope.isLoading = false;
					});
			}
		}

		return directiveDefinitionObject;

	}]);
