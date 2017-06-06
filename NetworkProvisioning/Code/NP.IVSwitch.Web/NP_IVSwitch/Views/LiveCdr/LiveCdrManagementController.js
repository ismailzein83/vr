(function (appControllers) {

    'use strict';

    LiveCdrManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService'];

    function LiveCdrManagementController($scope, UtilsService,VRUIUtilsService) {

        var gridAPI;
        var customerAccountDirectiveAPI;
        var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var endPointSelectorAPI;
        var endPointSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var routeSelectorAPI;
        var routeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };
            $scope.onCustomerAccountDirectiveReady = function (api) {
                customerAccountDirectiveAPI = api;
                customerAccountReadyPromiseDeferred.resolve();
            };
            $scope.onSelectorEndPointReady = function (api) {
                endPointSelectorAPI = api;
                endPointSelectorPromiseDeferred.resolve();
            };
            $scope.onSupplierDirectiveReady = function (api) {
                supplierDirectiveAPI = api;
                supplierReadyPromiseDeferred.resolve();
            };
            $scope.onSelectorRouteReady = function (api) {
                routeSelectorAPI = api;
                routeSelectorPromiseDeferred.resolve();
            };
            $scope.onCustomerSelectionChanged = function (value) {
                var selectedIds = customerAccountDirectiveAPI.getSelectedIds();
                if (selectedIds != undefined) {
                    var customerIds = selectedIds;
                    var selectorPayload = {
                        selectAll:true,
                        filter: {
                            CustomerIds: customerIds
                        }
                    };
                    var setLoader = function (value) {
                        $scope.isEndPointSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, endPointSelectorAPI, selectorPayload, setLoader, endPointSelectorPromiseDeferred);
                }
            };
            $scope.onSupplierSelectionChanged = function (value) {
                var selectedIds = supplierDirectiveAPI.getSelectedIds();
                if (selectedIds != undefined) {
                    var supplierIds = selectedIds;
                    var selectorPayload = {
                        selectAll: true,
                        filter: {
                            SupplierIds: supplierIds
                        }
                    };
                    var setLoader = function (value) {
                        $scope.isRouteSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeSelectorAPI, selectorPayload, setLoader, routeSelectorPromiseDeferred);
                }
            };
        }
        function load() {
            $scope.isLoadingFilter = true;
            UtilsService.waitMultipleAsyncOperations([loadCustomers, returnEndPointSelectorPromiseDeferred, loadSuppliers, returnRouteSelectorPromiseDeferred]).then(function () {
                endPointSelectorPromiseDeferred = undefined;
                routeSelectorPromiseDeferred = undefined;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }
        function buildGridQuery() {
            return {
                EndPointIds: endPointSelectorAPI.getSelectedIds(),
                RouteIds: routeSelectorAPI.getSelectedIds(),
                SourceIP: $scope.sourceIP,
                RouteIP: $scope.routeIP
            };
        }
        function loadCustomers() {
            var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            customerAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
            });

            return loadCustomerAccountPromiseDeferred.promise;
        }
        function loadSuppliers() {
            var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
            supplierReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, undefined, loadSupplierPromiseDeferred);
            });

            return loadSupplierPromiseDeferred.promise;
        }
        function returnEndPointSelectorPromiseDeferred()
        {
            return endPointSelectorPromiseDeferred.promise;
        }
        function returnRouteSelectorPromiseDeferred() {
            return routeSelectorPromiseDeferred.promise;
        }
    }

    appControllers.controller('NP_IVSwitch_LiveCdrManagementController', LiveCdrManagementController);

})(appControllers);
