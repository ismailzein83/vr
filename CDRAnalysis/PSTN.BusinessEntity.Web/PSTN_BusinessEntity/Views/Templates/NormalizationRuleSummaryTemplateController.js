(function (appControllers) {

    "use strict";

    NormalizationRuleSummaryTemplateController.$inject = ["$scope"];

    function NormalizationRuleSummaryTemplateController($scope) {

        defineScope();
        load();

        function defineScope() {

            $scope.showSwitches = ($scope.dataItem.SwitchNames != undefined);
            $scope.showTrunks = ($scope.dataItem.TrunkNames != undefined);
            $scope.showPhoneNumberType = ($scope.dataItem.PhoneNumberType != undefined);
            $scope.showPhoneNumberLength = ($scope.dataItem.PhoneNumberLength != undefined);
            $scope.showPhoneNumberPrefix = ($scope.dataItem.PhoneNumberPrefix != undefined);

            $scope.showSettings = $scope.dataItem.Descriptions != undefined;
        }

        function load() { }

    }

    appControllers.controller("PSTN_BE_NormalizationRuleSummaryTemplateController", NormalizationRuleSummaryTemplateController);

})(appControllers);