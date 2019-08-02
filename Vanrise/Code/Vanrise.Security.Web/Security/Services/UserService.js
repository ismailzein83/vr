﻿(function (appControllers) {

    'use strict';

    UserService.$inject = ['VRModalService', 'VR_Sec_UserAPIService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function UserService(VRModalService, VR_Sec_UserAPIService, VRNotificationService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            addUser: addUser,
            editUser: editUser,
            changeProvider: changeProvider,
            resetPassword: resetPassword,
            resetAuthServerPassword: resetAuthServerPassword,
            forgotPassword: forgotPassword,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToUser: registerObjectTrackingDrillDownToUser,
            getEntityUniqueName: getEntityUniqueName,
            registerHistoryViewAction: registerHistoryViewAction,
            getUserIdFieldType: getUserIdFieldType
        });



        function addUser(onUserAdded, connectionId) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserAdded = onUserAdded;
            };

            var modalParameters = {
                connectionId: connectionId
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', modalParameters, modalSettings);
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

        function changeProvider(userId, onUserUpdated) {
            var modalParameters = {
                userId: userId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onUserUpdated = onUserUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/User/ChangeProviderEditor.html', modalParameters, modalSettings);
        }

        function viewHistoryUser(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Security/Views/User/UserEditor.html', modalParameters, modalSettings);
        };

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

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Security_User_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryUser(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function getEntityUniqueName() {
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

        function getUserIdFieldType() {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType,Vanrise.GenericData.MainExtensions",
                ConfigId: "2e16c3d4-837b-4433-b80e-7c02f6d71467",
                RuntimeEditor: "vr-genericdata-fieldtype-businessentity-runtimeeditor",
                ViewerEditor: "vr-genericdata-fieldtype-businessentity-viewereditor",
                BusinessEntityDefinitionId: "217a8f71-1dd6-4613-8ae2-540a510f5ff5"
            };
        }
    };

    appControllers.service('VR_Sec_UserService', UserService);

})(appControllers);
