(
    function (appControllers) {
        "use strict";

        function supplierManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, supplierMappingService) {
            var gridAPI;

            var userDirectiveApi;
            var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountDirectiveApi;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            defineScope();
            load()
            function defineScope() {

                $scope.searchClicked = function () {
                    return gridAPI.loadGrid(getFilterObject());
                }; 
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid({});
                }

                $scope.onUserDirectiveReady = function (api) {
                    userDirectiveApi = api;
                    userReadyPromiseDeferred.resolve();
                }
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveApi = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                }

                $scope.AddSupplierMapping = function () {
                    supplierMappingService.addSupplierMapping();
                }
            }

            function getFilterObject() {
                var data = {
                    Users: userDirectiveApi.getSelectedIds(),
                    CarrierAccouts: carrierAccountDirectiveApi.getSelectedIds(),
                };
                return data;
            }

            function load() {
                $scope.isLoadingFilters = true;
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([LoadUser, LoadCarrierAccount])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
            function LoadUser() {
                var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                userReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(userDirectiveApi, undefined, userLoadPromiseDeferred);
                });
                return userLoadPromiseDeferred.promise;
            }

            function LoadCarrierAccount() {
                var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                carrierAccountReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveApi, undefined, carrierAccountLoadPromiseDeferred);
                });
                return carrierAccountLoadPromiseDeferred.promise;
            }


        }

        supplierManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierMappingService'];
        appControllers.controller('CP_SupplierPriceList_SupplierManagementController', supplierManagementController);
    }
)(appControllers);