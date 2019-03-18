"use strict";

app.directive("bpVrWorkflowManualexeceditor", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualExecEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowManualExecEditorTemplate.html"
        };

        function ManualExecEditor(ctrl, $scope, $attrs) {
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

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
