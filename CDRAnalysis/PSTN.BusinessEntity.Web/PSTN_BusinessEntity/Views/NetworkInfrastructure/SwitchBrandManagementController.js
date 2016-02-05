(function (appControllers) {

    "use strict";
    SwitchBrandManagementController.$inject = ["$scope", "CDRAnalysis_PSTN_SwitchBrandService"];

    function SwitchBrandManagementController($scope, CDRAnalysis_PSTN_SwitchBrandService ) {

    var brandGridAPI;
    var filter = {};
    defineScope();
    load();

    function defineScope() {

        $scope.searchClicked = function () {
            setFilterObject();
            return brandGridAPI.retrieveData(filter);
        }

        $scope.addBrand = function () {
            var onSwitchBrandAdded = function (onSwitchBrandAdded) {
                    brandGridAPI.onSwitchBrandAdded(onSwitchBrandAdded);
            }
            CDRAnalysis_PSTN_SwitchBrandService.addSwitchBrand(onSwitchBrandAdded);
        }

        $scope.onGridReady = function (api) {
            brandGridAPI = api;
            brandGridAPI.retrieveData({});
        }

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