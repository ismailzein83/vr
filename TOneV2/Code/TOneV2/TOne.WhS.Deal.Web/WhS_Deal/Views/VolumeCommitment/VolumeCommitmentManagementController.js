(function (appControllers) {

    "use strict";

    volumeCommitmentManagementController.$inject = ["$scope", "WhS_Deal_VolumeCommitmentService", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_Deal_VolumeCommitmentAPIService"];

    function volumeCommitmentManagementController($scope, WhS_Deal_VolumeCommitmentService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Deal_VolumeCommitmentAPIService) {
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

            function getFilterObject() {
                var query = {
                };
                return query;
            }
        }

        function load() {
        }

        function AddVolumeCommitment() {
            var onVolumeCommitmentAdded = function (addedItem) {
                gridAPI.onVolumeCommitmentAdded(addedItem);
            };

            WhS_Deal_VolumeCommitmentService.addVolumeCommitment(onVolumeCommitmentAdded);
        }
    }

    appControllers.controller("WhS_Deal_VolumeCommitmentManagementController", volumeCommitmentManagementController);
})(appControllers);