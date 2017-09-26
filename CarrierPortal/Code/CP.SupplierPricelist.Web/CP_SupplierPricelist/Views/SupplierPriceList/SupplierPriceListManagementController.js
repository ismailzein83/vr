(
    function (appControllers) {
        "use strict";

        function supplierPriceListManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, priceListTypeEnum, supplierPriceListApiService, VRDateTimeService) {
            var gridAPI;
            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();
            var carrierDirectiveApi;
            var carrierReadyPromiseDeferred = utilsService.createPromiseDeferred();
            function importPriceList() {
                var priceListObject = {
                    FileId: $scope.file.fileId,
                    PriceListType: $scope.selectedpriceListType.pricelistID,
                    EffectiveOnDate: $scope.effectiveOn,
                    CustomerId: customerDirectiveApi.getSelectedIds(),
                    CarrierAccountId: carrierDirectiveApi.getSelectedIds()
                };
                return supplierPriceListApiService.importPriceList(priceListObject)
                .then(function (response) {
                    if (vrNotificationService.notifyOnItemAdded("PriceList", response, 'status')) {

                    }
                }).catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });
            }
            function defineScope() {
                $scope.effectiveOn = VRDateTimeService.getNowDateTime();
                $scope.priceListTypes = [];
                $scope.importPriceList = function () {
                    importPriceList();
                }
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }
                $scope.hasImportPriceListPermission = function () {
                    return supplierPriceListApiService.HasImportPriceList();
                }
                $scope.onReadyCarrierAccountSelector = function (api) {
                    carrierDirectiveApi = api;
                    var setLoader = function (value) { $scope.isLoadingCarrierDirective = value };
                    vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierDirectiveApi, undefined, setLoader, carrierReadyPromiseDeferred);
                }
                $scope.onCustomerSelectionChanged = function () {
                    if (customerDirectiveApi.getSelectedIds() != undefined) {
                        $scope.carrierAccount = undefined;
                        var obj = {
                            filter: {
                                CustomerIdForCurrentSupplier: customerDirectiveApi.getSelectedIds()
                            }
                        }
                        carrierDirectiveApi.load(obj);
                    }
                    else {
                        $scope.carrierAccount = undefined;
                    }

                }
                $scope.onCustomerDirectiveReady = function (api) {
                    customerDirectiveApi = api;
                    customerReadyPromiseDeferred.resolve();
                }
            }

            defineScope();


            function loadPriceListType() {
                angular.forEach(priceListTypeEnum, function (value, key) {
                    $scope.priceListTypes.push({ pricelistID: value.ID, title: value.Value });
                }
                );
            }

            function load() {
                loadAllControls();
            }
            function loadCustomer() {
                var customerLoadPromiseDeferred = utilsService.createPromiseDeferred();
                customerReadyPromiseDeferred.promise.then(function () {
                    vruiUtilsService.callDirectiveLoad(customerDirectiveApi, { filter: { AssignedToCurrentSupplier: true } }, customerLoadPromiseDeferred);
                });
                return customerLoadPromiseDeferred.promise;
            }
            load();
            function loadAllControls() {
                return utilsService.waitMultipleAsyncOperations([loadCustomer, loadPriceListType])
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
        }

        supplierPriceListManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPriceList_PriceListTypeEnum', 'CP_SupplierPricelist_SupplierPriceListAPIService', 'VRDateTimeService'];
        appControllers.controller('CP_SupplierPriceList_SupplierPriceListManagementController', supplierPriceListManagementController);
    }
)(appControllers);