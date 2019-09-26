"use strict";

app.directive("demoModulePlayerPlayertypeProfessional", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope) { 
                var ctor = new ProfessionalPlayerCtor($scope);
                ctor.initializeController();
            },
            templateUrl: "/Client/Modules/Demo_Module/Elements/Player/Directives/MainExtensions/Templates/ProfessionalPlayerTemplate.html"
        };

        function ProfessionalPlayerCtor($scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        var playerTypeEntity = payload.playerTypeEntity;
                        if (playerTypeEntity != undefined) {
                            $scope.scopeModel.yearsOfExperience = playerTypeEntity.YearsOfExperience;
                            $scope.scopeModel.salary = playerTypeEntity.Salary;
                        }
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Player.ProfessionalPlayer, Demo.Module.MainExtension",
                        YearsOfExperience: $scope.scopeModel.yearsOfExperience,
                        Salary: $scope.scopeModel.salary,
                    };
                };

                if ($scope.onReady != null)
                    $scope.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);