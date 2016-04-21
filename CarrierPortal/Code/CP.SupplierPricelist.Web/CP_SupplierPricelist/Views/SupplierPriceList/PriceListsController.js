(
    function (appControllers) {
        "use strict";

        function priceListsController($scope, utilsService, vrNotificationService, vruiUtilsService, CP_SupplierPriceList_PriceListTypeEnum, CP_SupplierPricelist_PriceListResultEnum, CP_SupplierPricelist_PriceListStatusEnum, vrValidationService) {
            var gridAPI;
            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();
            var carrierDirectiveApi;
            var carrierReadyPromiseDeferred = utilsService.createPromiseDeferred();
            var PricelistTypeDirectiveAPI;
            var PricelistTypeReadyPromiseDeferred = utilsService.createPromiseDeferred();

            defineScope();
            load();
            function defineScope() {
                $scope.fromEffectiveDate = new Date();
              //  $scope.toEffectiveDate = new Date();
                $scope.priceListTypes = [];
                $scope.priceListResults = [];
                $scope.priceListStatus = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    //api.loadGrid({});
                }
                $scope.validateDateTime = function () {
                    return vrValidationService.validateTimeRange($scope.fromEffectiveDate, $scope.toEffectiveDate);
                }
                $scope.searchClicked = function () {
                    return gridAPI.loadGrid(getFilterObject());
                };
                $scope.onCustomerDirectiveReady = function (api) {
                    customerDirectiveApi = api;
                    customerReadyPromiseDeferred.resolve();
                }
                $scope.onReadyCarrierAccountSelector = function (api) {
                    carrierDirectiveApi = api;
                    var setLoader = function (value) { $scope.isLoadingCarrierDirective = value };
                    vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierDirectiveApi, undefined, setLoader, carrierReadyPromiseDeferred);
                }
                $scope.onCustomerSelectionChanged = function () {
                   var listCustomer = customerDirectiveApi.getSelectedIds();
                    if (listCustomer && listCustomer.length == 0) {
                      //  $scope.carrierAccount = undefined;
                        carrierDirectiveApi.load({ filter: undefined });
                    }
                    else if (listCustomer != undefined && listCustomer.length < 2)
                    {
                     //   $scope.carrierAccount = undefined;
                        var obj = {
                            filter: {
                                CustomerIdForCurrentSupplier: listCustomer[0]
                            }
                        }
                        carrierDirectiveApi.load(obj);
                    }
                }
                $scope.onPricelistTypeDirectiveReady = function (api) {
                    PricelistTypeDirectiveAPI = api;
                    PricelistTypeReadyPromiseDeferred.resolve();
                }
            }
            function load() {
                loadAllControls();
            }
            function loadAllControls() {
                $scope.isLoadingFilters = true;
                return utilsService.waitMultipleAsyncOperations([loadPriceListType, loadPriceListStatus, loadPriceListResult, loadCustomer, loadPricelistTypesSelector])
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
            function loadPriceListType() {
                angular.forEach(CP_SupplierPriceList_PriceListTypeEnum, function (value, key) {
                    $scope.priceListTypes.push({ pricelistTypeID: value.ID, title: value.Value });
                }
                );
            }
            function loadPriceListStatus() {
                angular.forEach(CP_SupplierPricelist_PriceListStatusEnum, function (value, key) {
                    $scope.priceListStatus.push({ statusID: value.value, title: value.description });
                }
                );
            }
            function loadPriceListResult() {
                angular.forEach(CP_SupplierPricelist_PriceListResultEnum, function (value, key) {
                    $scope.priceListResults.push({ resultID: value.value, title: value.description });
                }
                );
            }         
            function loadCustomer() {
                var customerLoadPromiseDeferred = utilsService.createPromiseDeferred();
                customerReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(customerDirectiveApi, { filter: { AssignedToCurrentSupplier: true } }, customerLoadPromiseDeferred);
                });
                return customerLoadPromiseDeferred.promise;
            }
            function loadPricelistTypesSelector() {
                var pricelistTypeLoadPromiseDeferred = utilsService.createPromiseDeferred();
                PricelistTypeReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(PricelistTypeDirectiveAPI, undefined,pricelistTypeLoadPromiseDeferred);
                });
                return pricelistTypeLoadPromiseDeferred.promise;
            }
            function getFilterObject() {
                var data = {
                    CarriersID: customerDirectiveApi.getSelectedIds(),
                    PriceListType: $scope.selectedpriceListType != undefined ? $scope.selectedpriceListType.pricelistTypeID : -1,
                    PriceListResult: $scope.selectedpriceListResult != undefined ? $scope.selectedpriceListResult.resultID : -1,
                    PriceListStatus: $scope.selectedpriceListStatus != undefined ? $scope.selectedpriceListStatus.statusID : -1,
                    FromEffectiveOnDate: $scope.fromEffectiveDate,
                    ToEffectiveOnDate: $scope.toEffectiveDate,
                    CarrierAccounts: carrierDirectiveApi.getSelectedIds()
                };
                return data;
            }    
        }

        priceListsController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPriceList_PriceListTypeEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum', 'VRValidationService'];
        appControllers.controller('CP_SupplierPriceList_PriceListsController', priceListsController);
    }
)(appControllers);