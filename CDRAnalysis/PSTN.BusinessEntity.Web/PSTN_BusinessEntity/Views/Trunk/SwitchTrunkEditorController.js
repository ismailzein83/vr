SwitchTrunkEditorController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SwitchTrunkEditorController($scope, SwitchTrunkAPIService, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var trunkID;
    var switchID;
    var editMode;
    var currentlyLinkedToTrunkHasBeenSet = false;
    var currentlyLinkedToTrunkID;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            trunkID = parameters.TrunkID;
            switchID = parameters.SwitchID;
        }

        editMode = (trunkID != undefined);
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.symbol = undefined;
        $scope.switches = [];
        $scope.selectedSwitch = undefined;
        $scope.types = UtilsService.getArrayEnum(SwitchTrunkTypeEnum);
        $scope.selectedType = undefined;
        $scope.directions = UtilsService.getArrayEnum(SwitchTrunkDirectionEnum);
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
                var index = UtilsService.getItemIndexByVal($scope.switchesToLinkTo, $scope.selectedSwitch.ID, "ID");
                $scope.switchesToLinkTo.splice(index, 1);
            }
        }

        $scope.onSwitchToLinkToChanged = function () {

            $scope.trunksToLinkTo = [];
            $scope.selectedTrunkToLinkTo = undefined;

            if ($scope.selectedSwitchToLinkTo != undefined) {
                
                $scope.isGettingData = true;
                
                var trunkFilterObj = {
                    SwitchIds: [$scope.selectedSwitchToLinkTo.ID],
                    TrunkNameFilter: null
                };

                SwitchTrunkAPIService.GetTrunksBySwitchIds(trunkFilterObj)
                    .then(function (responseArray) {

                        angular.forEach(responseArray, function (item) {
                            $scope.trunksToLinkTo.push(item);
                        });

                        // remove the trunk that's being edited from the list of trunks to link to
                        var index = UtilsService.getItemIndexByVal($scope.trunksToLinkTo, trunkID, "ID");
                        if (index >= 0)
                            $scope.trunksToLinkTo.splice(index, 1);

                        if (!currentlyLinkedToTrunkHasBeenSet && currentlyLinkedToTrunkID != null) {
                            $scope.selectedTrunkToLinkTo = UtilsService.getItemByVal($scope.trunksToLinkTo, currentlyLinkedToTrunkID, "ID");
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
                    return SwitchTrunkAPIService.GetSwitchTrunkByID(trunkID)
                        .then(function (response) {
                            fillScopeFromTrunkObject(response);

                            if (response.LinkedToTrunkID != null) {
                                return SwitchTrunkAPIService.GetSwitchTrunkByID(response.LinkedToTrunkID)
                                    .then(function (responseObject) {
                                        $scope.selectedSwitchToLinkTo = UtilsService.getItemByVal($scope.switchesToLinkTo, responseObject.SwitchID, "ID");
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
                    if (switchID != undefined && switchID != null) {
                        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, switchID, "ID");
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

    function fillScopeFromTrunkObject(trunkObject) {
        $scope.name = trunkObject.Name;
        $scope.symbol = trunkObject.Symbol;
        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, trunkObject.SwitchID, "ID");
        $scope.selectedType = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
        $scope.selectedDirection = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
        currentlyLinkedToTrunkID = trunkObject.LinkedToTrunkID;
    }

    function updateTrunk() {
        var trunkObject = buildTrunkObjectFromScope();

        return SwitchTrunkAPIService.UpdateSwitchTrunk(trunkObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response, "Name or Symbol")) {
                    if ($scope.onTrunkUpdated != undefined)
                        $scope.onTrunkUpdated(response.UpdatedObject, currentlyLinkedToTrunkID, trunkObject.LinkedToTrunkID);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertTrunk() {
        var trunkObject = buildTrunkObjectFromScope();

        return SwitchTrunkAPIService.AddSwitchTrunk(trunkObject)
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

    function buildTrunkObjectFromScope() {
        return {
            ID: trunkID,
            Name: $scope.name,
            Symbol: $scope.symbol,
            SwitchID: $scope.selectedSwitch.ID,
            Type: $scope.selectedType.value,
            Direction: $scope.selectedDirection.value,
            LinkedToTrunkID: ($scope.selectedTrunkToLinkTo != undefined) ? $scope.selectedTrunkToLinkTo.ID : null
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTrunkEditorController", SwitchTrunkEditorController);
