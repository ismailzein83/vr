(function (appControllers) {

    'use strict';

    UserService.$inject = ['VRModalService'];

    function UserService(VRModalService) {
        return ({
            addUser: addUser,
            editUser: editUser,
            resetPassword: resetPassword
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
    };

    appControllers.service('VR_Sec_UserService', UserService);

})(appControllers);
