(function (app) {

    "use strict";

    volumeCommitmentManagementController.$inject = ["$scope", "WhS_Deal_VolumeCommitmentService", "UtilsService", "VRUIUtilsService", "VRNotificationService","WhS_Deal_VolumeCommitmentTypeEnum"];

    function volumeCommitmentManagementController($scope, WhS_Deal_VolumeCommitmentService, UtilsService, VRUIUtilsService, VRNotificationService,WhS_Deal_VolumeCommitmentTypeEnum) {
        var gridAPI;
        var supplierApi;
        var customerApi;
        var typeApi;
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
            $scope.scopeModel.onSupplierDirectiveReady = function (api) {
                supplierApi = api;
                supplierApi.load();
            };
            $scope.scopeModel.onCustomerDirectiveReady = function (api) {
                customerApi = api;
                customerApi.load();
            };

            $scope.scopeModel.addVolumeCommitment = addVolumeCommitment;
        }

        function load() {

        }

        function addVolumeCommitment() {
            var onVolumeCommitmentAdded = function (addedItem) {
                gridAPI.onVolumeCommitmentAdded(addedItem);
            };
            WhS_Deal_VolumeCommitmentService.addVolumeCommitment(onVolumeCommitmentAdded);
        }

        function getFilterObject() {
            var filter = {
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