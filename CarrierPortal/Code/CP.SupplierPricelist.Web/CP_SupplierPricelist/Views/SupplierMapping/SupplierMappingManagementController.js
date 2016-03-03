﻿(
    function (appControllers) {
        "use strict";

        function supplierManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, supplierMappingService, supplierMappingAPIService, customerUserAPIService) {
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
                    gridAPI.loadGrid({});
                }

                $scope.onUserDirectiveReady = function (api) {
                    userDirectiveApi = api;
                    userReadyPromiseDeferred.resolve();
                }
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveApi = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                }
                $scope.hasAddSupplierMappingPermission = function () {
                    return supplierMappingAPIService.HasAddCustomerSupplierMapping();
                }
                $scope.AddSupplierMapping = function () {
                    var onSupplierMappingAdded = function (supplierMapping) {
                        gridAPI.onSupplierMappingAdded(supplierMapping);
                    };
                    supplierMappingService.addSupplierMapping(onSupplierMappingAdded);
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
                return customerUserAPIService.GetHasCurrentCustomerId().then(function (response) {
                    if (response == true) {
                        loadAllControls()
                    }
                    else {
                        $scope.isLoadingFilters = false;
                        VRNotificationService.notifyExceptionWithClose("This user has no related customer.");
                    }
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoadingFilters = false;
                });
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadUser, loadCarrierAccount ])
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   })
                  .finally(function () {
                      $scope.isLoadingFilters = false;
                  });
            }
            function loadUser() {
                var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                userReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(userDirectiveApi, {filter: { ExcludeInactive: true } }, userLoadPromiseDeferred);
                });
                return userLoadPromiseDeferred.promise;
            }

            function loadCarrierAccount() {
                var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    

                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveApi, undefined , carrierAccountLoadPromiseDeferred);
                });
                return carrierAccountLoadPromiseDeferred.promise;
            }
            

        }

        supplierManagementController.$inject = [
            '$scope',
            'UtilsService',
            'VRNotificationService',
            'VRUIUtilsService',
            'CP_SupplierPricelist_SupplierMappingService',
            'CP_SupplierPricelist_SupplierMappingAPIService',
            'CP_SupplierPricelist_CustomerUserAPIService'

        ];
        appControllers.controller('CP_SupplierPriceList_SupplierManagementController', supplierManagementController);
    }
)(appControllers);