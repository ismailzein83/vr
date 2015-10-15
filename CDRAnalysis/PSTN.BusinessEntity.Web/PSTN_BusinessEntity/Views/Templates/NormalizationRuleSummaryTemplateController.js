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

            $scope.showCriteria = ($scope.showSwitches != undefined || $scope.showTrunks != undefined || $scope.showPhoneNumberType != undefined || $scope.showPhoneNumberLength != undefined || $scope.showPhoneNumberPrefix != undefined);

            $scope.showActions = $scope.dataItem.ActionDescriptions != undefined;
        }

        function load() { }

    }

    appControllers.controller("PSTN_BE_NormalizationRuleSummaryTemplateController", NormalizationRuleSummaryTemplateController);

})(appControllers);