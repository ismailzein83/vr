(function (appControllers) {

    "use strict";

    releaseCodeDescriptionViewerController.$inject = ['$scope', 'WhS_BE_SwitchReleaseCauseAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function releaseCodeDescriptionViewerController($scope, WhS_BE_SwitchReleaseCauseAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        var switchIds;
        var code;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                switchIds = parameters.switchIds;
                code = parameters.code;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.releaseCodeDescriptions = [];
        }

        function load() {
            $scope.scopeModel.isGettingData = true;
            WhS_BE_SwitchReleaseCauseAPIService.GetReleaseCausesByCode(code, switchIds).then(function (response) {
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
