app.service('VR_Sec_UserService', ['VRModalService',
    function (VRModalService) {

        function addUser(onUserAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onUserAdded = onUserAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', null, settings);
        }

        function editUser(userId, onUserUpdated) {
            var modalSettings = {
            };
            var parameters = {
                userId: userId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserUpdated = onUserUpdated;
            };
            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', parameters, modalSettings);
        }

        function resetPassword(userId) {
            var modalSettings = {
            };
            
            var parameters = {
                userId: userId
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/ResetPasswordEditor.html', parameters, modalSettings);
        }

        return ({
            addUser: addUser,
            editUser: editUser,
            resetPassword: resetPassword
        });

 }]);