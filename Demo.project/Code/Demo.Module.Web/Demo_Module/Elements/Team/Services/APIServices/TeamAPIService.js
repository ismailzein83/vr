(function (appControllers) {

    "use strict";

    teamAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function teamAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Team";

        function GetFilteredTeams(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredTeams"), input);
        }

        function GetTeamByID(teamID) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetTeamByID"), {
                teamID: teamID
            });
        }

        function GetTeamSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetTeamSettingsConfigs"));
        }

        function GetPlayerTypeConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPlayerTypeConfigs"));
        }

        function AddTeam(team) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddTeam"), team);
        }

        function UpdateTeam(team) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateTeam"), team);
        }

        return {
            GetFilteredTeams: GetFilteredTeams,
            GetTeamByID: GetTeamByID,
            GetTeamSettingsConfigs: GetTeamSettingsConfigs,
            GetPlayerTypeConfigs: GetPlayerTypeConfigs,
            AddTeam: AddTeam,
            UpdateTeam: UpdateTeam
        };
    }

    appControllers.service("Demo_Module_TeamAPIService", teamAPIService);
})(appControllers);