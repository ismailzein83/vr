(function (appControllers) {

    "use strict";
    profileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function profileAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        function GetFilteredProfiles(input) {
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

        function GetProfileSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "Profile", "GetProfileSourceTemplates"));
        }

        function GetProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "Profile", "GetProfilesInfo"));
        }

        return ({
            GetFilteredProfiles: GetFilteredProfiles,
            GetProfile: GetProfile,
            UpdateProfile: UpdateProfile,
            GetProfileSourceTemplates: GetProfileSourceTemplates,
            GetProfilesInfo: GetProfilesInfo
        });
    }

    appControllers.service('QM_CLITester_ProfileAPIService', profileAPIService);
})(appControllers);