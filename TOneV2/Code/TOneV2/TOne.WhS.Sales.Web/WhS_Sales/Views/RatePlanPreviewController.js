(function (appControllers) {

    'use strict';

    RatePlanPreviewController.$inject = ['$scope', 'WhS_Sales_RatePlanPreviewAPIService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_SellingProductAPIService', 'WhS_BE_CarrierAccountAPIService'];

    function RatePlanPreviewController($scope, WhS_Sales_RatePlanPreviewAPIService, WhS_BE_SalePriceListOwnerTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SellingProductAPIService, WhS_BE_CarrierAccountAPIService) {

        var taskId;
        var processInstanceId;
        var ownerType;
        var ownerId;
        var ratePreviewGridAPI;
        var ratePreviewGridReadyDeferred = UtilsService.createPromiseDeferred();
        var customeRatePreviewGridReadyDeferred = UtilsService.createPromiseDeferred();
        var productRatePreviewGridAPI;
        var productRatePreviewGridReadyDeferred = UtilsService.createPromiseDeferred();

        var saleZoneRoutingProductPreviewGridAPI;
        var saleZoneRoutingProductGridReadyDeferred = UtilsService.createPromiseDeferred();

        var summaryServiceViewerAPI;
        var summaryServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

        var newCountryGridAPI;
        var newCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

        var changedCountryGridAPI;
        var changedCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

        var rateCarrierAccountSelectorAPI;
        var rateCarrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var newCountryCarrierAccountSelectorAPI;
        var newCountryCarrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var changedCountryCarrierAccountSelectorAPI;
        var changedCountryCarrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var routingProductCarrierAccountSelectorAPI;
        var RPCarrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var customerSaleZoneRoutingProductPreviewGridAPI;
        var customerSaleZoneRoutingProductGridReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                taskId = parameters.TaskId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.tabProductRPObject = { showTab: false };
            $scope.scopeModel.tabProductRateObject = { showTab: false };
            $scope.scopeModel.showCarrierAccountSelector = true;
            $scope.scopeModel.countryTab = { showTab: false };
            $scope.scopeModel.tabCustomerRPObject = { showTab: false };
            $scope.scopeModel.tabCustomerRateObject = { showTab: false };
            $scope.scopeModel.showCustomerRateMsg = false;
            $scope.scopeModel.showCustomerRPMsg = false;
            $scope.scopeModel.showProductRateMsg = false;
            $scope.scopeModel.showProductRPMsg = false;
            $scope.scopeModel.showProductRPData = false;
            $scope.scopeModel.showProductRateData = false;
            //$scope.scopeModel.defaultServicePreview = {};
            $scope.scopeModel.onRPSelectorBlurred = function () {
                customerSaleZoneRoutingProductPreviewGridAPI.cleanGrid();
                $scope.scopeModel.numberOfNewSaleZoneRoutingProducts = undefined;
                $scope.scopeModel.numberOfClosedSaleZoneRoutingProducts = undefined;
            };
            $scope.scopeModel.onRateSelectorBlurred = function () {
                ratePreviewGridAPI.cleanGrid();
                $scope.scopeModel.numberOfNewRates = undefined;
                $scope.scopeModel.numberOfIncreasedRates = undefined;
                $scope.scopeModel.numberOfDecreasedRates = undefined;
            };
            $scope.scopeModel.onCustomerRatePreviewGridReady = function (api) {
                ratePreviewGridAPI = api;
                customeRatePreviewGridReadyDeferred.resolve();
            };
            $scope.scopeModel.onRatePreviewGridReady = function (api) {
                productRatePreviewGridAPI = api;
                productRatePreviewGridReadyDeferred.resolve();
            };
            $scope.scopeModel.loadRatePrview = function () {

                var selectedCustomerIds = (rateCarrierAccountSelectorAPI.getSelectedIds() != null) ? rateCarrierAccountSelectorAPI.getSelectedIds() : null;
                var ratePreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    ZoneName: null,
                    CustomerIds: selectedCustomerIds,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showRateCarrierAccountSelector : true,
                };
                var query = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: selectedCustomerIds
                };
                WhS_Sales_RatePlanPreviewAPIService.GetCustomerRatePlanPreviewSummary(query).then(function (response) {
                    if (response == undefined || response == null)
                        return undefined;
                    else {
                        $scope.scopeModel.numberOfNewRates = response.NumberOfNewRates;
                        $scope.scopeModel.numberOfIncreasedRates = response.NumberOfIncreasedRates;
                        $scope.scopeModel.numberOfDecreasedRates = response.NumberOfDecreasedRates;
                    }
                });
                VRUIUtilsService.callDirectiveLoad(ratePreviewGridAPI, ratePreviewGridPayload, undefined);
            };

            $scope.scopeModel.loadNewCountryPreview = function () {

                var selectedCustomerIds = (newCountryCarrierAccountSelectorAPI.getSelectedIds() != null) ? newCountryCarrierAccountSelectorAPI.getSelectedIds() : null;
                var newCountryPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: selectedCustomerIds,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showNewCountriesCarrierAccountSelector : true,
                };

                VRUIUtilsService.callDirectiveLoad(newCountryGridAPI, newCountryPreviewGridPayload, undefined);
            };

            $scope.scopeModel.loadChangedCountryPreview = function () {

                var selectedCustomerIds = (changedCountryCarrierAccountSelectorAPI.getSelectedIds() != null) ? changedCountryCarrierAccountSelectorAPI.getSelectedIds() : null;
                var changedCountryPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: selectedCustomerIds,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showChangedCountriesCarrierAccountSelector : true,
                };

                VRUIUtilsService.callDirectiveLoad(changedCountryGridAPI, changedCountryPreviewGridPayload, undefined);
            };

            $scope.scopeModel.loadCustomerRoutingProductPrview = function () {

                var selectedCustomerIds = (routingProductCarrierAccountSelectorAPI.getSelectedIds() != null) ? routingProductCarrierAccountSelectorAPI.getSelectedIds() : null;
                var routingProductPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: selectedCustomerIds,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showCustomerRoutingProductCarrierAccountSelector : true,
                };
                var query = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: selectedCustomerIds
                };
                WhS_Sales_RatePlanPreviewAPIService.GetCustomerRatePlanPreviewSummary(query).then(function (response) {
                    if (response == undefined || response == null)
                        return undefined;
                    else {
                        $scope.scopeModel.numberOfNewSaleZoneRoutingProducts = response.NumberOfNewSaleZoneRoutingProducts;
                        $scope.scopeModel.numberOfClosedSaleZoneRoutingProducts = response.NumberOfClosedSaleZoneRoutingProducts;
                    }
                });

                VRUIUtilsService.callDirectiveLoad(customerSaleZoneRoutingProductPreviewGridAPI, routingProductPreviewGridPayload, undefined);
            };
            $scope.scopeModel.onSaleZoneRoutingProductPreviewGridReady = function (api) {
                saleZoneRoutingProductPreviewGridAPI = api;
                saleZoneRoutingProductGridReadyDeferred.resolve();
            };
            $scope.scopeModel.onCustomerSaleZoneRoutingProductPreviewGridReady = function (api) {
                customerSaleZoneRoutingProductPreviewGridAPI = api;
                customerSaleZoneRoutingProductGridReadyDeferred.resolve();
            };
            $scope.scopeModel.onRateCarrierAccountSelectorReady = function (api) {
                rateCarrierAccountSelectorAPI = api;
                rateCarrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onNewCountryCarrierAccountSelectorReady = function (api) {
                newCountryCarrierAccountSelectorAPI = api;
                newCountryCarrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onChangedCountryCarrierAccountSelectorReady = function (api) {
                changedCountryCarrierAccountSelectorAPI = api;
                changedCountryCarrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRoutingProductCarrierAccountSelectorReady = function (api) {
                routingProductCarrierAccountSelectorAPI = api;
                RPCarrierAccountSelectorReadyDeferred.resolve();

            };
            $scope.scopeModel.onRPCarrierAccountChanged = function () {

            };
            $scope.scopeModel.onRateCarrierAccountChanged = function () {

            };

            $scope.scopeModel.onSummaryServiceViewerReady = function (api) {
                summaryServiceViewerAPI = api;
                summaryServiceViewerReadyDeferred.resolve();
            };

            $scope.scopeModel.onNewCountryPreviewGridReady = function (api) {
                newCountryGridAPI = api;
                newCountryGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onChangedCountryPreviewGridReady = function (api) {
                changedCountryGridAPI = api;
                changedCountryGridReadyDeferred.resolve();
            };

            $scope.scopeModel.continueTask = function () {
                return executeTask(true);
            };
            $scope.scopeModel.stopTask = function () {
                return executeTask(false);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getTaskData().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getTaskData() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response == null)
                    return;
                processInstanceId = response.ProcessInstanceId;
                if (response.TaskData == null)
                    return;
                ownerType = response.TaskData.OwnerType;
                ownerId = response.TaskData.OwnerId;
                if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                    $scope.scopeModel.showCountrySummary = true;
                    $scope.scopeModel.countryTab.showTab = true;
                    $scope.scopeModel.showCarrierAccountSelector = false;
                }
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadDefaultRoutingProductPreview,
                loadCustomerRatePreview,
                loadCustomerRoutingProductPreview,
                loadNewCountryPreview,
                loadChangedCountryPreviewGrid,

                loadSaleZoneRoutingProductPreviewGrid,
                loadProductRatePreviewGrid,
            ])
                .then(function () {
                    if (customerSaleZoneRoutingProductPreviewGridAPI.gridHasData())
                        $scope.scopeModel.tabCustomerRPObject.showTab = true;
                    else {
                        $scope.scopeModel.showCustomerRPMsg = true;

                    }
                    if (ratePreviewGridAPI.gridHasData())
                        $scope.scopeModel.tabCustomerRateObject.showTab = true;
                    else {
                        $scope.scopeModel.showCustomerRateMsg = true;

                    }
                    if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.scopeModel.tabProductRPObject.showTab = true;
                        $scope.scopeModel.tabProductRateObject.showTab = true;
                        if (saleZoneRoutingProductPreviewGridAPI.gridHasData())
                            $scope.scopeModel.showProductRPData = true;
                        else {
                            $scope.scopeModel.showProductRPMsg = true;
                        }
                        if (productRatePreviewGridAPI.gridHasData())
                            $scope.scopeModel.showProductRateData = true;
                        else {
                            $scope.scopeModel.showProductRateMsg = true;
                        }
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        // #region RateTab

        function rateLoadCarrierAccountSelector() {
            var loadCarrierAccountSelectorDeferred = UtilsService.createPromiseDeferred();
            rateCarrierAccountSelectorReadyDeferred.promise.then(function () {
                var carrierAccountselectorPayload = {
                    filter: {
                        Filters: getCarrierAccountSelectorRateFilter()
                    }
                };
                VRUIUtilsService.callDirectiveLoad(rateCarrierAccountSelectorAPI, carrierAccountselectorPayload, loadCarrierAccountSelectorDeferred);
            });
            return loadCarrierAccountSelectorDeferred.promise;
        }
        function getCarrierAccountSelectorRateFilter() {
            var carrierAccountRateFilters = [];

            var carrierAccountRateFilterAffectedCustomer = {
                $type: 'TOne.WhS.Sales.Business.AffectedCarrierAccountFilterInRateChanges, TOne.WhS.Sales.Business',
                ProcessInstanceId: processInstanceId
            };
            carrierAccountRateFilters.push(carrierAccountRateFilterAffectedCustomer);
            return carrierAccountRateFilters;
        }
        function loadRatePreviewGrid() {
            var ratePreviewGridLoadDeferred = UtilsService.createPromiseDeferred();
            var ownerIds = [ownerId];
            customeRatePreviewGridReadyDeferred.promise.then(function () {
                var ratePreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: null,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showRateCarrierAccountSelector : true,
                };
                var query = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: null
                };
                WhS_Sales_RatePlanPreviewAPIService.GetCustomerRatePlanPreviewSummary(query).then(function (response) {
                    if (response == undefined || response == null)
                        return undefined;
                    else {
                        $scope.scopeModel.numberOfNewRates = response.NumberOfNewRates;
                        $scope.scopeModel.numberOfIncreasedRates = response.NumberOfIncreasedRates;
                        $scope.scopeModel.numberOfDecreasedRates = response.NumberOfDecreasedRates;
                    }
                });
                VRUIUtilsService.callDirectiveLoad(ratePreviewGridAPI, ratePreviewGridPayload, ratePreviewGridLoadDeferred);
            });

            return ratePreviewGridLoadDeferred.promise;
        }

        function loadCustomerRatePreview() {
            var loadRatePreviewPromiseDeffered = UtilsService.createPromiseDeferred();
            rateLoadCarrierAccountSelector().then(function (response) {
                $scope.scopeModel.showRateCarrierAccountSelector = !rateCarrierAccountSelectorAPI.hasSingleItem();
                loadRatePreviewGrid().then(function (response) {
                    loadRatePreviewPromiseDeffered.resolve();
                });
            })
            return loadRatePreviewPromiseDeffered.promise;
        }
        // #endregion

        // #region RoutingProductTab
        function routingProductLoadCarrierAccountSelector() {
            var loadCarrierAccountSelectorDeferred = UtilsService.createPromiseDeferred();
            RPCarrierAccountSelectorReadyDeferred.promise.then(function () {
                var carrierAccountselectorPayload = {
                    filter: {
                        Filters: getCarrierAccountSelectorRPFilter()
                    }

                };
                VRUIUtilsService.callDirectiveLoad(routingProductCarrierAccountSelectorAPI, carrierAccountselectorPayload, loadCarrierAccountSelectorDeferred);
            });
            return loadCarrierAccountSelectorDeferred.promise;
        }
        function getCarrierAccountSelectorRPFilter() {
            var carrierAccountRPFilters = [];

            var carrierAccountRPFilterAffectedCustomer = {
                $type: 'TOne.WhS.Sales.Business.AffectedCarrierAccountFilterInRPChanges, TOne.WhS.Sales.Business',
                ProcessInstanceId: processInstanceId
            };
            carrierAccountRPFilters.push(carrierAccountRPFilterAffectedCustomer);
            return carrierAccountRPFilters;
        }
        function loadCustomerSaleZoneRoutingProductPreviewGrid() {
            var saleZoneRoutingProductGridLoadDeferred = UtilsService.createPromiseDeferred();
            var ownerIds = [ownerId];
            customerSaleZoneRoutingProductGridReadyDeferred.promise.then(function () {
                var saleZoneRoutingProductPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: null,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showCustomerRoutingProductCarrierAccountSelector : true,
                };
                var query = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: null
                };
                WhS_Sales_RatePlanPreviewAPIService.GetCustomerRatePlanPreviewSummary(query).then(function (response) {
                    if (response == undefined || response == null)
                        return undefined;
                    else {
                        $scope.scopeModel.numberOfNewSaleZoneRoutingProducts = response.NumberOfNewSaleZoneRoutingProducts;
                        $scope.scopeModel.numberOfClosedSaleZoneRoutingProducts = response.NumberOfClosedSaleZoneRoutingProducts;
                    }
                });
                VRUIUtilsService.callDirectiveLoad(customerSaleZoneRoutingProductPreviewGridAPI, saleZoneRoutingProductPreviewGridPayload, saleZoneRoutingProductGridLoadDeferred);
            });

            return saleZoneRoutingProductGridLoadDeferred.promise;
        }

        function loadCustomerRoutingProductPreview() {
            var loadCustomerRoutingProductPreviewPromiseDeffered = UtilsService.createPromiseDeferred();
            routingProductLoadCarrierAccountSelector().then(function (response) {
                $scope.scopeModel.showCustomerRoutingProductCarrierAccountSelector = !routingProductCarrierAccountSelectorAPI.hasSingleItem();
                loadCustomerSaleZoneRoutingProductPreviewGrid().then(function (response) {
                    loadCustomerRoutingProductPreviewPromiseDeffered.resolve();
                });
            })
            return loadCustomerRoutingProductPreviewPromiseDeffered.promise;
        }
        // #endregion

        // #region NewCountryTab
        function newCountryLoadCarrierAccountSelector() {
            var loadCarrierAccountSelectorDeferred = UtilsService.createPromiseDeferred();
            newCountryCarrierAccountSelectorReadyDeferred.promise.then(function () {
                var carrierAccountselectorPayload = {
                    filter: {
                        Filters: getCarrierAccountSelectorNewCountryFilter()
                    }

                };
                VRUIUtilsService.callDirectiveLoad(newCountryCarrierAccountSelectorAPI, carrierAccountselectorPayload, loadCarrierAccountSelectorDeferred);
            });
            return loadCarrierAccountSelectorDeferred.promise;
        }
        function getCarrierAccountSelectorNewCountryFilter() {
            var carrierAccountNewCountryFilters = [];

            var carrierAccountNewCountryFilterAffectedCustomer = {
                $type: 'TOne.WhS.Sales.Business.AffectedCarrierAccountFilterInNewCountryChanges, TOne.WhS.Sales.Business',
                ProcessInstanceId: processInstanceId
            };
            carrierAccountNewCountryFilters.push(carrierAccountNewCountryFilterAffectedCustomer);
            return carrierAccountNewCountryFilters;
        }
        function loadNewCountryPreviewGrid() {
            var newCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
            newCountryGridReadyDeferred.promise.then(function () {
                var newCountryGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showNewCountriesCarrierAccountSelector : true,
                };
                VRUIUtilsService.callDirectiveLoad(newCountryGridAPI, newCountryGridPayload, newCountryGridLoadDeferred);
            });
            return newCountryGridLoadDeferred.promise;
        }

        function loadNewCountryPreview() {
            var loadNewCountryPreviewPromiseDeffered = UtilsService.createPromiseDeferred();
            changedCountryLoadCarrierAccountSelector().then(function (response) {
                $scope.scopeModel.showNewCountriesCarrierAccountSelector = !newCountryCarrierAccountSelectorAPI.hasSingleItem();
                loadNewCountryPreviewGrid().then(function (response) {
                    loadNewCountryPreviewPromiseDeffered.resolve();
                });
            })
            return loadNewCountryPreviewPromiseDeffered.promise;
        }
        // #endregion

        // #region ChangedCountryTab
        function changedCountryLoadCarrierAccountSelector() {
            var loadCarrierAccountSelectorDeferred = UtilsService.createPromiseDeferred();
            changedCountryCarrierAccountSelectorReadyDeferred.promise.then(function () {
                var carrierAccountselectorPayload = {
                    filter: {
                        Filters: getCarrierAccountSelectorChangedCountryFilter()
                    }
                };
                VRUIUtilsService.callDirectiveLoad(changedCountryCarrierAccountSelectorAPI, carrierAccountselectorPayload, loadCarrierAccountSelectorDeferred);
            });
            return loadCarrierAccountSelectorDeferred.promise;
        }
        function getCarrierAccountSelectorChangedCountryFilter() {
            var carrierAccountChangedCountryFilters = [];

            var carrierAccountChangedCountryFilterAffectedCustomer = {
                $type: 'TOne.WhS.Sales.Business.AffectedCarrierAccountFilterInChangedCountryChanges, TOne.WhS.Sales.Business',
                ProcessInstanceId: processInstanceId
            };
            carrierAccountChangedCountryFilters.push(carrierAccountChangedCountryFilterAffectedCustomer);
            return carrierAccountChangedCountryFilters;
        }
        function loadChangedCountryPreviewGrid() {
            var changedCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
            changedCountryGridReadyDeferred.promise.then(function () {
                var changedCountryGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    ShowCustomerName: (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) ? $scope.scopeModel.showChangedCountriesCarrierAccountSelector : true,
                };
                VRUIUtilsService.callDirectiveLoad(changedCountryGridAPI, changedCountryGridPayload, changedCountryGridLoadDeferred);
            });
            return changedCountryGridLoadDeferred.promise;
        }

        function loadChangedCountryPreview() {
            var loadChangedCountryPreviewPromiseDeffered = UtilsService.createPromiseDeferred();
            changedCountryLoadCarrierAccountSelector().then(function (response) {
                $scope.scopeModel.showChangedCountriesCarrierAccountSelector = !changedCountryCarrierAccountSelectorAPI.hasSingleItem();
                loadChangedCountryPreviewGrid().then(function (response) {
                    loadChangedCountryPreviewPromiseDeffered.resolve();
                });
            })
            return loadChangedCountryPreviewPromiseDeffered.promise;
        }
        // #endregion


        function loadProductRatePreviewGrid() {
            var ratePreviewGridLoadDeferred = UtilsService.createPromiseDeferred();

            productRatePreviewGridReadyDeferred.promise.then(function () {
                var ratePreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    ZoneName: null
                };
                var query = {
                    ProcessInstanceId: processInstanceId,
                    CustomerIds: null
                };
                WhS_Sales_RatePlanPreviewAPIService.GetProductRatePlanPreviewSummary(query).then(function (response) {
                    if (response == undefined || response == null)
                        return undefined;
                    else {
                        $scope.scopeModel.numberOfNewProductRates = response.NumberOfNewRates;
                        $scope.scopeModel.numberOfIncreasedProductRates = response.NumberOfIncreasedRates;
                        $scope.scopeModel.numberOfDecreasedProductRates = response.NumberOfDecreasedRates;
                    }
                });
                VRUIUtilsService.callDirectiveLoad(productRatePreviewGridAPI, ratePreviewGridPayload, ratePreviewGridLoadDeferred);
            });

            return ratePreviewGridLoadDeferred.promise;
        }
        function loadDefaultRoutingProductPreview() {
            var input = {
                ProcessInstanceId: processInstanceId
            };
            return WhS_Sales_RatePlanPreviewAPIService.GetDefaultRoutingProductPreview(input).then(function (response) {
                if (response != null) {
                    $scope.scopeModel.currentDefaultRoutingProductName = response.CurrentDefaultRoutingProductName;
                    if (response.IsCurrentDefaultRoutingProductInherited === true)
                        $scope.scopeModel.currentDefaultRoutingProductName += ' (Inherited)';
                    $scope.scopeModel.newDefaultRoutingProductName = response.NewDefaultRoutingProductName;
                    $scope.scopeModel.effectiveOn = UtilsService.getShortDate(new Date(response.EffectiveOn));
                }
            });
        }
        function loadSaleZoneRoutingProductPreviewGrid() {
            var saleZoneRoutingProductGridLoadDeferred = UtilsService.createPromiseDeferred();

            saleZoneRoutingProductGridReadyDeferred.promise.then(function () {
                var saleZoneRoutingProductPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId
                };
                VRUIUtilsService.callDirectiveLoad(saleZoneRoutingProductPreviewGridAPI, saleZoneRoutingProductPreviewGridPayload, saleZoneRoutingProductGridLoadDeferred);
            });

            return saleZoneRoutingProductGridLoadDeferred.promise;
        }


        function executeTask(decision) {
            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.Sales.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
                Decision: decision
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: taskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            var titlePromise = UtilsService.createPromiseDeferred();
            if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                WhS_BE_CarrierAccountAPIService.GetCarrierAccountName(ownerId).then(function (response) {
                    $scope.title = "Customer : " + response;
                    titlePromise.resolve();
                });
            }
            else {
                WhS_BE_SellingProductAPIService.GetSellingProductName(ownerId).then(function (response) {
                    $scope.title = "Selling Product : " + response;
                    titlePromise.resolve();
                });
            }

            return titlePromise.promise;

        }
    }

    appControllers.controller('WhS_Sales_RatePlanPreviewController', RatePlanPreviewController);

})(appControllers);