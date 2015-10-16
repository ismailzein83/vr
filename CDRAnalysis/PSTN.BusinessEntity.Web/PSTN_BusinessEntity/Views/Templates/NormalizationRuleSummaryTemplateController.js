(function (appControllers) {

    "use strict";

    NormalizationRuleSummaryTemplateController.$inject = ["$scope", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService"];

    function NormalizationRuleSummaryTemplateController($scope, PSTN_BE_PhoneNumberTypeEnum, UtilsService) {

        defineScope();
        load();

        function defineScope() {

            $scope.showSwitches = ($scope.dataItem.SwitchNames != undefined);
            $scope.showTrunks = ($scope.dataItem.TrunkNames != undefined);

            if ($scope.dataItem.Entity.Criteria.PhoneNumberType != undefined) {
                $scope.showPhoneNumberType = true;

                var phoneNumberType = UtilsService.getEnum(PSTN_BE_PhoneNumberTypeEnum, "value", $scope.dataItem.Entity.Criteria.PhoneNumberType);
                $scope.dataItem.Entity.Criteria.PhoneNumberTypeDescription = phoneNumberType.description;
            }

            $scope.showPhoneNumberLength = ($scope.dataItem.Entity.Criteria.PhoneNumberLength != undefined);
            $scope.showPhoneNumberPrefix = ($scope.dataItem.Entity.Criteria.PhoneNumberPrefix != undefined);

            $scope.showSettings = $scope.dataItem.Descriptions != undefined;
        }

        function load() { }

    }

    appControllers.controller("PSTN_BE_NormalizationRuleSummaryTemplateController", NormalizationRuleSummaryTemplateController);

})(appControllers);