(function (appControllers) {

    "use strict";

    SMSPriceListController.$inject = ["$scope", "VRNotificationService", "UtilsService", "VRUIUtilsService", "WhS_SMSBusinessEntity_CustomerSMSPriceListAPIService"];

    function SMSPriceListController($scope, VRNotificationService, UtilsService, VRUIUtilsService, WhS_SMSBusinessEntity_CustomerSMSPriceListAPIService) {

        var selectedCustomer;

        var customerSelectorAPI;
        var customerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isLoading = true;
            $scope.scopeModel.isLoadingMobileNetworkSelector = false;

            $scope.scopeModel.searchCustomerSMSPriceLists = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getCustomerSMSPriceListQuery() };

                var gridLoadedPromise = gridAPI.load(payload);
                promises.push(gridLoadedPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.hasSearchCustomerPriceListsPermission = function () {
                return WhS_SMSBusinessEntity_CustomerSMSPriceListAPIService.HasSearchPriceListsPermission();
            };

            $scope.scopeModel.onCustomerSelectorReady = function (api) {
                customerSelectorAPI = api;
                customerSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };
        }

        function load() {

            function loadAllControls() {

                var loadCustomerSelectorPromise = loadCustomerSelector();
                var rootPromiseNode = {
                    promises: [gridReadyDeferred.promise],
                    getChildNode: function () {
                        var loadPriceListGrid = gridAPI.load(undefined);
                        return {
                            promises: [loadPriceListGrid, loadCustomerSelectorPromise]
                        };
                    }
                };

                function loadCustomerSelector() {
                    var customerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    customerSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerSelectorAPI, undefined, customerSelectorLoadDeferred);
                    });
                    return customerSelectorLoadDeferred.promise;
                }

                return UtilsService.waitPromiseNode(rootPromiseNode).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getCustomerSMSPriceListQuery() {
            return {
                CustomerIds: customerSelectorAPI.getSelectedIds(),
                EffectiveDate: $scope.scopeModel.effectiveDate
            };
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_CustomerSMSPriceListPlanController", SMSPriceListController);

})(appControllers);