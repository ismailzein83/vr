"use strict";

app.directive("businessprocessVrWorkflowProcessinitiatorTaskassignees", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var processInitiatorTaskAssignees = new ProcessInitiatorTaskAssignees($scope, ctrl, $attrs);
                processInitiatorTaskAssignees.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: ''
        };

        function ProcessInitiatorTaskAssignees($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI()
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees.VRWorkflowProcessInitiatorTaskAssignees, Vanrise.BusinessProcess.MainExtensions",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);