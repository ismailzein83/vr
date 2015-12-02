(function (appControllers) {

    "use strict";

    switchEditorController.$inject = ['$scope', 'WhS_BE_SwitchAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

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

            $scope.SaveSwitch = function () {
                if (isEditMode) {
                    return updateSwitch();
                }
                else {
                    return insertSwitch();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
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



        function buildSwitchObjFromScope() {

            var whsSwitch = {
                SwitchId: (switchId != null) ? switchId : 0,
                Name: $scope.name
            };

            return whsSwitch;
        }

        function insertSwitch() {
            var switchObject = buildSwitchObjFromScope();
            return WhS_BE_SwitchAPIService.AddSwitch(switchObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch", response)) {
                    if ($scope.onSwitchAdded != undefined)
                        $scope.onSwitchAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSwitch() {
            var switchObject = buildSwitchObjFromScope();
            WhS_BE_SwitchAPIService.UpdateSwitch(switchObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch", response)) {
                    if ($scope.onSwitchUpdated != undefined)
                        $scope.onSwitchUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_SwitchEditorController', switchEditorController);
})(appControllers);
