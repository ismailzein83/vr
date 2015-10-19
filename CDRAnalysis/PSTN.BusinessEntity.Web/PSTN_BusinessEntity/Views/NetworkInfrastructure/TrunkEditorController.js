TrunkEditorController.$inject = ["$scope", "TrunkAPIService", "SwitchAPIService", "TrunkTypeEnum", "TrunkDirectionEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function TrunkEditorController($scope, TrunkAPIService, SwitchAPIService, TrunkTypeEnum, TrunkDirectionEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var trunkId;
    var switchId;
    var editMode;
    var currentlyLinkedToTrunkHasBeenSet = false;
    var currentlyLinkedToTrunkId;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            trunkId = parameters.TrunkId;
            switchId = parameters.SwitchId;
        }

        editMode = (trunkId != undefined);
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.symbol = undefined;
        $scope.switches = [];
        $scope.selectedSwitch = undefined;
        $scope.types = UtilsService.getArrayEnum(TrunkTypeEnum);
        $scope.selectedType = undefined;
        $scope.directions = UtilsService.getArrayEnum(TrunkDirectionEnum);
        $scope.selectedDirection = undefined;

        $scope.switchesToLinkTo = [];
        $scope.selectedSwitchToLinkTo = undefined;
        $scope.trunksToLinkTo = [];
        $scope.selectedTrunkToLinkTo = undefined;

        $scope.disableSwitchMenu = false;

        $scope.onSwitchChanged = function () {

            $scope.switchesToLinkTo = [];
            $scope.selectedSwitchToLinkTo = undefined;

            angular.forEach($scope.switches, function (item) {
                $scope.switchesToLinkTo.push(item);
            });

            if ($scope.selectedSwitch != undefined) {
                var index = UtilsService.getItemIndexByVal($scope.switchesToLinkTo, $scope.selectedSwitch.SwitchId, "SwitchId");
                $scope.switchesToLinkTo.splice(index, 1);
            }
        }

        $scope.onSwitchToLinkToChanged = function () {

            $scope.trunksToLinkTo = [];
            $scope.selectedTrunkToLinkTo = undefined;

            if ($scope.selectedSwitchToLinkTo != undefined) {
                
                $scope.isGettingData = true;
                
                var trunkFilterObj = {
                    SwitchIds: [$scope.selectedSwitchToLinkTo.SwitchId],
                    TrunkNameFilter: null
                };

                TrunkAPIService.GetTrunksBySwitchIds(trunkFilterObj)
                    .then(function (response) {

                        angular.forEach(response, function (item) {
                            $scope.trunksToLinkTo.push(item);
                        });

                        // remove the trunk that's being edited from the list of trunks to link to
                        var index = UtilsService.getItemIndexByVal($scope.trunksToLinkTo, trunkId, "TrunkId");
                        if (index >= 0)
                            $scope.trunksToLinkTo.splice(index, 1);

                        if (!currentlyLinkedToTrunkHasBeenSet && currentlyLinkedToTrunkId != null) {
                            $scope.selectedTrunkToLinkTo = UtilsService.getItemByVal($scope.trunksToLinkTo, currentlyLinkedToTrunkId, "TrunkId");
                            currentlyLinkedToTrunkHasBeenSet = true;
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    })
                    .finally(function () {
                        $scope.isGettingData = false;
                    });
            }
        }

        $scope.saveTrunk = function () {
            if (editMode)
                return updateTrunk();
            else
                return insertTrunk();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() {
        $scope.isGettingData = true;

        return SwitchAPIService.GetSwitches()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.switches.push(item);
                });

                if (editMode) {
                    return TrunkAPIService.GetTrunkById(trunkId)
                        .then(function (response) {
                            fillScopeFromTrunkObj(response);

                            if (response.LinkedToTrunkId != null) {
                                return TrunkAPIService.GetTrunkById(response.LinkedToTrunkId)
                                    .then(function (response) {
                                        $scope.selectedSwitchToLinkTo = UtilsService.getItemByVal($scope.switchesToLinkTo, response.SwitchId, "SwitchId");
                                    })
                                    .catch(function (error) {
                                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                                    })
                                    .finally(function () {
                                        $scope.isGettingData = false;
                                    });
                            }
                            else {
                                currentlyLinkedToTrunkHasBeenSet = true;
                                $scope.isGettingData = false;
                            }
                        })
                        .catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                }
                else {
                    if (switchId != undefined && switchId != null) {
                        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, switchId, "SwitchId");
                        $scope.disableSwitchMenu = true;
                    }

                    $scope.isGettingData = false;
                }
            })
            .catch(function (error) {
                $scope.isGettingData = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function fillScopeFromTrunkObj(trunkObj) {
        $scope.name = trunkObj.Name;
        $scope.symbol = trunkObj.Symbol;
        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, trunkObj.SwitchId, "SwitchId");
        $scope.selectedType = UtilsService.getEnum(TrunkTypeEnum, "value", trunkObj.Type);
        $scope.selectedDirection = UtilsService.getEnum(TrunkDirectionEnum, "value", trunkObj.Direction);
        currentlyLinkedToTrunkId = trunkObj.LinkedToTrunkId;
    }

    function updateTrunk() {
        var trunkObj = buildTrunkObjFromScope();

        return TrunkAPIService.UpdateTrunk(trunkObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response, "Name or Symbol")) {
                    if ($scope.onTrunkUpdated != undefined)
                        $scope.onTrunkUpdated(response.UpdatedObject, currentlyLinkedToTrunkId, trunkObj.LinkedToTrunkId);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertTrunk() {
        var trunkObj = buildTrunkObjFromScope();

        return TrunkAPIService.AddTrunk(trunkObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch Trunk", response, "Name or Symbol")) {
                    if ($scope.onTrunkAdded != undefined)
                        $scope.onTrunkAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildTrunkObjFromScope() {
        return {
            TrunkId: trunkId,
            Name: $scope.name,
            Symbol: $scope.symbol,
            SwitchId: $scope.selectedSwitch.SwitchId,
            Type: $scope.selectedType.value,
            Direction: $scope.selectedDirection.value,
            LinkedToTrunkId: ($scope.selectedTrunkToLinkTo != undefined) ? $scope.selectedTrunkToLinkTo.TrunkId : null
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_TrunkEditorController", TrunkEditorController);
