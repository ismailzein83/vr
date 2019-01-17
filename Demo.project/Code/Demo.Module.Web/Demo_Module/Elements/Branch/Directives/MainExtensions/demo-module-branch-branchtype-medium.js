"use strict";

app.directive("demoModuleBranchBranchtypeMedium", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new MediumBranch($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/MediumBranchTemplate.html"
        }

        function MediumBranch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineApi();
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.branchTypeEntity != undefined)
                        $scope.scopeModel.numberofBlocks = payload.branchTypeEntity.NumberOfBlocks;

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Branch.MediumBranch, Demo.Module.MainExtension", // namespace , dll
                        NumberOfBlocks: $scope.scopeModel.numberofBlocks
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
        }

        return directiveDefinitionObject;
    }]);