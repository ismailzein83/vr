(function (appControllers) {

    "use strict";

    leagueService.$inject = ['VRModalService'];

    function leagueService(VRModalService) {

        function addLeague(onLeagueAdded) {

            var parameters;

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onLeagueAdded = onLeagueAdded;
            };
            VRModalService.showModal("/Client/Modules/Demo_Module/Elements/League/Views/LeagueEditor.html", parameters, settings);
        }

        function editLeague(onLeagueUpdated, leagueID) {

            var parameters = {
                leagueID: leagueID
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onLeagueUpdated = onLeagueUpdated;
            };
            VRModalService.showModal("/Client/Modules/Demo_Module/Elements/League/Views/LeagueEditor.html", parameters, settings);
        }

        return {
            addLeague: addLeague,
            editLeague: editLeague
        };
    }

    appControllers.service("Demo_Module_LeagueService", leagueService);
})(appControllers);