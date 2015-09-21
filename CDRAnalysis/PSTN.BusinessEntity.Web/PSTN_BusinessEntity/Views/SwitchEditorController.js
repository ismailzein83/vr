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

        $scope.saveSwitch = function () {
            if (editMode)
                return updateSwitch();
            else
                return insertSwitch();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
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
    }

    function updateSwitch() {
        var switchObject = buildSwitchObjectFromScope();
        console.log(switchObject);

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
        console.log(switchObject);

        return SwitchAPIService.AddSwitch(switchObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response)) {
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
            AreaCode: $scope.areaCode
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchEditorController", SwitchEditorController);
