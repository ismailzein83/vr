SwitchTrunkEditorController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SwitchTrunkEditorController($scope, SwitchTrunkAPIService, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var trunkID = undefined;
    var editMode = undefined;
    var editorLoaded = false;
    var previouslyLinkedToTrunkID;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            trunkID = parameters.TrunkID;

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

        $scope.onSwitchToLinkToChanged = function () {

            $scope.trunksToLinkTo = [];
            $scope.selectedTrunkToLinkTo = undefined;

            if ($scope.selectedSwitchToLinkTo != undefined) {
                
                $scope.isGettingData = true;

                SwitchTrunkAPIService.GetSwitchTrunksBySwitchID($scope.selectedSwitchToLinkTo.ID)
                    .then(function (responseArray) {
                        angular.forEach(responseArray, function (item) {
                            $scope.trunksToLinkTo.push(item);
                        });

                        if (!editorLoaded && previouslyLinkedToTrunkID != null) {
                            $scope.selectedTrunkToLinkTo = UtilsService.getItemByVal($scope.trunksToLinkTo, previouslyLinkedToTrunkID, "ID");
                            editorLoaded = true;
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
                    $scope.switchesToLinkTo.push(item);
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
                            else
                                $scope.isGettingData = false;
                        })
                        .catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                }
                else
                    $scope.isGettingData = false;
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
        previouslyLinkedToTrunkID = trunkObject.LinkedToTrunkID;
    }

    function updateTrunk() {
        var trunkObject = buildTrunkObjectFromScope();

        return SwitchTrunkAPIService.UpdateSwitchTrunk(trunkObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response, "Name")) {
                    if ($scope.onTrunkUpdated != undefined)
                        $scope.onTrunkUpdated(response.UpdatedObject, previouslyLinkedToTrunkID, trunkObject.LinkedToTrunkID);

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
                if (VRNotificationService.notifyOnItemAdded("Switch Trunk", response, "Name")) {
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
