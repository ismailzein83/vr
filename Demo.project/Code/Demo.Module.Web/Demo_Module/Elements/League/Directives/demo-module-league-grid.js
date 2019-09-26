"use strict";

app.directive("demoModuleLeagueGrid", ["VRNotificationService", "Demo_Module_LeagueAPIService", "Demo_Module_LeagueService", "VRUIUtilsService",
    function (VRNotificationService, Demo_Module_LeagueAPIService, Demo_Module_LeagueService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LeagueGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/League/Directives/Templates/LeagueGridTemplate.html"
        };

        function LeagueGridCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridApi;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.leagues = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridApi);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_LeagueAPIService.GetFilteredLeagues(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridApi.retrieveData(query);
                };

                api.onLeagueAdded = function (league) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(league);
                    gridApi.itemAdded(league);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function buildDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(buildTeamDrillDownDefinition());
                return drillDownDefinitions;
            }

            function buildTeamDrillDownDefinition() {
                var drillDownDefinition = {};
                drillDownDefinition.title = "Team";
                drillDownDefinition.directive = "demo-module-team-search";

                drillDownDefinition.loadDirective = function (directiveAPI, leagueItem) {
                    leagueItem.teamGridAPI = directiveAPI;

                    var payload = {
                        leagueID: leagueItem.LeagueID
                    };
                    return leagueItem.teamGridAPI.load(payload);
                };

                return drillDownDefinition;
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editLeague
                }];
            }

            function editLeague(league) {

                var onLeagueUpdated = function (league) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(league);
                    gridApi.itemUpdated(league);
                };
                Demo_Module_LeagueService.editLeague(onLeagueUpdated, league.LeagueID);
            }
        }

        return directiveDefinitionObject;
    }]);