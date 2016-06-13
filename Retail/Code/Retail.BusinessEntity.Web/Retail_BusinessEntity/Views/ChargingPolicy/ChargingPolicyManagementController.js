(function (appControllers) {

    'use strict';

    ChargingPolicyManagementController.$inject = ['$scope', 'Retail_BE_ChargingPolicyService', 'Retail_BE_ChargingPolicyAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function ChargingPolicyManagementController($scope, Retail_BE_ChargingPolicyService, Retail_BE_ChargingPolicyAPIService, UtilsService, VRUIUtilsService, VRNotificationService)
    {
        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        defineScope();
        load();

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function ()
            {
                var onChargingPolicyAdded = function (addedChargingPolicy) {
                    gridAPI.onChargingPolicyAdded(addedChargingPolicy);
                };

                Retail_BE_ChargingPolicyService.addChargingPolicy(onChargingPolicyAdded);
            };

            $scope.scopeModel.hasAddChargingPolicyPermission = function () {
                return Retail_BE_ChargingPolicyAPIService.HasAddChargingPolicyPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls()
        {
            return UtilsService.waitMultipleAsyncOperations([loadServiceTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadServiceTypeSelector()
        {
            var serviceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            serviceTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, undefined, serviceTypeSelectorLoadDeferred);
            });

            return serviceTypeSelectorLoadDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                ServiceTypeIds: serviceTypeSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_ChargingPolicyManagementController', ChargingPolicyManagementController);

})(appControllers);