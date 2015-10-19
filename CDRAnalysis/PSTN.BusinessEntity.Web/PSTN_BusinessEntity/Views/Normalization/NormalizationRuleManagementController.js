NormalizationRuleManagementController.$inject = ["$scope", "PSTN_BE_Service", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "ValuesAPIService", "VRNotificationService"];

function NormalizationRuleManagementController($scope, PSTN_BE_Service, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, ValuesAPIService, VRNotificationService) {

    var gridAPI;

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

        $scope.ruleTypes = UtilsService.getArrayEnum(PSTN_BE_NormalizationRuleTypeEnum);
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

        defineAddButtonMenuActions();
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
            filterObj.RuleTypes = UtilsService.getPropValuesFromArray($scope.selectedRuleTypes, "value");
            filterObj.Description = $scope.description;
        }

        return filterObj;
    }

    function defineAddButtonMenuActions() {
        $scope.addButtonMenuActions = [
            {
                name: "Normalize Number Rule",
                clicked: function () {
                    addNormalizationRule(PSTN_BE_NormalizationRuleTypeEnum.AdjustNumber);
                }
            },
            {
                name: "Set Area Rule",
                clicked: function () {
                    addNormalizationRule(PSTN_BE_NormalizationRuleTypeEnum.SetArea);
                }
            }
        ];
    }

    function addNormalizationRule(ruleType) {

        var onNormalizationRuleAdded = function (normalizationRuleDetail) {
            gridAPI.onNormalizationRuleAdded(normalizationRuleDetail);
        };

        PSTN_BE_Service.addNormalizationRule(ruleType, onNormalizationRuleAdded);
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleManagementController", NormalizationRuleManagementController);
