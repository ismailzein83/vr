NormalizationRuleEditorController.$inject = ["$scope", "NormalizationRuleAPIService", "VRNavigationService", "VRNotificationService"];

function NormalizationRuleEditorController($scope, NormalizationRuleAPIService, VRNavigationService, VRNotificationService) {

    var normalizationRuleId;
    var editMode;

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

        $scope.templates = [];
        $scope.selectedBehaviorTemplate;

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

        return NormalizationRuleAPIService.GetNormalizationRuleActionBehaviorTemplates()
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.templates.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function fillScopeFromNormalizationRuleObject(trunkObject) {
        $scope.name = trunkObject.Name;
        $scope.symbol = trunkObject.Symbol;
        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, trunkObject.SwitchID, "ID");
        $scope.selectedType = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
        $scope.selectedDirection = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
        currentlyLinkedToTrunkID = trunkObject.LinkedToTrunkID;
    }

    function updateNormalizationRule() {
        var normalizationRuleObject = buildNormalizationRuleObjectFromScope();

        return NormalizationRuleAPIService.UpdateNormalizationRule(normalizationRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Normalization Rule", response, "Name")) {
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
        var normalizationRuleObject = buildNormalizationRuleObjectFromScope();

        return NormalizationRuleAPIService.AddNormalizationRule(normalizationRuleObject)
            .then(function (response) {

                if (VRNotificationService.notifyOnItemAdded("Normalization Rule", response, "Name")) {
                    if ($scope.onNormalizationRuleAdded != undefined)
                        $scope.onNormalizationRuleAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildNormalizationRuleObjectFromScope() {
        return {
            
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_NormalizationRuleEditorController", NormalizationRuleEditorController);
