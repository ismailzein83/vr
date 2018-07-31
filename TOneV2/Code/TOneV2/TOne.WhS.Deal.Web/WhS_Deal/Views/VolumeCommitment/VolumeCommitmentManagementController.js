(function (app) {

    "use strict";

    volumeCommitmentManagementController.$inject = ["$scope", "WhS_Deal_VolCommitmentDealAPIService", "WhS_Deal_VolumeCommitmentService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_Deal_VolumeCommitmentTypeEnum"];

    function volumeCommitmentManagementController($scope, WhS_Deal_VolCommitmentDealAPIService, WhS_Deal_VolumeCommitmentService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Deal_VolumeCommitmentTypeEnum) {
        var gridAPI;
        var supplierApi;
        var customerApi;
        var typeApi;

        var statusSelectorAPI;
        var statusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                api.load(getFilterObject());
            };
            $scope.scopeModel.searchClicked = function () {
                return gridAPI.load(getFilterObject());
            };
            $scope.scopeModel.onVolCommitmentTypeDirectiveReady = function (api) {
                typeApi = api;
                typeApi.load();
            };
            $scope.scopeModel.onStatusSelectorReady = function (api) {
                statusSelectorAPI = api;
                statusSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.hasAddVolCommitmentDealPermission = function () {
                return WhS_Deal_VolCommitmentDealAPIService.HasAddDealPermission();
            };

            $scope.scopeModel.onSupplierDirectiveReady = function (api) {
                $scope.scopeModel.isLoading = true;
                supplierApi = api;
                supplierApi.load().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
               
            };
            $scope.scopeModel.onCustomerDirectiveReady = function (api) {
                $scope.scopeModel.isLoading = true;
                customerApi = api;
                customerApi.load().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });                
            };

            $scope.scopeModel.addVolumeCommitment = addVolumeCommitment;
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([loadStatusSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadStatusSelector() {
            var statusSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            statusSelectorPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(statusSelectorAPI, undefined, statusSelectorLoadDeferred);
            });
            return statusSelectorLoadDeferred.promise;
        }

        function addVolumeCommitment() {
            var onVolumeCommitmentAdded = function (addedItem) {
                gridAPI.onVolumeCommitmentAdded(addedItem);
            };
            WhS_Deal_VolumeCommitmentService.addVolumeCommitment(onVolumeCommitmentAdded);
        }

        function getFilterObject() {
            var filter = {
                Status: statusSelectorAPI.getSelectedIds(),
                Name: $scope.scopeModel.description
            };

            if (typeApi.getSelectedIds() != undefined) {
                filter.Type = typeApi.getSelectedIds();
                filter.CarrierAccountIds = typeApi.getSelectedIds() == WhS_Deal_VolumeCommitmentTypeEnum.Buy.value ? supplierApi.getSelectedIds() : customerApi.getSelectedIds();
            }
            return filter;
        }
    }

    app.controller("WhS_Deal_VolumeCommitmentManagementController", volumeCommitmentManagementController);
})(app);