(
    function (appControllers) {
        "use strict";

        function priceListsController($scope, utilsService, vrNotificationService, vruiUtilsService, vrValidationService, VRDateTimeService) {
            var gridAPI;

            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var carrierDirectiveApi;
            var carrierReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var typeDirectiveAPI;
            var typeReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var resultDirectiveAPI;
            var resultReadyPromiseDeferred = utilsService.createPromiseDeferred();

            var groupedStatusDirectiveAPI;
            var groupedStatusReadyPromiseDeferred = utilsService.createPromiseDeferred();


            defineScope();
            load();
            function defineScope() {
                $scope.fromEffectiveDate = VRDateTimeService.getNowDateTime();
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

                    if (listCustomer != undefined && listCustomer.length < 2) {
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

                $scope.onPricelistGroupedStatusDirectiveReady = function (api) {
                    groupedStatusDirectiveAPI = api;
                    groupedStatusReadyPromiseDeferred.resolve();
                }


            }
            function load() {
                loadAllControls();
            }
            function loadAllControls() {
                $scope.isLoadingFilters = true;
                return utilsService.waitMultipleAsyncOperations([loadCustomer, loadPriceListType, loadPriceListResult, loadPriceListGroupedStatus])
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
                    vruiUtilsService.callDirectiveLoad(typeDirectiveAPI, undefined, pricelistTypeLoadPromiseDeferred);
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
            function loadPriceListGroupedStatus() {
                var pricelistGroupedStatusLoadPromiseDeferred = utilsService.createPromiseDeferred();
                groupedStatusReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(groupedStatusDirectiveAPI, undefined, pricelistGroupedStatusLoadPromiseDeferred);
                });
                return pricelistGroupedStatusLoadPromiseDeferred.promise;
            }

            function getFilterObject() {
                var data = {
                    CustomersIDs: customerDirectiveApi.getSelectedIds(),
                    PriceListTypes: typeDirectiveAPI.getSelectedIds(),
                    PriceListResults: resultDirectiveAPI.getSelectedIds(),
                    PriceListStatuses: groupedStatusDirectiveAPI.getSelectedIds(),
                    FromEffectiveOnDate: $scope.fromEffectiveDate,
                    ToEffectiveOnDate: $scope.toEffectiveDate
                };
                if ($scope.customers.length == 1)
                    data.CarrierAccounts = carrierDirectiveApi.getSelectedIds();
                return data;
            }
        }

        priceListsController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'VRDateTimeService'];
        appControllers.controller('CP_SupplierPriceList_PriceListsController', priceListsController);
    }
)(appControllers);