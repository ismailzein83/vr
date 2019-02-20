(function (appControllers) {

    "use strict";

    SMSPriceListController.$inject = ["$scope", "VRNotificationService", "UtilsService", "VRUIUtilsService", "WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService"];

    function SMSPriceListController($scope, VRNotificationService, UtilsService, VRUIUtilsService, WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService) {

        var selectedSupplier;

        var supplierSelectorAPI;
        var supplierSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isLoading = true;
            $scope.scopeModel.isLoadingMobileNetworkSelector = false;

            $scope.scopeModel.searchSupplierSMSPriceLists = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getSupplierSMSPriceListQuery() };

                var gridLoadedPromise = gridAPI.load(payload);
                promises.push(gridLoadedPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.hasSearchSupplierPriceListsPermission = function () {
                return WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService.HasSearchPriceListsPermission();
            };

            $scope.scopeModel.onSupplierSelectorReady = function (api) {
                supplierSelectorAPI = api;
                supplierSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };
        }

        function load() {

            function loadAllControls() {

                var loadSupplierSelectorPromise = loadSupplierSelector();
                var rootPromiseNode = {
                    promises: [gridReadyDeferred.promise],
                    getChildNode: function () {
                        var loadPriceListGrid = gridAPI.load(undefined);
                        return {
                            promises: [loadPriceListGrid, loadSupplierSelectorPromise]
                        };
                    }
                };

                function loadSupplierSelector() {
                    var supplierSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    supplierSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, undefined, supplierSelectorLoadDeferred);
                    });
                    return supplierSelectorLoadDeferred.promise;
                }

                return UtilsService.waitPromiseNode(rootPromiseNode).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getSupplierSMSPriceListQuery() {
            return {
                SupplierIds: supplierSelectorAPI.getSelectedIds(),
                EffectiveDate: $scope.scopeModel.effectiveDate
            };
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_SupplierSMSPriceListPlanController", SMSPriceListController);

})(appControllers);