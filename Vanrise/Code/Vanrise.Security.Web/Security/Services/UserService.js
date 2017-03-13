(function (appControllers) {

    'use strict';

    UserService.$inject = ['VRModalService', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function UserService(VRModalService, VR_Sec_UserAPIService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addUser: addUser,
            editUser: editUser,
            resetPassword: resetPassword,
            resetAuthServerPassword: resetAuthServerPassword,
            forgotPassword: forgotPassword,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToUser: registerObjectTrackingDrillDownToUser,
            getEntityUniqueName:getEntityUniqueName
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
        
        function forgotPassword(email) {
            var modalParameters = {
                email: email
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/ForgotPasswordEditor.html', modalParameters, undefined);
        }

        function getEntityUniqueName()
        {
            return "VR_Security_User";
        }

        function registerObjectTrackingDrillDownToUser() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";
            

            drillDownDefinition.loadDirective = function (directiveAPI, userItem) {
                userItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: userItem.Entity.UserId,
                    EntityUniqueName: getEntityUniqueName(),
                    
                };
                return userItem.objectTrackingGridAPI.load(query);
            };
            
            addDrillDownDefinition(drillDownDefinition);
           
        }
        function addDrillDownDefinition(drillDownDefinition) {
          
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    };

    appControllers.service('VR_Sec_UserService', UserService);

})(appControllers);
