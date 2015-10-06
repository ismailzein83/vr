NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "SwitchAPIService", "SwitchTrunkAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, SwitchAPIService, SwitchTrunkAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var normalizationRuleId;
    var editMode;

    var normalizationRuleActionSettingsDirectiveAPI;
    var normalizationRuleActionSettings;

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

        $scope.numberTypes = UtilsService.getArrayEnum(PSTN_BE_PhoneNumberTypeEnum);
        $scope.selectedNumberTypes = [];

        $scope.numberLength = undefined;
        $scope.numberPrefix = undefined;

        $scope.templates = [];
        $scope.selectedTemplate = undefined;

        $scope.addedActions = [];

        $scope.onNormalizationRuleActionSettingsDirectiveLoaded = function (api) {
            normalizationRuleActionSettingsDirectiveAPI = api;

            if (normalizationRuleActionSettings != undefined) {
                normalizationRuleActionSettingsDirectiveAPI.setData(normalizationRuleActionSettings);
                normalizationRuleActionSettings = undefined;
            }
        }

        $scope.addAction = function () {
            var action = {
                ActionId: $scope.addedActions.length + 1,
                Editor: $scope.selectedTemplate.Editor,
                Data: normalizationRuleActionSettingsDirectiveAPI.getData()
            };

            $scope.addedActions.push(action);
        }

        $scope.removeAction = function ($event, action) {
            $event.preventDefault();
            $event.stopPropagation();
            
            var index = UtilsService.getItemIndexByVal($scope.addedActions, action.ActionId, 'ActionId');
            $scope.addedActions.splice(index, 1);
        };

        $scope.onDirectiveLoaded = function (api) {
            var data = normalizationRuleActionSettingsDirectiveAPI.getData();
            api.setData(data);
        }

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
                // code goes here
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
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
                    $scope.templates.push(item);
                });
            });
    }

    function fillScopeFromNormalizationRuleObj(trunkObj) {
        $scope.name = trunkObj.Name;
        $scope.symbol = trunkObj.Symbol;
        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, trunkObj.SwitchID, "ID");
        $scope.selectedType = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObj.Type);
        $scope.selectedDirection = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObj.Direction);
        currentlyLinkedToTrunkID = trunkObj.LinkedToTrunkID;
    }

    function buildNormalizationRuleObjFromScope() {

        var normalizationRuleObj = {
            NormalizationRuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
            Criteria: {
                SwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "ID"),
                TrunkIds: UtilsService.getPropValuesFromArray($scope.selectedTrunks, "ID"),
                PhoneNumberType: ($scope.selectedNumberTypes.length == 1) ? $scope.selectedNumberTypes[0].value : null,
                PhoneNumberLength: $scope.numberLength,
                PhoneNumberPrefix: $scope.numberPrefix
            },
            Settings: {
                Actions: ($scope.actions.length > 0) ? $scope.actions : null
            }
        };

        return normalizationRuleObj;
    }

    function updateNormalizationRule() {
        var normalizationRuleObj = buildNormalizationRuleObjFromScope();

        return NormalizationRuleAPIService.UpdateNormalizationRule(normalizationRuleObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Normalization Rule", response, "Name")) {
                    if ($scope.onNormalizationRuleUpdated != undefined)
                        $scope.onNormalizationRuleUpdated(response.UpdatedObj);

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
            .then(function (response) {

                if (VRNotificationService.notifyOnItemAdded("Normalization Rule", response, "Name")) {
                    if ($scope.onNormalizationRuleAdded != undefined)
                        $scope.onNormalizationRuleAdded(response.InsertedObj);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);
