(function (appControllers) {

    "use strict";

    leagueManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_LeagueService', 'UtilsService', 'VRUIUtilsService'];

    function leagueManagementController($scope, VRNotificationService, Demo_Module_LeagueService, UtilsService, VRUIUtilsService) {

        var leagueGridAPI;
        var leagueGridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isLoading = true;

            $scope.scopeModel.onGridReady = function (api) {
                leagueGridAPI = api;
                leagueGridReadyDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                return leagueGridAPI.load(getFilter());
            };

            $scope.scopeModel.addLeague = function () {
                var onLeagueAdded = function (league) {
                    if (leagueGridAPI != undefined) {
                        leagueGridAPI.onLeagueAdded(league);
                    }
                };

                Demo_Module_LeagueService.addLeague(onLeagueAdded);
            };
        }

        function load() {
            loadLeagueGrid().then(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadLeagueGrid() {
            var leagueGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            leagueGridReadyDeferred.promise.then(function () {
                leagueGridAPI.load(getFilter()).then(function () {
                    leagueGridLoadPromiseDeferred.resolve();
                });
            });

            return leagueGridLoadPromiseDeferred.promise;
        }

        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Demo_Module_LeagueManagementController', leagueManagementController);
})(appControllers);