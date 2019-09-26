(function (appControllers) {
    "use strict";

    teamManagementController.$inject = ['$scope', 'Demo_Module_TeamService', 'VRUIUtilsService', 'UtilsService'];

    function teamManagementController($scope, Demo_Module_TeamService, VRUIUtilsService, UtilsService) {

        var teamGridAPI;
        var teamGridReadyDeferred = UtilsService.createPromiseDeferred();

        var leagueSelectorAPI;
        var leagueSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                teamGridAPI = api;
                teamGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onLeagueSelectorReady = function (api) {
                leagueSelectorAPI = api;
                leagueSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                return teamGridAPI.load(getFilter());
            };

            $scope.scopeModel.addTeam = function () {
                var onTeamAdded = function (team) {
                    if (teamGridAPI != undefined) {
                        teamGridAPI.onTeamAdded(team);
                    }
                };

                Demo_Module_TeamService.addTeam(onTeamAdded);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadLeagueSelector().then(function () {
                loadTeamGrid().then(function () {
                    $scope.scopeModel.isLoading = false;
                });
            });
        }

        function loadLeagueSelector() {
            var leagueSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            leagueSelectorReadyDeferred.promise.then(function () {

                var leagueSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(leagueSelectorAPI, leagueSelectorPayload, leagueSelectorLoadPromiseDeferred);
            });

            return leagueSelectorLoadPromiseDeferred.promise;
        }

        function loadTeamGrid() {
            var teamGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            teamGridReadyDeferred.promise.then(function () {
                teamGridAPI.load(getFilter()).then(function () {
                    teamGridLoadPromiseDeferred.resolve();
                });
            });

            return teamGridLoadPromiseDeferred.promise;
        }

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    LeagueIDs: leagueSelectorAPI.getSelectedIds()
                }
            };
        }
    }

    appControllers.controller('Demo_Module_TeamManagementController', teamManagementController);
})(appControllers);