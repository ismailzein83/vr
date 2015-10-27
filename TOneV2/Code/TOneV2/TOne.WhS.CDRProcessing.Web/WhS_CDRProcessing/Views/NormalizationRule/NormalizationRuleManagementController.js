normalizationRuleManagementController.$inject = ["$scope", "WhS_CDRProcessing_MainService", "WhS_CDRProcessing_PhoneNumberTypeEnum", "UtilsService", "VRNotificationService"];

function normalizationRuleManagementController($scope, WhS_CDRProcessing_MainService, WhS_CDRProcessing_PhoneNumberTypeEnum, UtilsService, VRNotificationService) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.phoneNumberTypes = UtilsService.getArrayEnum(WhS_CDRProcessing_PhoneNumberTypeEnum);
        $scope.selectedPhoneNumberTypes = [];

        $scope.effectiveDate = undefined;

        $scope.phoneNumberPrefix = undefined;
        $scope.phoneNumberLength = undefined;

        $scope.switches = [];
        $scope.selectedSwitches = [];

        $scope.trunks = [];
        $scope.selectedTrunks = [];
        $scope.selectedRuleTypes = [];

        $scope.description = undefined;

        $scope.onDirectiveGridReady = function (api) {
            gridAPI = api;
            gridAPI.retrieveData({});
        };

        $scope.onSearchClicked = function () {
            var query = getFilterObj();
            return gridAPI.retrieveData(query);
        };
        $scope.addNormalizationRule = addNormalizationRule;
    }

    function load() {
    }


    function getFilterObj() {

        var filterObj = {
            PhoneNumberTypes: UtilsService.getPropValuesFromArray($scope.selectedPhoneNumberTypes, "value"),
            EffectiveDate: $scope.effectiveDate,
        };

        if ($scope.selectedTabIndex == 1) {
            filterObj.PhoneNumberPrefix = $scope.phoneNumberPrefix;
            filterObj.PhoneNumberLength = $scope.phoneNumberLength;
            filterObj.Description = $scope.description;
        }

        return filterObj;
    }

    function addNormalizationRule() {

        var onNormalizationRuleAdded = function (normalizationRuleDetail) {
            gridAPI.onNormalizationRuleAdded(normalizationRuleDetail);
        };

        WhS_CDRProcessing_MainService.addNormalizationRule(onNormalizationRuleAdded);
    }
}

appControllers.controller("WhS_CDRProcessing_NormalizationRuleManagementController", normalizationRuleManagementController);
