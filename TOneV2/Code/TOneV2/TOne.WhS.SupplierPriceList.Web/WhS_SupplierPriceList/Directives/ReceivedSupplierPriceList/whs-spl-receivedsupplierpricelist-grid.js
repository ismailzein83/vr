"use strict";

app.directive("whsSplReceivedsupplierpricelistGrid", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_SupPL_ReceivedSupplierPricelistAPIService", "FileAPIService", "WhS_BE_SupplierPriceListService", "BusinessProcess_BPInstanceService", "WhS_SupPL_ReceivedPricelistStatusEnum", "WhS_SupPL_SupplierPriceListTemplateService", "WhS_SupPL_ReceivedSupplierPricelistService", "WhS_SupPL_SupplierPriceListService",
	function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_SupPL_ReceivedSupplierPricelistAPIService, FileAPIService, WhS_BE_SupplierPriceListService, BusinessProcess_BPInstanceService, WhS_SupPL_ReceivedPricelistStatusEnum, WhS_SupPL_SupplierPriceListTemplateService, WhS_SupPL_ReceivedSupplierPricelistService, WhS_SupPL_SupplierPriceListService) {
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

	            $scope.getStatusColor = function (dataItem) {
	                return WhS_SupPL_ReceivedSupplierPricelistService.getStatusColor(dataItem.ReceivedPricelist.Status);
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
	                if (dataItem.ReceivedPricelist.FileId != 0 && dataItem.ReceivedPricelist.FileId != null) {
	                    var downloadPricelistAction = {
	                        name: "Download",
	                        clicked: downloadPricelist
	                    };
	                    menuActions.push(downloadPricelistAction);
	                }

	                if (processInstanceId != null && dataItem.ReceivedPricelist.Status == WhS_SupPL_ReceivedPricelistStatusEnum.Succeeded.value) {

	                    var previewPricelistAction = {
	                        name: "Preview",
	                        clicked: previewPricelist
	                    };
	                    menuActions.push(previewPricelistAction);
	                }

	                if (processInstanceId != null && dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.ImportedManually.value) {
	                    var viewTrackerMenuAction = {
	                        name: "Process Log",
	                        clicked: viewTracker,
	                    };
	                    menuActions.push(viewTrackerMenuAction);
	                }

	                if (dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.Succeeded.value
						&& dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.CompletedWithNoChanges.value
						&& dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToReceivedMailError.value
                        && dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.ImportedManually.value
                        && dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.Rejected.value
                        && dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.WaitingConfirmation.value
						&& dataItem.ReceivedPricelist.Status != WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToBusinessRuleError.value) {
	                    var setAsCompletedMenuAction = {
	                        name: "Set As Completed",
	                        clicked: setReceivedPricelistAsCompleted,
	                    };
	                    menuActions.push(setAsCompletedMenuAction);
	                }

	                return menuActions;
	            };
	        }
            function previewPricelist(dataItem) {
	            var obj = {
	                processInstanceId: dataItem.ReceivedPricelist.ProcessInstanceId,
	                fileId: dataItem.ReceivedPricelist.FileId,
	                supplierPricelistType: dataItem.ReceivedPricelist.PricelistType,
	                pricelistDate: dataItem.ReceivedPricelist.ReceivedDateTime,
                    currencyId: dataItem.CurrencyId,
                    supplierId: dataItem.ReceivedPricelist.SupplierId
	            };
	            WhS_SupPL_SupplierPriceListService.previewSupplierPriceList(obj);
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
	           return WhS_SupPL_ReceivedSupplierPricelistAPIService.SetReceivedPricelistAsCompleted(pricelistObj)
					.then(function (response) {
					    gridDrillDownTabsObj.setDrillDownExtensionObject(response.UpdatedObject);
					    gridAPI.itemUpdated(response.UpdatedObject);
					    $scope.isLoading = false;
					})
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
	        }
	    }

	    return directiveDefinitionObject;

	}]);
