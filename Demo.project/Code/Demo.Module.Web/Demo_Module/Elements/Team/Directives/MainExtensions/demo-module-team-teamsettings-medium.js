"use strict";

app.directive("demoModuleTeamTeamsettingsMedium", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                validationcontext: "="
            },
            controller: function ($scope) {
                var ctor = new MediumTeamCtor($scope);
                ctor.initializeController();
            },

            templateUrl: "/Client/Modules/Demo_Module/Elements/Team/Directives/MainExtensions/Templates/MediumTeamTemplate.html"
        };

        function MediumTeamCtor($scope) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridAPIdeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIdeferred.resolve();
                };

                $scope.scopeModel.addPlayerField = function () {
                    var playerIDs = $scope.scopeModel.datasource.map(function (array) {
                        return array.ID;
                    });

                    var lastplayerID = Math.max.apply(Math, playerIDs);

                    var gridItem = {
                        ID: lastplayerID + 1,
                        Name: undefined,
                        DOB: undefined,
                        Number: undefined,
                        playerTypeEntity: undefined,
                        playerTypeSelectorAPI: undefined
                    };

                    gridItem.onPlayerTypeDirectiveReady = function (api) {
                        gridItem.playerTypeSelectorAPI = api;

                        var playerTypeSelectorPayload = undefined;

                        var setLoader = function (value) {
                            gridItem.isPlayerTypeSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.playerTypeSelectorAPI, playerTypeSelectorPayload, setLoader, undefined);
                    };

                    $scope.scopeModel.datasource.push(gridItem);
                    gridAPI.expandRow(gridItem);
                };

                $scope.scopeModel.removeFilter = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, item.ID, 'ID');
                    if (index > -1) {
                        $scope.scopeModel.datasource.splice(index, 1);
                    }
                };

                $scope.scopeModel.isValid = function () {
                    if (checkDuplicateInArray($scope.scopeModel.datasource))
                        return "Same Number exist";

                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should Add at least one field";

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var players;

                    if (payload != undefined) {
                        if (payload.teamSettingsEntity != undefined) {
                            players = payload.teamSettingsEntity.Players;
                        }
                    }

                    loadPlayerGrid(players);
                };

                api.getData = function () {
                    var dsPlayers = $scope.scopeModel.datasource;
                    if (dsPlayers != undefined) {
                        var players = [];
                        for (var i = 0; i < dsPlayers.length; i++) {
                            var currentItem = dsPlayers[i];
                            if (currentItem != undefined) {
                                players.push({
                                    ID: currentItem.ID,
                                    Name: currentItem.Name,
                                    DOB: currentItem.dob,
                                    Number: currentItem.Number,
                                    Type: currentItem.playerTypeSelectorAPI != undefined ? currentItem.playerTypeSelectorAPI.getData() : currentItem.playerTypeEntity
                                });
                            }
                        }

                        return {
                            "$type": "Demo.Module.MainExtension.Team.MediumTeamType, Demo.Module.MainExtension",
                            Players: players
                        };
                    }
                };

                if ($scope.onReady != null)
                    $scope.onReady(api);
            }

            function loadPlayerGrid(players) {
                if (players == undefined)
                    return;

                for (var i = 0; i < players.length; i++) {
                    var currentPlayer = players[i];
                    if (currentPlayer != undefined) {
                        loadPlayer(currentPlayer);
                    }
                }
            }

            function loadPlayer(player) {
                var gridItem = {
                    ID: player.ID,
                    Name: player.Name,
                    Number: player.Number,
                    dob: player.DOB,
                    playerTypeEntity: player.Type,
                    playerTypeSelectorAPI: undefined
                };

                gridItem.onPlayerTypeDirectiveReady = function (api) {
                    gridItem.playerTypeSelectorAPI = api;

                    var playerTypeSelectorPayload = {
                        playerTypeEntity: gridItem.playerTypeEntity
                    };
                    var setLoader = function (value) {
                        gridItem.isPlayerTypeSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.playerTypeSelectorAPI, playerTypeSelectorPayload, setLoader, undefined);
                };

                $scope.scopeModel.datasource.push(gridItem);
            }

            function checkDuplicateInArray(array) {
                if (array == undefined)
                    return false;

                for (var i = 0; i < array.length; i++) {
                    var item = array[i];
                    for (var j = i + 1; j < array.length; j++) {
                        var currentItem = array[j];
                        if (item.Number == currentItem.Number) {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        return directiveDefinitionObject;
    }]);