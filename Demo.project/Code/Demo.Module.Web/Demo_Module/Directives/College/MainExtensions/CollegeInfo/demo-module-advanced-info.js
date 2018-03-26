"use strict";

app.directive("demoModuleAdvancedInfo", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AdvancedCollegeInfoType($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/College/MainExtensions/CollegeInfo/Templates/AdvancedCollegeInfoTypeTemplate.html"
        };

        function AdvancedCollegeInfoType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.nbOfStudents = payload.MaxNbOfStudents;
                        $scope.nbOfRooms = payload.MaxNbOfRooms;
                        $scope.nbOfEmployees = payload.MaxNbOfEmployees;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.Entities.AdvancedInfo,Demo.Module.Entities",
                        MaxNbOfStudents: $scope.nbOfStudents,
                        MaxNbOfRooms: $scope.nbOfRooms,
                        MaxNbOfEmployees: $scope.nbOfEmployees
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);