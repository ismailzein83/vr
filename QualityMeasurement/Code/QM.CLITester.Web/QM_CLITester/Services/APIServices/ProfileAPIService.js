(function (appControllers) {

    "use strict";
    profileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function profileAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        var controllerName = 'Profile';

        function GetFilteredProfiles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetFilteredProfiles"), input);
        }

        function GetProfile(profileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetProfile"), {
                profileId: profileId
            });
        }

        function UpdateProfile(profileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "UpdateProfile"), profileObject);
        }

        function GetProfileSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetProfileSourceTemplates"));
        }

        function GetProfilesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetProfilesInfo"));
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