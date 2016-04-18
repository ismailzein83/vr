(
    function (appControllers) {
        "use strict";

        function priceListsController($scope, utilsService, vrNotificationService, vruiUtilsService, priceListTypeEnum, priceListResultEnum, priceListStatusEnum, vrValidationService) {
            var gridAPI;
            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();
            var carrierDirectiveApi;
            var carrierReadyPromiseDeferred = utilsService.createPromiseDeferred();

            function defineScope() {
                $scope.fromEffectiveDate = new Date();
                $scope.toEffectiveDate = new Date();
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
                    console.log("in")
                    var listCustomer = customerDirectiveApi.getSelectedIds();


                    if (listCustomer && listCustomer.length == 0) {
                        $scope.carrierAccount = undefined;
                        carrierDirectiveApi.load({ filter: undefined });
                    }
                    else if (listCustomer != undefined && listCustomer.length < 2)
                    {
                        $scope.carrierAccount = undefined;
                        var obj = {
                            filter: {
                                CustomerIdForCurrentSupplier: listCustomer[0]
                            }
                        }
                        carrierDirectiveApi.load(obj);
                    }
                }
            }
            function loadPriceListType() {
                angular.forEach(priceListTypeEnum, function (value, key) {
                    $scope.priceListTypes.push({ pricelistTypeID: value.ID, title: value.Value });
                }
                );
            }
            function loadPriceListResult() {
                angular.forEach(priceListResultEnum, function (value, key) {
                    $scope.priceListResults.push({ resultID: value.value, title: value.description });
                }
                );
            }
            function loadPriceListStatus() {
                angular.forEach(priceListStatusEnum, function (value, key) {
                    $scope.priceListStatus.push({ statusID: value.value, title: value.description });
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
            function loadAllControls() {
                return utilsService.waitMultipleAsyncOperations([loadPriceListType, loadPriceListStatus, loadPriceListResult, loadCustomer])
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
            defineScope();
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
            function load() {
                loadAllControls();
            }
            load();
        }

        priceListsController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPriceList_PriceListTypeEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum', 'VRValidationService'];
        appControllers.controller('CP_SupplierPriceList_PriceListsController', priceListsController);
    }
)(appControllers);