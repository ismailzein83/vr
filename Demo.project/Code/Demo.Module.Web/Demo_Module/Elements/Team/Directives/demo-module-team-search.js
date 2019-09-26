"use strict";

app.directive("demoModuleTeamSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_TeamService',
    function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_TeamService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope) {
                var ctor = new TeamSearchCtor($scope);
                ctor.initializeController();
            },
            templateUrl: "/Client/Modules/Demo_Module/Elements/Team/Directives/Templates/TeamSearchTemplate.html"
        };

        function TeamSearchCtor($scope,) {
            this.initializeController = initializeController;

            var leagueID;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.addTeam = function () {
                    var onTeamAdded = function (team) {
                        gridAPI.onTeamAdded(team);
                    };

                    var leagueIDItem;
                    if (leagueID != undefined) {
                        leagueIDItem = { LeagueID: leagueID };
                    }
                    Demo_Module_TeamService.addTeam(onTeamAdded, leagueIDItem);
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        leagueID = payload.leagueID;
                    }
                    return gridAPI.load(getGridPayload());
                };

                api.onTeamAdded = function (team) {
                    gridAPI.onTeamAdded(team);
                };

                if ($scope.onReady != undefined && typeof ($scope.onReady) == "function")
                    $scope.onReady(api);
            }

            function getGridPayload() {
                var payload = {
                    query: { LeagueIDs: [leagueID] },
                    leagueID: leagueID,
                    hideLeagueColumn: true
                };
                return payload;
            }
        }

        return directiveDefinitionObject;
    }]);
