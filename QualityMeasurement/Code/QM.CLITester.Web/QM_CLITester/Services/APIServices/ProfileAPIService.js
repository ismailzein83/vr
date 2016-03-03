(function (appControllers) {

    "use strict";
    profileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig', 'SecurityService'];

    function profileAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig, SecurityService) {

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

        function HasEditProfilePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_CLITester_ModuleConfig.moduleName, controllerName, ['UpdateProfile']));
        }

        return ({
            HasEditProfilePermission: HasEditProfilePermission,
            GetFilteredProfiles: GetFilteredProfiles,
            GetProfile: GetProfile,
            UpdateProfile: UpdateProfile,
            GetProfileSourceTemplates: GetProfileSourceTemplates,
            GetProfilesInfo: GetProfilesInfo
        });
    }

    appControllers.service('QM_CLITester_ProfileAPIService', profileAPIService);
})(appControllers);