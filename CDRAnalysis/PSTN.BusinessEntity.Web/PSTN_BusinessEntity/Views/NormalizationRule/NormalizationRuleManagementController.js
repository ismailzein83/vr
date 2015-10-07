NormalizationRuleManagementController.$inject = ["$scope", "PSTN_BE_Service"];

function NormalizationRuleManagementController($scope, PSTN_BE_Service) {

    var directiveGridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.onDirectiveGridReady = function (api) {
            directiveGridAPI = api;
            retrieveData();
        };

        $scope.onSearchClicked = function () {
            retrieveData();
        };

        $scope.addNormalizationRule = function () {

            var onNormalizationRuleAdded = function (normalizationRuleObj) {
                directiveGridAPI.onNormalizationRuleAdded(normalizationRuleObj);
            };

            PSTN_BE_Service.addNormalizationRule(onNormalizationRuleAdded);
        };

    }

    function load() {

    }

    function retrieveData() {
        if (directiveGridAPI != undefined) {
            var query = getFilterObject();
            return directiveGridAPI.retrieveData(query);
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
