SwitchEditorController.$inject = ["$scope", "SwitchAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SwitchEditorController($scope, SwitchAPIService, UtilsService, VRNavigationService, VRNotificationService) {

    var switchID = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            switchID = parameters.SwitchID;

        editMode = (switchID != undefined);
    }

    function defineScope() {

        $scope.name = undefined;
        $scope.types = [];
        $scope.selectedType = undefined;
        $scope.areaCode = undefined;
        $scope.timeOffset = undefined;

        $scope.saveSwitch = function () {
            if (editMode)
                return updateSwitch();
            else
                return insertSwitch();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }

        $scope.validateTimeOffset = function (value) {

            if (value == undefined) return null;

            if (value.length == 0) return null;

            if (value.length != 8) return "Format: HH:MM:SS";

            for (var i = 0; i < value.length; i++) {

                if ((i == 2 || i == 5) && value.charAt(i) != ":")
                    return "Format: HH:MM:SS";
                else if (value.charAt(i) != ":" && isNaN(parseInt(value.charAt(i))))
                    return "Format: HH:MM:SS";
            }
        }
    }

    function load() {
        $scope.isGettingData = true;

        return SwitchAPIService.GetSwitchTypes()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.types.push(item);
                });

                if (editMode) {
                    return SwitchAPIService.GetSwitchByID(switchID)
                        .then(function (response) {
                            fillScopeFromSwitchObject(response);
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

    function fillScopeFromSwitchObject(switchObject) {
        $scope.name = switchObject.Name;
        $scope.selectedType = UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID");
        $scope.areaCode = switchObject.AreaCode;
        $scope.timeOffset = switchObject.TimeOffset;
    }

    function updateSwitch() {
        var switchObject = buildSwitchObjectFromScope();

        return SwitchAPIService.UpdateSwitch(switchObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                    if ($scope.onSwitchUpdated != undefined)
                        $scope.onSwitchUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertSwitch() {
        var switchObject = buildSwitchObjectFromScope();

        return SwitchAPIService.AddSwitch(switchObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                    if ($scope.onSwitchAdded != undefined)
                        $scope.onSwitchAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildSwitchObjectFromScope() {
        return {
            ID: switchID,
            Name: $scope.name,
            TypeID: ($scope.selectedType != undefined) ? $scope.selectedType.ID : null,
            AreaCode: $scope.areaCode,
            TimeOffset: $scope.timeOffset
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchEditorController", SwitchEditorController);
