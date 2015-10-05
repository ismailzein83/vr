NormalizationRuleManagementController.$inject = ["$scope"];

function NormalizationRuleManagementController($scope) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.onNormalizationRuleGridReady = function (api) {
            gridAPI = api;
            retrieveData();
        };

        $scope.searchClicked = function () {
            retrieveData();
        };

    }

    function load() {

    }

    function retrieveData() {
        if (gridAPI != undefined) {
            var query = getFilterObject();
            return gridAPI.retrieveData(query);
        }
    }

    function getFilterObject() {
        return {
            BeginEffectiveDate: $scope.beginEffectiveDate,
            EndEffectiveDate: $scope.endEffectiveDate
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleManagementController", NormalizationRuleManagementController);
