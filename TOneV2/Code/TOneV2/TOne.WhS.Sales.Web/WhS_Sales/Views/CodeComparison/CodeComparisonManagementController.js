(function (appControllers) {

    "use strict";

    codeCompareManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'WhS_Sales_CodeCompareAPIService'];

    function codeCompareManagementController($scope, UtilsService, VRUIUtilsService, WhS_Sales_CodeCompareAPIService) {
        var gridAPI;
        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var selectorAPI;
        var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.scopeModel = {};
            $scope.loadClicked = function () {
                setFilterObject();
                var gridPayload = {
                    query: filter,
                    selectedSuppliers: UtilsService.cloneObject($scope.selectedSuppliers)
                };
                return gridAPI.load(gridPayload);
            };
            $scope.exportClicked = function () {
                setFilterObject();
                return WhS_Sales_CodeCompareAPIService.ExportCodeCompareTemplate(filter).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyPromiseDeferred.resolve();
            };
            $scope.onSellingNumberPlanSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };
            $scope.validateThreshold = function () {
                if (carrierAccountDirectiveAPI.getSelectedIds() == null)
                    return "Select Suppliers";
                if ($scope.threshold > 0 && $scope.threshold <= carrierAccountDirectiveAPI.getSelectedIds().length)
                    return null;
                else {
                    if ($scope.threshold < 0)
                        return "negative number";
                    if ($scope.threshold > carrierAccountDirectiveAPI.getSelectedIds().length)
                        return "Maximum value : " + carrierAccountDirectiveAPI.getSelectedIds().length;
                }
            };

            $scope.scopeModel.SupplierZoneNames = [];
            $scope.onSelectItem = function (selectedValue) {
                $scope.scopeModel.SupplierZoneNames.push(selectedValue);
            };
            $scope.onDeselectItem = function (selectedValue) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.SupplierZoneNames, selectedValue.CarrierAccountId, 'CarrierAccountId');
                $scope.scopeModel.SupplierZoneNames.splice(datasourceIndex, 1);
            };
            $scope.onDeselectAll = function () {
                $scope.scopeModel.SupplierZoneNames = [];
            };
        }

        function load() {
            $scope.isLoadingFilter = true;

            UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans, loadCarrierAccountSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }



        function loadSellingNumberPlans() {
            var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, { selectifsingleitem: true }, loadSNPPromiseDeferred);
            });

            return loadSNPPromiseDeferred.promise;
        }

        function loadCarrierAccountSelector() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;

        }

        function setFilterObject() {
            filter = {
                threshold: $scope.threshold,
                sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                supplierIds: carrierAccountDirectiveAPI.getSelectedIds(),
                codeStartWith: ($scope.codeStartWith != null) ? $scope.codeStartWith : null,
                ZoneNameSupplierId: $scope.scopeModel.selectedSupplierZoneName != undefined ? $scope.scopeModel.selectedSupplierZoneName.CarrierAccountId : null
            };
        }

    }

    appControllers.controller('WhS_Sales_CodeCompareManagementController', codeCompareManagementController);
})(appControllers);