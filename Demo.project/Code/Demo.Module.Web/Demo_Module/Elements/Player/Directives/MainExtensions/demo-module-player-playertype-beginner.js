"use strict";

app.directive("demoModulePlayerPlayertypeBeginner", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope) {
                var ctor = new BeginnerPlayerCtor($scope);
                ctor.initializeController();
            },
            templateUrl: "/Client/Modules/Demo_Module/Elements/Player/Directives/MainExtensions/Templates/BeginnerPlayerTemplate.html"
        };

        function BeginnerPlayerCtor($scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Player.BeginnerPlayer, Demo.Module.MainExtension"
                    };
                };

                if ($scope.onReady != null)
                    $scope.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);