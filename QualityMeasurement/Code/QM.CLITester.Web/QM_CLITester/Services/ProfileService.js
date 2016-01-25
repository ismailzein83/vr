﻿(function (appControllers) {

    'use strict';

    ProfileService.$inject = ['QM_CLITester_ProfileAPIService', 'VRModalService', 'VRNotificationService'];

    function ProfileService(QM_CLITester_ProfileAPIService, VRModalService, VRNotificationService) {
        return {
            editProfile: editProfile
        };
        function editProfile(profileId, onProfileUpdated) {
            var modalSettings = {
            };
            var parameters = {
                profileId: profileId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onProfileUpdated = onProfileUpdated;
            };
            VRModalService.showModal('/Client/Modules/QM_CLITester/Views/Profile/ProfileEditor.html', parameters, modalSettings);
        }
    }
    appControllers.service('QM_CLITester_ProfileService', ProfileService);

})(appControllers);