(function (appControllers) {
    
    "use strict";

    NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

        var editMode;
        var ruleId;
        var ruleTypeDirectiveAPI;
        var directiveLoadCount = 0;
        var ruleTypeDirectiveData;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                ruleId = parameters.RuleId;
            }

            editMode = (ruleId != undefined);
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

            $scope.ruleTypes = UtilsService.getArrayEnum(PSTN_BE_NormalizationRuleTypeEnum);
            $scope.selectedRuleType = undefined;

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

            $scope.onRuleTypeDirectiveLoaded = function (api) {
                directiveLoadCount++;

                ruleTypeDirectiveAPI = api;

                if (editMode && directiveLoadCount == 1)
                    ruleTypeDirectiveAPI.setData(ruleTypeDirectiveData);
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
                        NormalizationRuleAPIService.GetNormalizationRuleById(ruleId)
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

            $scope.description = rule.Entity.Description;

            $scope.selectedSwitches = (rule.Entity.Criteria.SwitchIds != null) ?
                getItemsByPropValues($scope.switches, rule.Entity.Criteria.SwitchIds, "SwitchId") : [];

            $scope.selectedTrunks = (rule.Entity.Criteria.TrunkIds != null) ?
                getItemsByPropValues($scope.trunks, rule.Entity.Criteria.TrunkIds, "TrunkId") : [];

            $scope.selectedPhoneNumberType = (rule.Entity.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, rule.Entity.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = rule.Entity.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = rule.Entity.Criteria.PhoneNumberPrefix;

            ruleTypeDirectiveData = rule.Entity.Settings;
            $scope.selectedRuleType = UtilsService.getItemByVal($scope.ruleTypes, rule.Entity.Settings.RuleType, "value");

            $scope.beginEffectiveTime = rule.Entity.BeginEffectiveTime;
            $scope.endEffectiveTime = rule.Entity.EndEffectiveTime;
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

            var rule = {
                RuleId: (ruleId != undefined) ? ruleId : null,
                Criteria: {
                    SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
                    TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "TrunkId"),
                    PhoneNumberType: ($scope.selectedPhoneNumberType != undefined) ? $scope.selectedPhoneNumberType.value : null,
                    PhoneNumberLength: $scope.phoneNumberLength,
                    PhoneNumberPrefix: $scope.phoneNumberPrefix
                },
                Settings: ruleTypeDirectiveAPI.getData(),
                Description: $scope.description,
                BeginEffectiveTime: $scope.beginEffectiveTime,
                EndEffectiveTime: $scope.endEffectiveTime
            };

            rule.Settings.RuleType = $scope.selectedRuleType.value;

            return rule;
        }
    }

    appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);

})(appControllers);
