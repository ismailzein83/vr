(function (appControllers) {

    "use strict";

    supplierIdentificationRuleManagementController.$inject = ['$scope', 'WhS_CDRProcessing_MainService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function supplierIdentificationRuleManagementController($scope, WhS_CDRProcessing_MainService, UtilsService, VRNotificationService, VRUIUtilsService) {
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
            $scope.AddNewSupplierRule = AddNewSupplierRule;
        }

        function load() {
            $scope.isLoadingFilterData = true;
            return UtilsService.waitMultipleAsyncOperations([loadSuppliersSection]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.isLoadingFilterData = false;
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }
        function loadSuppliersSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;
        }

        function getFilterObject() {
            var data = {
                Description: $scope.description,
                SupplierIds: carrierAccountDirectiveAPI.getSelectedIds(),
                OutTrunk: $scope.outtrunk,
                OutCarrier: $scope.outcarrier,
                CDPN: $scope.cdpn,
                EffectiveDate: $scope.effectiveDate
            };
            return data;
        }
     
        function AddNewSupplierRule() {
            var onSupplierIdentificationRuleAdded = function (supplierRuleObj) {
                if (gridAPI != undefined)
                    gridAPI.onSupplierIdentificationRuleAdded(supplierRuleObj);
            };

            WhS_CDRProcessing_MainService.addSupplierIdentificationRule(onSupplierIdentificationRuleAdded);
        }
    }

    appControllers.controller('WhS_CDRProcessing_SupplierIdentificationRuleManagementController', supplierIdentificationRuleManagementController);
})(appControllers);