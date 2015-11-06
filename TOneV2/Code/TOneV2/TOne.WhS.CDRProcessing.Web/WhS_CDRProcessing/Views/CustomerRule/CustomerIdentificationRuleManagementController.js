(function (appControllers) {

    "use strict";

    customerIdentificationRuleManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function customerIdentificationRuleManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.AddNewCustomerRule = AddNewCustomerRule;
        }

        function load() {
            $scope.isLoadingFilterData = true;
            return UtilsService.waitMultipleAsyncOperations([loadCustomersSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.isLoadingFilterData = false;
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
           
        }
        function loadCustomersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
                CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
                InTrunk:$scope.intrunk,
                InCarrier:$scope.incarrier,
                CDPN:$scope.cdpn
            };
            return data;
        }

        function AddNewCustomerRule() {
            var onCustomerIdentificationRuleAdded = function (customerRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onCustomerIdentificationRuleAdded(customerRuleObj);
            };

            WhS_CDRProcessing_MainService.addCustomerIdentificationRule(onCustomerIdentificationRuleAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_CustomerIdentificationRuleManagementController', customerIdentificationRuleManagementController);
})(appControllers);