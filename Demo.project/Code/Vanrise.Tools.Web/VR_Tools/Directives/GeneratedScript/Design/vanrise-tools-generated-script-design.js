"use strict"
app.directive("vanriseToolsGeneratedScriptDesign", ["UtilsService", "VRNotificationService", "Vanrise_Tools_GeneratedScriptService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Vanrise_Tools_GeneratedScriptService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var generatedScriptDesignGrid = new GeneratedScriptDesignGrid($scope, ctrl, $attrs);
            generatedScriptDesignGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Vanrise_Tools/Directives/GeneratedScript/Templates/GeneratedScriptDesignGridTemplate.html"
    };

    function GeneratedScriptDesignGrid($scope, ctrl) {

        var gridApi;
        $scope.scopeModel = {};
        $scope.scopeModel.designs = [];

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {

                    var directiveApi = {};

                    directiveApi.load = function (payload) {

                    };

                    return directiveApi;
                };
            };


            $scope.scopeModel.onGeneratedScriptDesignAdded = function () {

                var onGeneratedScriptDesignAdded = function (design) {
                    $scope.scopeModel.designs.push(design);
                };

                Vanrise_Tools_GeneratedScriptService.addGeneratedScriptDesign(onGeneratedScriptDesignAdded);
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editGeneratedScriptDesign,
            }];
        }

        function editGeneratedScriptDesign(Design) {

            var index = $scope.scopeModel.designs.indexOf(Design);
            var onGeneratedScriptDesignUpdated = function (design) {
                $scope.scopeModel.designs[index] = design;
            };

            Vanrise_Tools_GeneratedScriptService.editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, Design);
        }

    }

    return directiveDefinitionObject;

}]);
