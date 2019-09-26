(function (appControllers) {

    "use strict";

    teamService.$inject = ['VRModalService'];

    function teamService(VRModalService) {

        function addTeam(onTeamAdded, leagueIDItem) {

            var parameters = {
                leagueIDItem: leagueIDItem
            };

            var settings = {
                onScopeReady: function (modalScope) {
                    modalScope.onTeamAdded = onTeamAdded;
                }
            };

            VRModalService.showModal("/Client/Modules/Demo_Module/Elements/Team/Views/TeamEditor.html", parameters, settings);
        }

        function editTeam(onTeamUpdated, teamID, leagueIDItem) {
            var parameters = {
                teamID: teamID,
                leagueIDItem: leagueIDItem
            };

            var settings = {
                onScopeReady: function (modalScope) {
                    modalScope.onTeamUpdated = onTeamUpdated;
                }
            };

            VRModalService.showModal("/Client/Modules/Demo_Module/Elements/Team/Views/TeamEditor.html", parameters, settings);
        }

        return {
            addTeam: addTeam,
            editTeam: editTeam
        };
    }

    appControllers.service("Demo_Module_TeamService", teamService);
})(appControllers);