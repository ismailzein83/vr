appControllers.directive("vanriseToolsGeneratedScriptDesignGrid", ["UtilsService", "VRNotificationService", "VR_Tools_GeneratedScriptService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, VR_Tools_GeneratedScriptService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
    "use strict";

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
        templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/GeneratedScript/Design/Templates/GeneratedScriptDesignGridTemplate.html"
    };

    function GeneratedScriptDesignGrid($scope, ctrl) {
    

        var gridApi;
        var addText;
        var generatedScripts;

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
                        var promises = [];
                        $scope.scopeModel.designs = [];
                        if (payload != undefined && payload.designs != undefined) {

                            for (var i = 0; i < payload.designs.length; i++) {
                                $scope.scopeModel.designs.push(payload.designs[i]);
                            }
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    };

                    directiveApi.getData = function () {
                        return $scope.scopeModel.designs;

                    };
                    return directiveApi;
                }
            };


            $scope.scopeModel.onGeneratedScriptDesignAdded = function () {
                var onGeneratedScriptDesignAdded = function (design) {
                    $scope.scopeModel.designs.push(design);
                };

                VR_Tools_GeneratedScriptService.addGeneratedScriptDesign(onGeneratedScriptDesignAdded);
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

            VR_Tools_GeneratedScriptService.editGeneratedScriptDesign(onGeneratedScriptDesignUpdated, Design);
        }

    }

    return directiveDefinitionObject;

}]);
