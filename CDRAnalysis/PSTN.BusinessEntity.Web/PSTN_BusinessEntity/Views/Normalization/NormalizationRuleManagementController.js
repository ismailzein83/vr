NormalizationRuleManagementController.$inject = ["$scope", "PSTN_BE_Service", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService", "VRNotificationService"];

function NormalizationRuleManagementController($scope, PSTN_BE_Service, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService, VRNotificationService) {

    var directiveGridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.phoneNumberTypes = UtilsService.getArrayEnum(PSTN_BE_PhoneNumberTypeEnum);
        $scope.selectedPhoneNumberTypes = [];

        $scope.effectiveDate = undefined;

        $scope.phoneNumberPrefix = undefined;
        $scope.phoneNumberLength = undefined;

        $scope.switches = [];
        $scope.selectedSwitches = [];

        $scope.trunks = [];
        $scope.selectedTrunks = [];

        $scope.description = undefined;

        $scope.onDirectiveGridReady = function (api) {
            directiveGridAPI = api;
            directiveGridAPI.retrieveData({});
        };

        $scope.onSearchClicked = function () {
            var query = getFilterObj();
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
                VRNotificationService.notifyException(error, $scope);
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
        return TrunkAPIService.GetTrunks()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.trunks.push(item);
                });
            });
    }

    function getFilterObj() {

        var filterObj = {
            PhoneNumberTypes: UtilsService.getPropValuesFromArray($scope.selectedPhoneNumberTypes, "value"),
            EffectiveDate: $scope.effectiveDate
        };

        if ($scope.selectedTabIndex == 1) {
            filterObj.PhoneNumberPrefix = $scope.phoneNumberPrefix;
            filterObj.PhoneNumberLength = $scope.phoneNumberLength;
            filterObj.SwitchIds = UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId");
            filterObj.TrunkIds = UtilsService.getPropValuesFromArray($scope.selectedTrunks, "TrunkId");
            filterObj.Description = $scope.description;
        }

        return filterObj;
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleManagementController", NormalizationRuleManagementController);
