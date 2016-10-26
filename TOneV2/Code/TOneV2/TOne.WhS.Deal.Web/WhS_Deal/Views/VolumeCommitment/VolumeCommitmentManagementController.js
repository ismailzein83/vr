(function (app) {

    "use strict";

    volumeCommitmentManagementController.$inject = ["$scope", "WhS_Deal_VolumeCommitmentService", "UtilsService", "VRUIUtilsService", "VRNotificationService"];

    function volumeCommitmentManagementController($scope, WhS_Deal_VolumeCommitmentService, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                api.load({});
            }

            $scope.searchClicked = function () {
                return gridAPI.load(getFilterObject());
            };

            $scope.AddVolumeCommitment = AddVolumeCommitment;

            
        }

        function load() {
        }

        function AddVolumeCommitment() {
            var onVolumeCommitmentAdded = function (addedItem) {
                gridAPI.onVolumeCommitmentAdded(addedItem);
            };
            WhS_Deal_VolumeCommitmentService.addVolumeCommitment(onVolumeCommitmentAdded);
        }

        function getFilterObject() {
            var query = {
            };
            return query;
        }
    }

    app.controller("WhS_Deal_VolumeCommitmentManagementController", volumeCommitmentManagementController);
})(app);