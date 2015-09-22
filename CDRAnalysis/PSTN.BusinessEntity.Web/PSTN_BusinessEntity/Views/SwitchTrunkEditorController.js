SwitchTrunkEditorController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SwitchTrunkEditorController($scope, SwitchTrunkAPIService, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var trunkID = undefined;

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

    function fillScopeFromTrunkObject(trunkObject) {
        $scope.name = trunkObject.Name;
        $scope.symbol = trunkObject.Symbol;
        $scope.selectedSwitch = UtilsService.getItemByVal($scope.switches, trunkObject.SwitchID, "ID");
        $scope.selectedType = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
        $scope.selectedDirection = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
    }

    function updateTrunk() {
        var trunkObject = buildTrunkObjectFromScope();

        return SwitchTrunkAPIService.UpdateSwitchTrunk(trunkObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response, "Name")) {
                    if ($scope.onTrunkUpdated != undefined)
                        $scope.onTrunkUpdated(response.UpdatedObject);

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
            Direction: $scope.selectedDirection.value
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTrunkEditorController", SwitchTrunkEditorController);
