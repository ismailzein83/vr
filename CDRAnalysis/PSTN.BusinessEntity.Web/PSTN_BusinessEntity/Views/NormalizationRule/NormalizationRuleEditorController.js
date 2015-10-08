(function (appControllers) {
    
    "use strict";

    NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "SwitchTrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, SwitchTrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

        var editMode;
        var normalizationRuleId;

        var normalizationRuleActionSettingsDirectiveAPI;
        var normalizationRuleActionSettings;
        var normalizationRuleActionSettingsDataObject;

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

            $scope.onNormalizationRuleActionSettingsTemplateChanged = function () {
                $scope.isAddButtonDisabled = ($scope.selectedNormalizationRuleActionSettingsTemplate == undefined);
            }

            $scope.onNormalizationRuleActionSettingsDirectiveLoaded = function (api) {
                normalizationRuleActionSettingsDirectiveAPI = api;

                if (normalizationRuleActionSettings != undefined) {
                    normalizationRuleActionSettingsDirectiveAPI.setData(normalizationRuleActionSettings);
                    normalizationRuleActionSettings = undefined;
                }
            }

            $scope.addNormalizationRuleActionSettingsObjToList = function () {

                var normalizationRuleActionSettingsObj = {
                    ObjectId: $scope.normalizationRuleActionSettingsList.length + 1,
                    Editor: $scope.selectedNormalizationRuleActionSettingsTemplate.Editor,
                    Data: normalizationRuleActionSettingsDirectiveAPI.getData()
                };
                normalizationRuleActionSettingsObj.onListItemDirectiveLoaded = function (api) {
                        var data = normalizationRuleActionSettingsDirectiveAPI.getData();
                        api.setData(data);     
                }
                $scope.normalizationRuleActionSettingsList.push(normalizationRuleActionSettingsObj);
            }

            $scope.removeNormalizationRuleActionSettingsObjFromList = function ($event, normalizationRuleActionSettingsObj) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.normalizationRuleActionSettingsList, normalizationRuleActionSettingsObj.ActionId, 'ObjectId');
                $scope.normalizationRuleActionSettingsList.splice(index, 1);
            };

            //$scope.onListItemDirectiveLoaded = function (api) {

            //    if (normalizationRuleActionSettingsDirectiveAPI != undefined)
            //        var data = normalizationRuleActionSettingsDirectiveAPI.getData();
                
            //    api.setData(data);
            //}

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

            UtilsService.waitMultipleAsyncOperations([loadSwitches, loadTrunks, loadTemplates])
                .then(function () {
                    if (editMode) {
                        NormalizationRuleAPIService.GetNormalizationRuleByID(normalizationRuleId)
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
            return SwitchTrunkAPIService.GetSwitchTrunks()
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

        function fillScopeFromNormalizationRuleObj(normalizationRuleObj) {
            $scope.selectedSwitches = (normalizationRuleObj.Criteria.SwitchIds != null) ?
                getItemsByPropValues($scope.switches, normalizationRuleObj.Criteria.SwitchIds, "ID") : [];

            $scope.selectedTrunks = (normalizationRuleObj.Criteria.TrunkIds != null) ?
                getItemsByPropValues($scope.trunks, normalizationRuleObj.Criteria.TrunkIds, "ID") : [];

            $scope.selectedPhoneNumberType = (normalizationRuleObj.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, normalizationRuleObj.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = normalizationRuleObj.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = normalizationRuleObj.Criteria.PhoneNumberPrefix;

            addFetchedNormalizationRuleActionSettingsToList(normalizationRuleObj.Settings.Actions);
        }

        function buildNormalizationRuleObjFromScope() {

            var normalizationRuleObj = {
                NormalizationRuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
                Criteria: {
                    SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "ID"),
                    TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "ID"),
                    PhoneNumberType: ($scope.selectedPhoneNumberType != undefined) ? $scope.selectedPhoneNumberType.value : null,
                    PhoneNumberLength: $scope.phoneNumberLength,
                    PhoneNumberPrefix: $scope.phoneNumberPrefix
                },
                Settings: {
                    Actions: ($scope.normalizationRuleActionSettingsList.length > 0) ? UtilsService.getPropValuesFromArray($scope.normalizationRuleActionSettingsList, "Data") : null
                }
            };

            return normalizationRuleObj;
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

        function addFetchedNormalizationRuleActionSettingsToList(array) {
            if (array == undefined || array == null) return;

            for (var i = 0; i < array.length; i++) {
                var normalizationRuleActionSettingsObj = {
                    ObjectId: i + 1,
                    Editor: UtilsService.getItemByVal($scope.normalizationRuleActionSettingsTemplates, array[i].BehaviorId, "TemplateConfigID").Editor,
                    Data: array[i]
                };

                addAPIObj(normalizationRuleActionSettingsObj);
            }
        }

        function addAPIObj(obj) {
            obj.onListItemDirectiveLoaded = function (api) {
                api.setData(obj.Data);
            }
            $scope.normalizationRuleActionSettingsList.push(obj);
        }
    }

    appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);

})(appControllers);
