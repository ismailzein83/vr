(function (appControllers) {

    "use strict";
    SwitchBrandManagementController.$inject = ["$scope", "CDRAnalysis_PSTN_SwitchBrandService", "CDRAnalysis_PSTN_SwitchBrandAPIService"];

    function SwitchBrandManagementController($scope, CDRAnalysis_PSTN_SwitchBrandService, CDRAnalysis_PSTN_SwitchBrandAPIService) {

        var brandGridAPI;
        var filter = {};
        defineScope();
        load();

        function defineScope() {
            $scope.hasAddSwitchBrandPermission = function () {
                return CDRAnalysis_PSTN_SwitchBrandAPIService.HasAddSwitchBrandPermission();
            };

            $scope.searchClicked = function () {
                setFilterObject();
                return brandGridAPI.retrieveData(filter);
            };

            $scope.addBrand = function () {
                var onSwitchBrandAdded = function (onSwitchBrandAdded) {
                    brandGridAPI.onSwitchBrandAdded(onSwitchBrandAdded);
                };
                CDRAnalysis_PSTN_SwitchBrandService.addSwitchBrand(onSwitchBrandAdded);
            };

            $scope.onGridReady = function (api) {
                brandGridAPI = api;
                brandGridAPI.retrieveData({});
            };


        }

        function load() {

        }


        function setFilterObject() {
            filter = {
                Name: $scope.name
            };
        }

    }

    appControllers.controller("PSTN_BusinessEntity_SwitchBrandManagementController", SwitchBrandManagementController);
})(appControllers);