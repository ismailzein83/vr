﻿(function (appControllers) {

    "use strict";

    customerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function customerRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        var routingDatabaseSelectorAPI;
        var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onRoutingDatabaseSelectorReady = function (api) {
                routingDatabaseSelectorAPI = api;
                routingDatabaseReadyPromiseDeferred.resolve();
            }

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            function getFilterObject() {
                
                var query = {
                    RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
                    Code: $scope.code,
                    CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                    LimitResult: $scope.limit
                };
                return query;
            }
        }
        function load() {
            $scope.isLoadingFilterData = true;
            $scope.limit = 1000;
            return UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadCustomersSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function loadRoutingDatabaseSelector()
        {
            var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

            routingDatabaseReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, undefined, loadRoutingDatabasePromiseDeferred);
            });

            return loadRoutingDatabasePromiseDeferred.promise;
        }
        function loadCustomersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_Routing_CustomerRouteManagementController', customerRouteManagementController);
})(appControllers);