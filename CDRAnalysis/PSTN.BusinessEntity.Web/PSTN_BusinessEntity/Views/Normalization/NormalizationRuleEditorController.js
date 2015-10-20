(function (appControllers) {
    
    "use strict";

    NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

        var editMode;
        var normalizationRuleId;
        var normalizationRuleType;

        var normalizationRuleSettingsDirectiveAPI;
        var normalizationRuleSettingsDirectiveData;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                normalizationRuleId = parameters.NormalizationRuleId;
                normalizationRuleType = parameters.NormalizationRuleType;
            }

            editMode = (normalizationRuleId != undefined);
        }

        function defineScope() {
             
            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.trunks = [];
            $scope.selectedTrunks = [];

            $scope.phoneNumberTypes = UtilsService.getArrayEnum(PSTN_BE_PhoneNumberTypeEnum);
            $scope.selectedPhoneNumberType = undefined;

            $scope.phoneNumberLength = undefined;
            $scope.phoneNumberPrefix = undefined;

            if (!editMode) {
                showSelectedNormalizationRuleType();
            }

            $scope.beginEffectiveDate = Date.now();
            $scope.endEffectiveDate = undefined;

            $scope.onSelectedSwitchesChanged = function () {

                if ($scope.selectedSwitches == undefined || $scope.selectedSwitches.length == 0)
                    return;

                var selectedSwitchIds = UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId");

                var tempSelectedTrunks =   UtilsService.cloneObject(  $scope.selectedTrunks, true);

                angular.forEach(tempSelectedTrunks, function (trunk) {
                    if (!UtilsService.contains(selectedSwitchIds, trunk.SwitchId)) {
                        var index = UtilsService.getItemIndexByVal($scope.selectedTrunks, trunk.TrunkId, "TrunkId");
                        if (index > -1)
                            $scope.selectedTrunks.splice(index, 1);
                    }
                });

                tempSelectedTrunks.length = 0;

            }

            $scope.getSwitchRelatedTrunks = function (trunkNameFilter) {

                var trunkFilterObj = {
                    SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
                    TrunkNameFilter: trunkNameFilter
                };

                return TrunkAPIService.GetTrunksBySwitchIds(trunkFilterObj);
            }

            $scope.onNormalizationRuleSettingsDirectiveLoaded = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;

                if (editMode)
                    normalizationRuleSettingsDirectiveAPI.setData(normalizationRuleSettingsDirectiveData);
            };

            $scope.saveNormalizationRule = function () {
                if (editMode)
                    return updateNormalizationRule();
                else
                    return insertNormalizationRule();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.isGettingData = true;

            UtilsService.waitMultipleAsyncOperations([loadSwitches, loadTrunks])
                .then(function () {
                    if (editMode) {
                        NormalizationRuleAPIService.GetNormalizationRuleById(normalizationRuleId)
                            .then(function (response) {
                                fillScopeFromNormalizationRuleObj(response);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            })
                            .finally(function () {
                                $scope.isGettingData = false;
                            });
                    }
                    else {
                        $scope.isGettingData = false;
                    }
                })
                .catch(function (error) {
                    $scope.isGettingData = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
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

        function fillScopeFromNormalizationRuleObj(rule) {

            $scope.description = rule.Description;

            $scope.selectedSwitches = (rule.Criteria.SwitchIds != null) ?
                getItemsByPropValues($scope.switches, rule.Criteria.SwitchIds, "SwitchId") : [];

            $scope.selectedTrunks = (rule.Criteria.TrunkIds != null) ?
                getItemsByPropValues($scope.trunks, rule.Criteria.TrunkIds, "TrunkId") : [];

            $scope.selectedPhoneNumberType = (rule.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, rule.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = rule.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = rule.Criteria.PhoneNumberPrefix;

            normalizationRuleSettingsDirectiveData = rule.Settings;
            normalizationRuleType = UtilsService.getEnum(PSTN_BE_NormalizationRuleTypeEnum, "value", rule.Settings.RuleType);
            showSelectedNormalizationRuleType();

            $scope.beginEffectiveTime = rule.BeginEffectiveTime;
            $scope.endEffectiveTime = rule.EndEffectiveTime;
        }

        function getItemsByPropValues(array, values, propName) {
            if (array == undefined || array == null || values == undefined || values == null || propName == undefined || propName == null) return [];

            var matchingItems = [];

            for (var i = 0; i < array.length; i++) {
                var propValue = array[i][propName];

                if (UtilsService.contains(values, propValue))
                    matchingItems.push(array[i]);
            }

            return matchingItems;
        }

        function updateNormalizationRule() {
            var normalizationRuleObj = buildNormalizationRuleObjFromScope();

            return NormalizationRuleAPIService.UpdateNormalizationRule(normalizationRuleObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Normalization Rule", response)) {
                        if ($scope.onNormalizationRuleUpdated != undefined)
                            $scope.onNormalizationRuleUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        function insertNormalizationRule() {
            var normalizationRuleObj = buildNormalizationRuleObjFromScope();

            return NormalizationRuleAPIService.AddNormalizationRule(normalizationRuleObj)
                .then(function (responseObject) {

                    if (VRNotificationService.notifyOnItemAdded("Normalization Rule", responseObject)) {
                        if ($scope.onNormalizationRuleAdded != undefined)
                            $scope.onNormalizationRuleAdded(responseObject.InsertedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        function buildNormalizationRuleObjFromScope() {

            var normalizationRule = {
                RuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
                Criteria: {
                    SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
                    TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "TrunkId"),
                    PhoneNumberType: ($scope.selectedPhoneNumberType != undefined) ? $scope.selectedPhoneNumberType.value : null,
                    PhoneNumberLength: $scope.phoneNumberLength,
                    PhoneNumberPrefix: $scope.phoneNumberPrefix
                },
                Settings: normalizationRuleSettingsDirectiveAPI.getData(),
                Description: $scope.description,
                BeginEffectiveTime: $scope.beginEffectiveTime,
                EndEffectiveTime: $scope.endEffectiveTime
            };

            normalizationRule.Settings.RuleType = normalizationRuleType.value;

            return normalizationRule;
        }

        function showSelectedNormalizationRuleType() {
            $scope.isAdjustNumberRule = (normalizationRuleType.value == PSTN_BE_NormalizationRuleTypeEnum.AdjustNumber.value);
            $scope.isSetAreaRule = (normalizationRuleType.value == PSTN_BE_NormalizationRuleTypeEnum.SetArea.value);
        }
    }

    appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);

})(appControllers);
