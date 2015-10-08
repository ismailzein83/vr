(function (appControllers) {

    "use strict";

    NormalizationRuleDescriptionTemplateController.$inject = ["$scope"];

    function NormalizationRuleDescriptionTemplateController($scope) {

        defineScope();
        load();

        function defineScope() {
            $scope.showSwitches = ($scope.dataItem.SwitchNames != undefined);
            $scope.showTrunks = ($scope.dataItem.TrunkNames != undefined);
            $scope.showPhoneNumberType = ($scope.dataItem.PhoneNumberType != undefined);
            $scope.showPhoneNumberLength = ($scope.dataItem.PhoneNumberLength != undefined);
            $scope.showPhoneNumberPrefix = ($scope.dataItem.PhoneNumberPrefix != undefined && $scope.dataItem.PhoneNumberPrefix != ""); // to be checked
        }

        function load() { }

    }

    appControllers.controller("PSTN_BE_NormalizationRuleDescriptionTemplateController", NormalizationRuleDescriptionTemplateController);

})(appControllers);