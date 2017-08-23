﻿(function (appControllers) {
    'use strict';

    ActivatePasswordEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VRNotificationService', 'VR_Sec_SecurityAPIService'];

    function ActivatePasswordEditorController($scope, VR_Sec_UserAPIService, VRNavigationService, VRNotificationService, VR_Sec_SecurityAPIService) {
        var email;
        var userObj;
        var tempPassword;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                email = parameters.email;
                userObj = parameters.userObj;
                tempPassword = parameters.tempPassword;
            }
        }

        function defineScope() {
            $scope.passwordHint = "";
            $scope.save = function () {
                var activatePasswordInput = {
                    Email: email,
                    Password: $scope.password,
                    Name: $scope.name,
                    TempPassword : tempPassword
                };

                return VR_Sec_UserAPIService.ActivatePassword(activatePasswordInput).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
                        if ($scope.onPasswordActivated && typeof $scope.onPasswordActivated == 'function') {
                            $scope.onPasswordActivated($scope.password);
                        }

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.validatePasswords = function () {
                if ($scope.password != $scope.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };
        }

        function load() {
            return loadPasswordHint().then(function () {
                setTitle();
                setData();
            });
            
        }

        function setData() {
            $scope.name = userObj.Name;
        }

        function setTitle() {
            $scope.title = 'Activate Password';
        }

        function loadPasswordHint() {
            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo().then(function (response) {
                $scope.passwordHint = response.RequirementsMessage;
            });
        }
    }

    appControllers.controller('VR_Sec_ActivatePasswordEditorController', ActivatePasswordEditorController);

})(appControllers);
