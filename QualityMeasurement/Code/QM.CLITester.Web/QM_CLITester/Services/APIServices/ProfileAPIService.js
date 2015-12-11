(function (appControllers) {

    "use strict";
    ProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Qm_CliTester_ModuleConfig'];

    function ProfileAPIService(BaseAPIService, UtilsService, Qm_CliTester_ModuleConfig) {

        function GetFilteredProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Qm_CliTester_ModuleConfig.moduleName, "Profile", "GetFilteredProfiles"), input);
        }

        function GetProfile(profileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Qm_CliTester_ModuleConfig.moduleName, "Profile", "GetProfile"), {
                profileId: profileId
            });
        }

        function UpdateProfile(profileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Qm_CliTester_ModuleConfig.moduleName, "Profile", "UpdateProfile"), profileObject);
        }
        
        return ({
            GetFilteredProfiles: GetFilteredProfiles,
            GetProfile: GetProfile,
            UpdateProfile: UpdateProfile
        });
    }

    appControllers.service('Qm_CliTester_ProfileAPIService', ProfileAPIService);
})(appControllers);