(function (appControllers) {

    "use strict";

    leagueAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function leagueAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_League";

        function GetFilteredLeagues(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredLeagues"), input);
        }

        function GetLeagueByID(leagueID) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetLeagueByID"), {
                leagueID: leagueID
            });
        }

        function AddLeague(league) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddLeague"), league);
        }

        function UpdateLeague(league) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateLeague"), league);
        }

        function GetLeaguesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetLeaguesInfo"), {
                filter: filter
            });
        }

        return {
            GetFilteredLeagues: GetFilteredLeagues,
            GetLeagueByID: GetLeagueByID,
            GetLeaguesInfo: GetLeaguesInfo,
            AddLeague: AddLeague,
            UpdateLeague: UpdateLeague
        };
    }

    appControllers.service("Demo_Module_LeagueAPIService", leagueAPIService);
})(appControllers);