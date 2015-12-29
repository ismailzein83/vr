(function (appControllers) {

    "use strict";

    NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "PSTN_BE_NormalizationRuleTypeEnum", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, PSTN_BE_NormalizationRuleTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var editMode;

        var normalizationRuleId;
        var normalizationRuleType;

        var normalizationRuleSettingsDirectiveAPI;
        var normalizationRuleSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var normalizationRuleEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                normalizationRuleId = parameters.NormalizationRuleId;
                normalizationRuleType = UtilsService.getEnum(PSTN_BE_NormalizationRuleTypeEnum, "value", parameters.NormalizationRuleTypeValue);
            }

            editMode = (normalizationRuleId != undefined);
        }

        function defineScope() {

            $scope.onNormalizationRuleSettingsDirectiveReady = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;
                normalizationRuleSettingsReadyPromiseDeferred.resolve();
            }

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.trunks = [];
            $scope.selectedTrunks = [];

            $scope.phoneNumberTypes = UtilsService.getArrayEnum(PSTN_BE_PhoneNumberTypeEnum);
            $scope.selectedPhoneNumberType = undefined;

            $scope.phoneNumberLength = undefined;
            $scope.phoneNumberPrefix = undefined;

            $scope.beginEffectiveDate = Date.now();
            $scope.endEffectiveDate = undefined;

            $scope.onSelectedSwitchesChanged = function () {

                if ($scope.selectedSwitches == undefined || $scope.selectedSwitches.length == 0)
                    return;

                var selectedSwitchIds = UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId");

                var tempSelectedTrunks = UtilsService.cloneObject($scope.selectedTrunks, true);

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

            $scope.validateDirectiveData = function () {
                if (normalizationRuleSettingsDirectiveAPI == undefined)
                    return false;

                var isValid = normalizationRuleSettingsDirectiveAPI.validateData();
                return isValid;
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
                        NormalizationRuleAPIService.GetRule(normalizationRuleId)
                            .then(function (response) {
                                normalizationRuleEntity = response;
                                fillScopeFromNormalizationRuleObj();
                                loadNormalizationRuleSettings();
                            })
                            .catch(function (error) {
                                $scope.isGettingData = false;
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            }).finally(function () {
                                $scope.isGettingData = false;
                            });
                    }
                    else {
                        setDefaultValues();
                        loadNormalizationRuleSettings();
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


        function loadNormalizationRuleSettings() {
            var normalizationRuleSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            normalizationRuleSettingsReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload;
                    if (normalizationRuleEntity != undefined)
                        directivePayload = normalizationRuleEntity.Settings;

                    VRUIUtilsService.callDirectiveLoad(normalizationRuleSettingsDirectiveAPI, directivePayload, normalizationRuleSettingsLoadPromiseDeferred);
                });
            return normalizationRuleSettingsLoadPromiseDeferred.promise;
        }

        function setDefaultValues() {
            $scope.title = UtilsService.buildTitleForAddEditor(normalizationRuleType.title + " Normalization Rule");
            $scope.normalizationRuleSettingsDirective = normalizationRuleType.directive;
        }

        function fillScopeFromNormalizationRuleObj() {

            $scope.description = normalizationRuleEntity.Description;

            $scope.selectedSwitches = (normalizationRuleEntity.Criteria.SwitchIds != null) ?
                getItemsByPropValues($scope.switches, normalizationRuleEntity.Criteria.SwitchIds, "SwitchId") : [];

            $scope.selectedTrunks = (normalizationRuleEntity.Criteria.TrunkIds != null) ?
                getItemsByPropValues($scope.trunks, normalizationRuleEntity.Criteria.TrunkIds, "TrunkId") : [];

            $scope.selectedPhoneNumberType = (normalizationRuleEntity.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, normalizationRuleEntity.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = normalizationRuleEntity.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = normalizationRuleEntity.Criteria.PhoneNumberPrefix;

            normalizationRuleType = UtilsService.getEnum(PSTN_BE_NormalizationRuleTypeEnum, "value", normalizationRuleEntity.Settings.RuleType);
            $scope.normalizationRuleSettingsDirective = normalizationRuleType.directive;
            $scope.title = UtilsService.buildTitleForUpdateEditor(normalizationRuleType.title + " Normalization Rule");

            $scope.beginEffectiveTime = normalizationRuleEntity.BeginEffectiveTime;
            $scope.endEffectiveTime = normalizationRuleEntity.EndEffectiveTime;
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

            return NormalizationRuleAPIService.UpdateRule(normalizationRuleObj)
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

            return NormalizationRuleAPIService.AddRule(normalizationRuleObj)
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
    }

    appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);

})(appControllers);
