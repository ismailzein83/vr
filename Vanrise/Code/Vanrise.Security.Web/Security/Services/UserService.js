app.service('VR_Sec_UserService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {

        function addUser(onUserAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onUserAdded = onUserAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', null, settings);
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
            VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', parameters, modalSettings);
        }

        function resetPassword(userId) {
            var modalSettings = {
            };
            var parameters = {
                userId: userId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Reset Password for User: " //+ userObj.Entity.Name;
                
            };

            VRModalService.showModal('/Client/Modules/Security/Views/ResetPasswordEditor.html', parameters, modalSettings);
        }

        function assignPermissions(userId) {
            var modalSettings = {
            };
            var parameters = {
                holderType: 0,
                holderId: userId,
                notificationResponseText: "User Permissions"
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Assign Permissions to User: " //+ userObj.Entity.Name;
            };
            VRModalService.showModal('/Client/Modules/Security/Views/PermissionEditor.html', parameters, modalSettings);
        }

        return ({
            addUser: addUser,
            editUser: editUser,
            resetPassword: resetPassword,
            assignPermissions: assignPermissions
        });

 }]);