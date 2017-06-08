(function (appControllers) {

    'use strict';

    LiveCdrManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'NP_IVSwitch_TimeUnitEnum', 'NP_IVSwitch_CallsEnum', 'NP_IVSwitch_SortingEnum'];

    function LiveCdrManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, NP_IVSwitch_TimeUnitEnum, NP_IVSwitch_CallsEnum, NP_IVSwitch_SortingEnum) {

        var gridAPI;
        var customerAccountDirectiveAPI;
        var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var endPointSelectorAPI;
        var endPointSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        var supplierDirectiveAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var routeSelectorAPI;
        var routeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        var viewLiveCdrsPromiseDeferred = UtilsService.createPromiseDeferred();
        var customerId;
        var supplierId;
        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                customerId = (parameters.CustomerId != undefined) ? parameters.CustomerId : undefined;
                supplierId = (parameters.SupplierId != undefined) ? parameters.SupplierId : undefined;
                
            }
        }
        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (customerId != undefined || supplierId != undefined) {
                    viewLiveCdrsPromiseDeferred.promise.then(function () {
                        
                        var query = buildGridQuery();
                        gridAPI.loadGrid(query);
                    });
                    
                }
                else
                {
                    var filter = {
                        query: {},
                        sorting:0 
                    }
                    gridAPI.loadGrid(filter);
                }
               
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
                        selectAll: true,
                        filter: {
                            CustomerIds: customerIds
                        }
                    };
                    var setLoader = function (value) {
                        $scope.isEndPointSelectorLoading = value;
                    };
                    if (customerId != undefined) {
                        VRUIUtilsService.callDirectiveLoad(endPointSelectorAPI, selectorPayload, viewLiveCdrsPromiseDeferred);
                    }
                    else {
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, endPointSelectorAPI, selectorPayload, setLoader, endPointSelectorPromiseDeferred);
                    }
                    
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
                    if (supplierId != undefined) {
                        VRUIUtilsService.callDirectiveLoad(routeSelectorAPI, selectorPayload, viewLiveCdrsPromiseDeferred);
                    }
                    else
                    {
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeSelectorAPI, selectorPayload, setLoader, routeSelectorPromiseDeferred);
                    }

                }
            };
        }
        function load() {
            $scope.isLoadingFilter = true;
            UtilsService.waitMultipleAsyncOperations([loadStaticData, loadCustomers, returnEndPointSelectorPromiseDeferred, loadSuppliers, returnRouteSelectorPromiseDeferred]).then(function () {
                endPointSelectorPromiseDeferred = undefined;
                routeSelectorPromiseDeferred = undefined;
                
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }

        function loadStaticData() {
            $scope.unitTime = UtilsService.getArrayEnum(NP_IVSwitch_TimeUnitEnum);
            $scope.calls = UtilsService.getArrayEnum(NP_IVSwitch_CallsEnum);
            $scope.sortingCalls = UtilsService.getArrayEnum(NP_IVSwitch_SortingEnum);
        }

        function buildGridQuery() {
            var filter = {
                query: {
                    EndPointIds: endPointSelectorAPI.getSelectedIds(),
                    RouteIds: routeSelectorAPI.getSelectedIds(),
                    SourceIP: $scope.sourceIP,
                    RouteIP: $scope.routeIP,
                    CallsMode: ($scope.selectedcalls != undefined) ? $scope.selectedcalls.value : 0,
                    TimeUnit: ($scope.selectedUnitTime != undefined) ? $scope.selectedUnitTime.value : 0,
                    Time: ($scope.time != undefined) ? $scope.time : 0
                },
                sorting: ($scope.selectedsortingCalls != undefined) ? $scope.selectedsortingCalls.value : 0
            };
            return filter;
        }
        function loadCustomers() {
            var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            customerAccountReadyPromiseDeferred.promise.then(function () {                
                var payload={
                    selectedIds:[]
                    };
                if (customerId != undefined)
                    payload.selectedIds.push(customerId);
                VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, payload, loadCustomerAccountPromiseDeferred);
            });

            return loadCustomerAccountPromiseDeferred.promise;
        }
        function loadSuppliers() {
            var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
            supplierReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: []
                };
                if (supplierId != undefined)
                    payload.selectedIds.push(supplierId);
                VRUIUtilsService.callDirectiveLoad(supplierDirectiveAPI, payload, loadSupplierPromiseDeferred);
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
