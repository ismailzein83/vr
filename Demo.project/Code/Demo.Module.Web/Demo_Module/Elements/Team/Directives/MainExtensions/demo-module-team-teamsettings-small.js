"use strict";

app.directive("demoModuleTeamTeamsettingsSmall", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope) {
                var ctor = new SmallTeamCtor($scope);
                ctor.initializeController();
            },

            templateUrl: "/Client/Modules/Demo_Module/Elements/Team/Directives/MainExtensions/Templates/SmallTeamTemplate.html"
        };

        function SmallTeamCtor($scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        if (payload.teamSettingsEntity != undefined) {
                            $scope.scopeModel.numberOfPlayers = payload.teamSettingsEntity.NumberOfPlayers;
                        }
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Team.SmallTeamType, Demo.Module.MainExtension",
                        NumberOfPlayers: $scope.scopeModel.numberOfPlayers
                    };
                };

                if ($scope.onReady != null)
                    $scope.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);