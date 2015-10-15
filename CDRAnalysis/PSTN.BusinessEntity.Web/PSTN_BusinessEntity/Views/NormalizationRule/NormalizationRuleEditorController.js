(function (appControllers) {
    
    "use strict";

    NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "TrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, TrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

        var editMode;
        var normalizationRuleId;
        var normalizationRuleSettingsDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                normalizationRuleId = parameters.NormalizationRuleId;
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

            $scope.normalizationRuleActionSettingsTemplates = [];
            $scope.selectedNormalizationRuleActionSettingsTemplate = undefined;
            $scope.isAddButtonDisabled = true;

            $scope.normalizationRuleActionSettingsList = [];

            $scope.beginEffectiveDate = Date.now();
            $scope.endEffectiveDate = undefined;

            /* Settings */

            $scope.normalizationRuleSettingsTemplates = [];
            $scope.selectedNormalizationRuleSettingsTemplate = undefined;

            $scope.onNormalizationRuleSettingsDirectiveAPILoaded = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;
            }

            /* Settings */

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

            $scope.onNormalizationRuleActionSettingsTemplateChanged = function () {
                $scope.isAddButtonDisabled = ($scope.selectedNormalizationRuleActionSettingsTemplate == undefined);
            }

            $scope.addNormalizationRuleActionSettingsListItem = function () {
                var listItem = getNormalizationRuleActionSettingsListItem(null);
                $scope.normalizationRuleActionSettingsList.push(listItem);
            }

            $scope.removeNormalizationRuleActionSettingsListItem = function ($event, normalizationRuleActionSettingsListItem) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.normalizationRuleActionSettingsList, normalizationRuleActionSettingsListItem.ListItemId, 'ListItemId');
                $scope.normalizationRuleActionSettingsList.splice(index, 1);
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

            UtilsService.waitMultipleAsyncOperations([loadSwitches, loadTrunks, loadTemplates, loadNormalizationRuleSettingsTemplates])
                .then(function () {
                    if (editMode) {
                        NormalizationRuleAPIService.GetNormalizationRuleById(normalizationRuleId)
                            .then(function (responseObject) {
                                fillScopeFromNormalizationRuleObj(responseObject);
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

        function loadTemplates() {

            return NormalizationRuleAPIService.GetNormalizationRuleActionBehaviorTemplates()
                .then(function (responseArray) {
                    angular.forEach(responseArray, function (item) {
                        $scope.normalizationRuleActionSettingsTemplates.push(item);
                    });
                });
        }

        function loadNormalizationRuleSettingsTemplates() {

            return NormalizationRuleAPIService.GetNormalizationRuleSettingsTemplates()
                .then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.normalizationRuleSettingsTemplates.push(item);
                    });
                });
        }

        function fillScopeFromNormalizationRuleObj(normalizationRuleObj) {
            $scope.description = normalizationRuleObj.Description;

            $scope.selectedSwitches = (normalizationRuleObj.Criteria.SwitchIds != null) ?
                getItemsByPropValues($scope.switches, normalizationRuleObj.Criteria.SwitchIds, "SwitchId") : [];

            $scope.selectedTrunks = (normalizationRuleObj.Criteria.TrunkIds != null) ?
                getItemsByPropValues($scope.trunks, normalizationRuleObj.Criteria.TrunkIds, "TrunkId") : [];

            $scope.selectedPhoneNumberType = (normalizationRuleObj.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, normalizationRuleObj.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = normalizationRuleObj.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = normalizationRuleObj.Criteria.PhoneNumberPrefix;

            addRetrievedNormalizationRuleActionSettingsToList(normalizationRuleObj.Settings.Actions);

            $scope.beginEffectiveDate = normalizationRuleObj.BeginEffectiveDate;
            $scope.endEffectiveDate = normalizationRuleObj.EndEffectiveDate;
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

        function addRetrievedNormalizationRuleActionSettingsToList(array) {
            if (array == undefined || array == null) return;

            angular.forEach(array, function (item) {
                var listItem = getNormalizationRuleActionSettingsListItem(item);
                $scope.normalizationRuleActionSettingsList.push(listItem);
            });
        }

        function getNormalizationRuleActionSettingsListItem(normalizationRuleActionSettingsObj) {

            var item = {
                ListItemId: $scope.normalizationRuleActionSettingsList.length + 1,
                BehaviorId: (normalizationRuleActionSettingsObj != null) ?
                    normalizationRuleActionSettingsObj.BehaviorId : $scope.selectedNormalizationRuleActionSettingsTemplate.TemplateConfigID,
                Editor: (normalizationRuleActionSettingsObj != null) ?
                    UtilsService.getItemByVal($scope.normalizationRuleActionSettingsTemplates, normalizationRuleActionSettingsObj.BehaviorId, "TemplateConfigID").Editor :
                    $scope.selectedNormalizationRuleActionSettingsTemplate.Editor,
                Data: (normalizationRuleActionSettingsObj != null) ? normalizationRuleActionSettingsObj : {}
            };

            item.onNormalizationRuleActionSettingsDirectiveAPILoaded = function (api) {
                item.normalizationRuleActionSettingsDirectiveAPI = api;
                item.normalizationRuleActionSettingsDirectiveAPI.setData(item.Data);

                item.onNormalizationRuleActionSettingsDirectiveAPILoaded = undefined;
                item.Data = undefined;
            }

            return item;
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

            var normalizationRuleObj = {
                NormalizationRuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
                Criteria: {
                    SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
                    TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "TrunkId"),
                    PhoneNumberType: ($scope.selectedPhoneNumberType != undefined) ? $scope.selectedPhoneNumberType.value : null,
                    PhoneNumberLength: $scope.phoneNumberLength,
                    PhoneNumberPrefix: $scope.phoneNumberPrefix
                },
                Settings: {
                    Actions: ($scope.normalizationRuleActionSettingsList.length > 0) ? getNormalizationRuleActionSettings() : null
                },
                Description: $scope.description,
                BeginEffectiveDate: $scope.beginEffectiveDate,
                EndEffectiveDate: $scope.endEffectiveDate
            };

            return normalizationRuleObj;
        }

        function getNormalizationRuleActionSettings() {
            var normalizationRulActionSettings = [];

            angular.forEach($scope.normalizationRuleActionSettingsList, function (item) {
                normalizationRulActionSettings.push(item.normalizationRuleActionSettingsDirectiveAPI.getData());
            });

            return normalizationRulActionSettings;
        }
    }

    appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);

})(appControllers);
