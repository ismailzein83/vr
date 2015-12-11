
app.service('Qm_CliTester_ProfileService', ['Qm_CliTester_ProfileAPIService', 'VRModalService', 'VRNotificationService',
    function (Qm_CliTester_ProfileAPIService, VRModalService, VRNotificationService) {

        return ({
            editProfile: editProfile
        });


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

    }]);
