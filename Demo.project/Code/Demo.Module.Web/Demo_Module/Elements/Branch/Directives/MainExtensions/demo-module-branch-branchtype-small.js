"use strict"

app.directive("demoModuleBranchBranchtypeSmall", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SmallBranch($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/SmallBranchTemplate.html"
        }

        function SmallBranch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineApi();
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.branchTypeEntity != undefined)
                        $scope.scopeModel.numberOfRooms = payload.branchTypeEntity.NumberOfRooms;

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Branch.SmallBranch, Demo.Module.MainExtension",
                        NumberOfRooms: $scope.scopeModel.numberOfRooms
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
])