(function (appControllers) {

    "use strict";

    overriddenConfigGroupEditorController.$inject = ['$scope', 'VRCommon_OverriddenConfigGroupAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function overriddenConfigGroupEditorController($scope, VRCommon_OverriddenConfigGroupAPIService, VRNotificationService, VRNavigationService, UtilsService) {

       
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope); 
        }
        function defineScope() {
            $scope.saveOverriddenConfigGroup = function () {
                return insertOverriddenConfigGroup();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
                $scope.title = UtilsService.buildTitleForAddEditor("Overridden Configuration Group");
                $scope.isLoading = false;
        }

        function buildOverriddenConfigGroupObjFromScope() {
            var obj = {
                OverriddenConfigurationGroupId: undefined,
                Name: $scope.name,
            };
            return obj;
        }
        function insertOverriddenConfigGroup() {
            $scope.isLoading = true;

            var overriddenConfigGroupObject = buildOverriddenConfigGroupObjFromScope();
            return VRCommon_OverriddenConfigGroupAPIService.AddOverriddenConfigurationGroup(overriddenConfigGroupObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Ovrridden Configuration Group", response, "Name")) {
                    if ($scope.onOverriddenConfigGroupAdded != undefined)
                        $scope.onOverriddenConfigGroupAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }
    }

    appControllers.controller('VRCommon_OverriddenConfigGroupEditorController', overriddenConfigGroupEditorController);
})(appControllers);
