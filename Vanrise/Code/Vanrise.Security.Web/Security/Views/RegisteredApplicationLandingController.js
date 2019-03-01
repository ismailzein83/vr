(function (appControllers) {

    'use strict';

    RegisteredApplicationLanding.$inject = ['$rootScope', '$scope', 'VRNavigationService', 'UISettingsService', 'VR_Sec_RegisteredApplicationAPIService', 'SecurityService', 'VRNotificationService'];

    function RegisteredApplicationLanding($rootScope, $scope, VRNavigationService, UISettingsService, VR_Sec_RegisteredApplicationAPIService, SecurityService, VRNotificationService) {



        defineScope();
        load();

        function defineScope() {
            $scope.registredApplications = [];
            $scope.openApplication = function (item) {
                $scope.isLoading = true;
                SecurityService.redirectToRegisteredApplication(item.URL, item.ApplicationId).then(function () {
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
            };
        }

        function load() {
            $scope.isLoading = true;
            VR_Sec_RegisteredApplicationAPIService.GetAllRegisteredApplications()
                .then(function (response) {
                    $scope.registredApplications.length = 0;
                    $scope.registredApplications = response;
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });

        }
    }

    appControllers.controller('VR_Sec_RegisteredApplicationLanding', RegisteredApplicationLanding);

})(appControllers);