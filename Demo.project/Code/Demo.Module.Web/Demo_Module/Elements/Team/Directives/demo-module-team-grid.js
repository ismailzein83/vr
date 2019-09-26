"use strict";

app.directive("demoModuleTeamGrid", ["VRNotificationService", "Demo_Module_TeamAPIService", "Demo_Module_TeamService", "VRUIUtilsService",
    function (VRNotificationService, Demo_Module_TeamAPIService, Demo_Module_TeamService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctor = new TeamGridCtor($scope);
                ctor.initializeController();
            },

            templateUrl: "/Client/Modules/Demo_Module/Elements/Team/Directives/Templates/TeamGridTemplate.html"
        };

        function TeamGridCtor($scope) {
            this.initializeController = initializeController;

            var leagueID;

            var gridApi;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.teams = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_TeamAPIService.GetFilteredTeams(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var query;

                    if (payload != undefined) {
                        query = payload.query;
                        leagueID = payload.leagueID;

                        if (payload.hideLeagueColumn)
                            $scope.scopeModel.hideLeagueColumn = (payload.hideLeagueColumn != undefined);
                    }

                    return gridApi.retrieveData(query);
                };

                api.onTeamAdded = function(team){
                    return gridApi.itemAdded(team);
                };

                if ($scope.onReady != undefined && typeof ($scope.onReady) == "function")
                    $scope.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editTeam
                }];
            }

            function editTeam(team) {
                var onTeamUpdated = function (team) {
                    gridApi.itemUpdated(team);
                };

                var leagueIDItem;
                if (leagueID != undefined) {
                    leagueIDItem = { LeagueID: leagueID };
                }
                Demo_Module_TeamService.editTeam(onTeamUpdated, team.TeamID, leagueIDItem);
            }
        }

        return directiveDefinitionObject;
    }]);