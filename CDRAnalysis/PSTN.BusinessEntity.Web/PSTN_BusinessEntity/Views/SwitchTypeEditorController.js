SwitchTypeEditorController.$inject = ["$scope", "SwitchTypeAPIService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchTypeEditorController($scope, SwitchTypeAPIService, VRNavigationService, VRNotificationService, VRModalService) {

    var switchTypeID = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            switchTypeID = parameters.SwitchTypeID;

        editMode = (switchTypeID != undefined);
    }

    function defineScope() {

        $scope.name = undefined;

        $scope.saveSwitchType = function () {
            if (editMode)
                return updateSwitchType();
            else
                return insertSwitchType();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() {

        if (editMode) {
            $scope.isGettingData = true;

            SwitchTypeAPIService.GetSwitchTypeByID(switchTypeID)
                .then(function (responseObject) {
                    fillScopeFromSwitchTypeObject(responseObject);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isGettingData = false;
                });
        }
    }

    function fillScopeFromSwitchTypeObject(switchTypeObject) {
        $scope.name = switchTypeObject.Name;
    }

    function updateSwitchType() {
        var switchTypeObject = buildSwitchTypeObjectFromScope();

        return SwitchTypeAPIService.UpdateSwitchType(switchTypeObject)
            .then(function (responseObject) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Type", responseObject, "Name")) {
                    if ($scope.onSwitchTypeUpdated != undefined)
                        $scope.onSwitchTypeUpdated(responseObject.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertSwitchType() {
        var switchTypeObject = buildSwitchTypeObjectFromScope();

        return SwitchTypeAPIService.AddSwitchType(switchTypeObject)
            .then(function (responseObject) {
                if (VRNotificationService.notifyOnItemAdded("Switch Type", responseObject, "Name")) {
                    if ($scope.onSwitchTypeAdded != undefined)
                        $scope.onSwitchTypeAdded(responseObject.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildSwitchTypeObjectFromScope() {
        return {
            ID: switchTypeID,
            Name: $scope.name
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTypeEditorController", SwitchTypeEditorController);
