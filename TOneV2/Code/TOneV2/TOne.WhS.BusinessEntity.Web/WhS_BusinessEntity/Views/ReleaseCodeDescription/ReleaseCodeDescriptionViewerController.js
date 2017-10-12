(function (appControllers) {

    "use strict";

    releaseCodeDescriptionViewerController.$inject = ['$scope', 'WhS_BE_SwitchAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function releaseCodeDescriptionViewerController($scope, WhS_BE_SwitchAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var switchId;
        var code;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                switchId = parameters.switchId;
                code = parameters.code;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.releaseCodeDescriptions = [];
        }

        function load() {
            $scope.scopeModel.isGettingData = true;
            WhS_BE_SwitchAPIService.GetReleaseCausesByCode(code, switchId).then(function (response) {
                $scope.scopeModel.releaseCodeDescriptions = response;
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isGettingData = false;
            });
        }

    }

    appControllers.controller('WhS_BE_ReleaseCodeDescriptionViewerController', releaseCodeDescriptionViewerController);
})(appControllers);
