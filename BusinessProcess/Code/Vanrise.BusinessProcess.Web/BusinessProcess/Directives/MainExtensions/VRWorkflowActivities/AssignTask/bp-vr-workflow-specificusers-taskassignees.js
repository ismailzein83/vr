"use strict";

app.directive("businessprocessVrWorkflowSpecificusersTaskassignees", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var specificUsersTaskAssignees = new SpecificUsersTaskAssignees($scope, ctrl, $attrs);
                specificUsersTaskAssignees.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/AssignTask/Templates/VRWorkflowSpecificUsersTaskAssigneesTemplate.html'
        };

        function SpecificUsersTaskAssignees($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI()
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.userIds = payload.userIds;
                        $scope.scopeModel.getParentVariables = payload.getParentVariables;
                        $scope.scopeModel.getWorkflowArguments = payload.getWorkflowArguments;
                        $scope.scopeModel.isVRWorkflowActivityDisabled = payload.isVRWorkflowActivityDisabled;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees.VRWorkflowSpecificUsersTaskAssignees,  Vanrise.BusinessProcess.MainExtensions",
                        UserIds: $scope.scopeModel.userIds,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);