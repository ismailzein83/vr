(function (appControllers) {

    'use strict';

    UserService.$inject = ['VRModalService', 'VR_Sec_UserAPIService', 'VRNotificationService'];

    function UserService(VRModalService, VR_Sec_UserAPIService, VRNotificationService) {
        return ({
            addUser: addUser,
            editUser: editUser,
            resetPassword: resetPassword,
            resetAuthServerPassword: resetAuthServerPassword,
            activatePassword: activatePassword,
            forgotPassword: forgotPassword
        });

        function addUser(onUserAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAdded = onUserAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', null, modalSettings);
        }

        function editUser(userId, onUserUpdated) {
            var modalParameters = {
                userId: userId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserUpdated = onUserUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', modalParameters, modalSettings);
        }

        function resetPassword(userId) {
            var modalParameters = {
                userId: userId
            };

            var modalSettings = {};

            VRModalService.showModal('/Client/Modules/Security/Views/User/ResetPasswordEditor.html', modalParameters, modalSettings);
        }


        function resetAuthServerPassword(scope, userId) {
            VRNotificationService.showConfirmation('Are you sure you want to reset the password?')
                .then(function (response) {
                    if (response) {
                        var resetPasswordInput = { UserId: userId };
                        return VR_Sec_UserAPIService.ResetPassword(resetPasswordInput)
                            .then(function () {
                                VRNotificationService.showSuccess("Password has been successfully reset");
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }



        function activatePassword(email, userObj, tempPassword, onPasswordActivated) {
            var modalParameters = {
                email: email,
                userObj: userObj,
                tempPassword: tempPassword
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPasswordActivated = onPasswordActivated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/ActivatePasswordEditor.html', modalParameters, modalSettings);
        }

        function forgotPassword(email) {
            var modalParameters = {
                email: email
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/ForgotPasswordEditor.html', modalParameters, undefined);
        }
        
    };

    appControllers.service('VR_Sec_UserService', UserService);

})(appControllers);
