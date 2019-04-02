"use strict";

app.directive("businessprocessVrWorkflowSpecificgroupsTaskassignees", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var specificGroupsTaskAssignees = new SpecificGroupsTaskAssignees($scope, ctrl, $attrs);
                specificGroupsTaskAssignees.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/AssignTask/Templates/VRWorkflowSpecificGroupsTaskAssigneesTemplate.html'
        };

        function SpecificGroupsTaskAssignees($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI()
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.groupIds = (payload.settings) != undefined ? payload.settings.GroupIds : undefined;
                        $scope.scopeModel.getParentVariables = payload.getParentVariables;
                        $scope.scopeModel.getWorkflowArguments = payload.getWorkflowArguments;
                        $scope.scopeModel.isVRWorkflowActivityDisabled = payload.isVRWorkflowActivityDisabled;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees.VRWorkflowSpecificGroupsTaskAssignees,  Vanrise.BusinessProcess.MainExtensions",
                        GroupIds: $scope.scopeModel.groupIds,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);