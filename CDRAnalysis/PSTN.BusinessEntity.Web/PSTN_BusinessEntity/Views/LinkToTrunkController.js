LinkToTrunkController.$inject = ["$scope", "SwitchAPIService", "SwitchTrunkAPIService", "VRNavigationService", "VRNotificationService"];

function LinkToTrunkController($scope, SwitchAPIService, SwitchTrunkAPIService, VRNavigationService, VRNotificationService) {

    var switchTrunkID = undefined;
    var switchID = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            switchTrunkID = parameters.TrunkID;
            switchID = parameters.SwitchID;
        }
    }

    function defineScope() {

        $scope.switches = [];
        $scope.selectedSwitch = undefined;
        $scope.switchTrunks = [];
        $scope.selectedSwitchTrunk = undefined;

        $scope.saveSwitchTrunk = function () {
            console.log(switchTrunkID);
            console.log($scope.selectedSwitchTrunk.ID);

            return SwitchTrunkAPIService.LinkToTrunk(switchTrunkID, $scope.selectedSwitchTrunk.ID)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response)) {
                        if ($scope.onSwitchTrunkUpdated != undefined)
                            $scope.onSwitchTrunkUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }

        $scope.onSwitchChanged = function () {

            if ($scope.selectedSwitch != undefined) {
                $scope.switchTrunks = [];
                $scope.isGettingData = true;

                SwitchTrunkAPIService.GetSwitchTrunksBySwitchID($scope.selectedSwitch.ID)
                    .then(function (responseArray) {
                        angular.forEach(responseArray, function (item) {
                            $scope.switchTrunks.push(item);
                        });
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    })
                    .finally(function () {
                        $scope.isGettingData = false;
                    });
            }
        }
    }

    function load() {
        $scope.isGettingData = true;

        return SwitchAPIService.GetSwitchesToLinkTo(switchID)
            .then(function (responseArray) {
                angular.forEach(responseArray, function (item) {
                    $scope.switches.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_LinkToTrunkController", LinkToTrunkController);
