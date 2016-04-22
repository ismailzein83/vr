(
    function (appControllers) {
        "use strict";

        function priceListsController($scope, utilsService, vrNotificationService, vruiUtilsService,vrValidationService) {
            var gridAPI;

            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var carrierDirectiveApi;
            var carrierReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var typeDirectiveAPI;
            var typeReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var resultDirectiveAPI;
            var resultReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var statusDirectiveAPI;
            var statusReadyPromiseDeferred = utilsService.createPromiseDeferred();


            defineScope();
            load();
            function defineScope() {
                $scope.fromEffectiveDate = new Date();
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPI.loadGrid(getFilterObject())
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
                    carrierReadyPromiseDeferred.resolve();
                }
                $scope.onCustomerSelectionChanged = function () {
                   var listCustomer = customerDirectiveApi.getSelectedIds();

                   if (listCustomer != undefined && listCustomer.length < 2)
                   {
                        var obj = {
                            filter: {
                                CustomerIdForCurrentSupplier: listCustomer[0]
                            }
                        }
                        carrierDirectiveApi.load(obj);
                    }
                }
                $scope.onPricelistTypeDirectiveReady = function (api) {
                    typeDirectiveAPI = api;
                    typeReadyPromiseDeferred.resolve();
                }

                $scope.onPricelistResultDirectiveReady = function (api) {
                    resultDirectiveAPI = api;
                    resultReadyPromiseDeferred.resolve();
                }

                $scope.onPricelistStatusDirectiveReady = function (api) {
                    statusDirectiveAPI = api;
                    statusReadyPromiseDeferred.resolve();
                }

               
            }
            function load() {
                loadAllControls();
            }
            function loadAllControls() {
                $scope.isLoadingFilters = true;
                return utilsService.waitMultipleAsyncOperations([loadCustomer,  loadPriceListType, loadPriceListResult, loadPriceListStatus ])
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
            
            function loadCustomer() {
                var customerLoadPromiseDeferred = utilsService.createPromiseDeferred();
                customerReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(customerDirectiveApi, { filter: { AssignedToCurrentSupplier: true } }, customerLoadPromiseDeferred);
                });
                return customerLoadPromiseDeferred.promise;
            }
            function loadPriceListType() {
                var pricelistTypeLoadPromiseDeferred = utilsService.createPromiseDeferred();
                typeReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(typeDirectiveAPI, undefined,pricelistTypeLoadPromiseDeferred);
                });
                return pricelistTypeLoadPromiseDeferred.promise;
            }
            function loadPriceListResult() {
                var pricelistResultLoadPromiseDeferred = utilsService.createPromiseDeferred();
                resultReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(resultDirectiveAPI, undefined, pricelistResultLoadPromiseDeferred);
                });
                return pricelistResultLoadPromiseDeferred.promise;
            }
            function loadPriceListStatus() {
                var pricelistStatusLoadPromiseDeferred = utilsService.createPromiseDeferred();
                statusReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(statusDirectiveAPI, undefined, pricelistStatusLoadPromiseDeferred);
                });
                return pricelistStatusLoadPromiseDeferred.promise;
            }
           
            function getFilterObject() {
                var data = {
                    CustomersIDs:  customerDirectiveApi.getSelectedIds(),
                    CarrierAccounts: $scope.customers.length == 1 ? carrierDirectiveApi.getSelectedIds() : null,
                    PriceListTypes: typeDirectiveAPI.getSelectedIds(),
                    PriceListResults: resultDirectiveAPI.getSelectedIds(),
                    PriceListStatuses: statusDirectiveAPI.getSelectedIds(),
                    FromEffectiveOnDate: $scope.fromEffectiveDate,
                    ToEffectiveOnDate: $scope.toEffectiveDate
                };
                return data;
            }    
        }

        priceListsController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService'];
        appControllers.controller('CP_SupplierPriceList_PriceListsController', priceListsController);
    }
)(appControllers);