(function (appControllers) {
    'use strict';

    ForgotPasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService'];

    function ForgotPasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService) {
        var email;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                email = parameters.email;
            }
        }

        function defineScope() {
            $scope.yesClicked = function () {
                var forgotPasswordInput = {
                    Email: $scope.email
                };

                return VR_Sec_UserAPIService.ForgotPassword(forgotPasswordInput).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.noClicked = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            setTitle();
            setData();
        }

        function setData() {
            $scope.email = email;
        }

        function setTitle() {
            $scope.title = 'Forgot Password';
        }
    }

    appControllers.controller('VR_Sec_ForgotPasswordEditorController', ForgotPasswordEditorController);

})(appControllers);
