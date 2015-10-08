(function (appControllers) {

    "use strict";

    NormalizationRuleDescriptionTemplateController.$inject = ["$scope"];

    function NormalizationRuleDescriptionTemplateController($scope) {

        defineScope();
        load();

        function defineScope() {
            $scope.dataItem.SwitchNames = "Switch, Switch, Switch, Switch, Switch";
            $scope.dataItem.TrunkNames = "Trunk, Trunk, Trunk, Trunk, Trunk";
            $scope.dataItem.ActionDescriptions = [
                "Add Prefix: 1",
                "Add Prefix: 2",
                "Add Prefix: 3",
                "Add Prefix: 4",
                "Add Prefix: 5"
            ];
        }

        function load() { }

    }

    appControllers.controller("PSTN_BE_NormalizationRuleDescriptionTemplateController", NormalizationRuleDescriptionTemplateController);

})(appControllers);