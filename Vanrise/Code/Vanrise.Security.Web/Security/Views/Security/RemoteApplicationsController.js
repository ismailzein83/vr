(function (appControllers) {

    'use strict';

    RemoteApplicationsController.$inject = ['$scope', 'SecurityService', 'VRNotificationService', 'VRNavigationService','VR_Sec_RegisteredApplicationAPIService'];

    function RemoteApplicationsController($scope, SecurityService, VRNotificationService, VRNavigationService, VR_Sec_RegisteredApplicationAPIService) {

        loadParameters();
        defineScope();
        load();
        var securityProviderId;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                securityProviderId = parameters.securityProviderId;
            }
        }

        function defineScope() {

            $scope.redirectToApplication = function (applicationURL) {
                $scope.isLoadingRemoteApplications = true;
                SecurityService.redirectToApplication(applicationURL).then(function () {
                    
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingRemoteApplications = false;
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };


        }

        function load() {
            getRemoteRegisteredApplicationsInfo();
        }

        function getRemoteRegisteredApplicationsInfo() {
            $scope.isLoadingRemoteApplications = true;

            VR_Sec_RegisteredApplicationAPIService.GetRemoteRegisteredApplicationsInfo(securityProviderId).then(function (response) {
                if (response != undefined) {
                    $scope.remoteApplications = response;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingRemoteApplications = false;
            });
        };
    };

    appControllers.controller('VR_Sec_RemoteApplicationsController', RemoteApplicationsController);

})(appControllers);
