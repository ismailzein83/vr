'use strict';

app.directive('businessprocessVrWorkflowactivityAssigntask', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPTaskTypeAPIService', 'BusinessProcess_VRWorkflowService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_BPTaskTypeAPIService, BusinessProcess_VRWorkflowService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WorkflowAssignTask(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowAssignTaskTemplate.html'
        };

        function WorkflowAssignTask(ctrl, $scope, $attrs) {

            var taskTypeId;
            var taskTitle;
            var taskAssignees;
            var displayName;
            var inputItems = [];
            var outputItems = [];
            var context;
            var isNew;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var editModeAction = {
                        name: "Edit",
                        clicked: openActivityEditor
                    };

                    if (payload != undefined) {
                        if (payload.Settings != undefined) {
                            isNew = payload.Settings.IsNew;
                            taskTypeId = payload.Settings.TaskTypeId;
                            taskTitle = payload.Settings.TaskTitle;
                            displayName = payload.Settings.DisplayName;
                            $scope.scopeModel.displayName = displayName;
                            taskAssignees = payload.Settings.TaskAssignees;
                            inputItems = payload.Settings.InputItems;
                            outputItems = payload.Settings.OutputItems;
                        }

                        if (payload.Context != null)
                            context = payload.Context;

                        if (payload.SetMenuAction != undefined)
                            payload.SetMenuAction(editModeAction);

                        if (isNew) {
                            openActivityEditor();
                        }
                    }

                    function openActivityEditor() {
                        var onActivityUpdated = function (updatedObject) {
                            console.log(updatedObject);
                            $scope.scopeModel.displayName = updatedObject.displayName;
                            taskTypeId = updatedObject.taskTypeId;
                            taskTitle = updatedObject.taskTitle;
                            taskAssignees = updatedObject.taskAssignees;
                            displayName = updatedObject.displayName;
                            inputItems = updatedObject.inputItems;
                            outputItems = updatedObject.outputItems;
                            isNew = false;
                        };

                        BusinessProcess_VRWorkflowService.openAssignTaskEditor(buildObjectFromScope(), context, onActivityUpdated, ctrl.remove, isNew);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    console.log(buildObjectFromScope());
                    return buildObjectFromScope();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildObjectFromScope() {
                return {
                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowAssignTaskActivity, Vanrise.BusinessProcess.MainExtensions",
                    TaskTypeId: taskTypeId,
                    TaskTitle: taskTitle,
                    TaskAssignees: taskAssignees,
                    DisplayName: displayName,
                    InputItems: inputItems,
                    OutputItems: outputItems,
                };
            }
        }
        return directiveDefinitionObject;
    }]);