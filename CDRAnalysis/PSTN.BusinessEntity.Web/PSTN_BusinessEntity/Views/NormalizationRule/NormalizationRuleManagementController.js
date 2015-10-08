NormalizationRuleManagementController.$inject = ["$scope", "PSTN_BE_Service", "SwitchAPIService", "SwitchTrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService"];

function NormalizationRuleManagementController($scope, PSTN_BE_Service, SwitchAPIService, SwitchTrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService) {

    var directiveGridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.switches = [];
        $scope.selectedSwitches = [];

        $scope.trunks = [];
        $scope.selectedTrunks = [];

        $scope.phoneNumberTypes = UtilsService.getArrayEnum(PSTN_BE_PhoneNumberTypeEnum);
        $scope.selectedPhoneNumberTypes = [];

        $scope.phoneNumberLength = undefined;
        $scope.phoneNumberPrefix = undefined;

        $scope.onDirectiveGridReady = function (api) {
            directiveGridAPI = api;
            directiveGridAPI.retrieveData({});
        };

        $scope.onSearchClicked = function () {
            var query = getFilterObject();
            return directiveGridAPI.retrieveData(query);
        };

        $scope.addNormalizationRule = function () {

            var onNormalizationRuleAdded = function (normalizationRuleObj) {
                directiveGridAPI.onNormalizationRuleAdded(normalizationRuleObj);
            };

            PSTN_BE_Service.addNormalizationRule(onNormalizationRuleAdded);
        };

    }

    function load() {
        $scope.isLoadingFilters = true;

        UtilsService.waitMultipleAsyncOperations([loadSwitches, loadTrunks])
            .catch(function (error) {
                VRNotficationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function loadSwitches() {
        return SwitchAPIService.GetSwitches()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.switches.push(item);
                });
            });
    }

    function loadTrunks() {
        return SwitchTrunkAPIService.GetSwitchTrunks()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.trunks.push(item);
                });
            });
    }

    function getFilterObject() {
        return {
            BeginEffectiveDate: $scope.beginEffectiveDate,
            EndEffectiveDate: $scope.endEffectiveDate,
            SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "ID"),
            TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "ID"),
            PhoneNumberType: ($scope.selectedPhoneNumberTypes.length == 1) ? $scope.selectedPhoneNumberTypes[0].value : null,
            PhoneNumberLength: $scope.phoneNumberLength,
            PhoneNumberPrefix: $scope.phoneNumberPrefix
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleManagementController", NormalizationRuleManagementController);
