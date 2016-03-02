﻿NormalizationRuleManagementController.$inject = ["$scope", "PSTN_BE_Service", "CDRAnalysis_PSTN_SwitchAPIService", "CDRAnalysis_PSTN_TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "ValuesAPIService", "VRNotificationService", "NormalizationRuleAPIService"];

function NormalizationRuleManagementController($scope, PSTN_BE_Service, CDRAnalysis_PSTN_SwitchAPIService, CDRAnalysis_PSTN_TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, ValuesAPIService, VRNotificationService, NormalizationRuleAPIService) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.hasAddRulePermission = function () {
            return NormalizationRuleAPIService.HasAddRulePermission();
        };

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
        return CDRAnalysis_PSTN_SwitchAPIService.GetSwitches()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.switches.push(item);
                });
            });
    }

    function loadTrunks() {
        return CDRAnalysis_PSTN_TrunkAPIService.GetTrunks()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.trunks.push(item);
                });
            });
    }

    function getFilterObj() {

        var filterObj = {
            PhoneNumberTypes: UtilsService.getPropValuesFromArray($scope.selectedPhoneNumberTypes, "value"),
            EffectiveDate: $scope.effectiveDate,
            RuleTypes: UtilsService.getPropValuesFromArray($scope.selectedRuleTypes, "value")
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

    function defineAddButtonMenuActions() {
        $scope.addButtonMenuActions = [
            {
                name: "Adjust Number",
                clicked: function () {
                    addNormalizationRule(PSTN_BE_NormalizationRuleTypeEnum.AdjustNumber.value);
                }
            },
            {
                name: "Set Area",
                clicked: function () {
                    addNormalizationRule(PSTN_BE_NormalizationRuleTypeEnum.SetArea.value);
                }
            }
        ];
    }

    function addNormalizationRule(ruleTypeValue) {

        var onNormalizationRuleAdded = function (normalizationRuleDetail) {
            gridAPI.onNormalizationRuleAdded(normalizationRuleDetail);
        };

        PSTN_BE_Service.addNormalizationRule(ruleTypeValue, onNormalizationRuleAdded);
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleManagementController", NormalizationRuleManagementController);
