(function (appControllers) {

    "use strict";
    ProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function ProfileAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        function GetFilteredProfiles(input) {
            console.log('111111111111111')
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "Profile", "GetFilteredProfiles"), input);
        }

        function GetProfile(profileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "Profile", "GetProfile"), {
                profileId: profileId
            });
        }

        function UpdateProfile(profileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "Profile", "UpdateProfile"), profileObject);
        }
        
        return ({
            GetFilteredProfiles: GetFilteredProfiles,
            GetProfile: GetProfile,
            UpdateProfile: UpdateProfile
        });
    }

    appControllers.service('QM_CLITester_ProfileAPIService', ProfileAPIService);
})(appControllers);