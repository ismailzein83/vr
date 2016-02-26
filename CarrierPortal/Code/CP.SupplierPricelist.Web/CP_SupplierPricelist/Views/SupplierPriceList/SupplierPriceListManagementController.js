(
    function (appControllers) {
        "use strict";

        function supplierPriceListManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, priceListTypeEnum, supplierPriceListAPIService) {
            var gridAPI;
            var customerDirectiveApi;
            var customerReadyPromiseDeferred = utilsService.createPromiseDeferred();
            function importPriceList() {
                var priceListObject = {
                    FileId: $scope.file.fileId,
                    PriceListType: $scope.selectedpriceListType.pricelistID,
                    EffectiveOnDate: $scope.effectiveOn,
                    UserId: 1,
                    Satus: 0,
                    CustomerId: 1,
                    CarrierAccountId: 'C169'
                };
                supplierPriceListAPIService.importPriceList(priceListObject);
            }

            function defineScope() {
                $scope.priceListTypes = [];
                $scope.importPriceList = function () {
                    importPriceList();
                }
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }
            }

            defineScope();

            function loadPriceListType() {
                angular.forEach(priceListTypeEnum, function (value, key) {
                    $scope.priceListTypes.push({ pricelistID: value.ID, title: value.Value });
                }
                );
            }
            $scope.onCustomerDirectiveReady = function (api) {
                customerDirectiveApi = api;
                customerReadyPromiseDeferred.resolve();
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

        supplierPriceListManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPriceList_PriceListTypeEnum', 'CP_SupplierPricelist_SupplierPriceListAPIService'];
        appControllers.controller('CP_SupplierPriceList_SupplierPriceListManagementController', supplierPriceListManagementController);
    }
)(appControllers);