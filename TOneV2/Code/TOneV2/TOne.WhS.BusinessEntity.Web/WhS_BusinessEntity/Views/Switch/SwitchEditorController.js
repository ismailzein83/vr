(function (appControllers) {

    "use strict";

    switchEditorController.$inject = ["$scope", "WhS_BE_SwitchAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function switchEditorController($scope, WhS_BE_SwitchAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var switchId;
        var switchEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                switchId = parameters.switchId;
            }

            isEditMode = (switchId != undefined);
        }

        function defineScope() {

            $scope.hasSaveSwitchPermission = function () {
                if (isEditMode)
                    return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
                else
                    return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
            }

            $scope.SaveSwitch = function () {
                if (isEditMode) {
                    return updateSwitch();
                }
                else {
                    return insertSwitch();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Switch";
                getSwitch().then(function () {
                    $scope.isLoading = false;
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                $scope.title = "New Switch";
                $scope.isLoading = false;
            }
        }

        function getSwitch() {
            return WhS_BE_SwitchAPIService.GetSwitch(switchId).then(function (whsSwitch) {
                switchEntity = whsSwitch;
                $scope.name = switchEntity.Name;
            });
        }

        function insertSwitch() {
            $scope.isLoading = true;
            var whsSwitch = buildSwitchToAddFromScope();
            return WhS_BE_SwitchAPIService.AddSwitch(whsSwitch)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response, "Name")) {
                    if ($scope.onSwitchAdded != undefined)
                        $scope.onSwitchAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updateSwitch() {
            $scope.isLoading = true;
            var whsSwitch = buildSwitchToEditFromScope();
            return WhS_BE_SwitchAPIService.UpdateSwitch(whsSwitch)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response, "Name")) {
                    if ($scope.onSwitchUpdated != undefined)
                        $scope.onSwitchUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildSwitchToAddFromScope() {
            return buildBaseSwitchFromScope();
        }

        function buildSwitchToEditFromScope() {
            return buildBaseSwitchFromScope();
        }

        function buildBaseSwitchFromScope() {
            return {
                SwitchId: (switchId != null) ? switchId : 0,
                Name: $scope.name
            };
        }
    }

    appControllers.controller("WhS_BE_SwitchEditorController", switchEditorController);
})(appControllers);
