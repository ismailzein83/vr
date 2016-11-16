"use strict";

app.directive("vrQueueingExecutionflowStageManagement", ["UtilsService", "VRNotificationService", "VR_Queueing_ExecutionFlowStageService",
function (UtilsService, VRNotificationService, VR_Queueing_ExecutionFlowStageService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope:
        {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var executionFlowStageManagement = new ExecutionFlowStageManagement($scope, ctrl, $attrs);
            executionFlowStageManagement.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Queueing/Directives/ExecutionFlowStage/Templates/ExecutionFlowStageManagement.html"

    };

    function ExecutionFlowStageManagement($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.datasource = [];
            ctrl.fieldTypeConfigs = [];

            ctrl.addExecutionFlowStage = function () {
                var onExecutionFlowStageAdded = function (executionFlowStage) {

                    ctrl.datasource.push(executionFlowStage);
                };
                VR_Queueing_ExecutionFlowStageService.addExecutionFlowStage(onExecutionFlowStageAdded, ctrl.datasource);
            };

            defineMenuActions();
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var stages;
                var stage;
                if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                    stages = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        stage = ctrl.datasource[i];
                        stages.push({
                            StageName: stage.StageName,
                            QueueNameTemplate: stage.QueueNameTemplate,
                            QueueTitleTemplate: stage.QueueTitleTemplate,
                            MaximumConcurrentReaders: stage.MaximumConcurrentReaders,
                            QueueItemType: stage.QueueItemType,
                            QueueActivator: stage.QueueActivator,
                            SourceStages: stage.SourceStages
                        });
                    }

                }

                var obj = {
                    Stages: stages,
                };
                return obj;
            };

            api.load = function (payload) {


                if (payload != undefined) {
                    if (payload.Stages && payload.Stages.length > 0) {
                        for (var i = 0; i < payload.Stages.length; i++) {
                            var dataItem = payload.Stages[i];
                            ctrl.datasource.push(dataItem);
                        }
                    }
                }

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }



        function defineMenuActions() {
            var defaultMenuActions = [
            {
                name: "Edit",
                clicked: editExecutionFlowStage,
            },
            {
                name: "Delete",
                clicked: deleteExecutionFlowStage,
            }];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            };
        }

        function editExecutionFlowStage(executionFlowStagedObj) {
            var onExecutionFlowStageUpdated = function (executionFlowStageObj) {

                var index = UtilsService.getItemIndexByVal(ctrl.datasource, executionFlowStagedObj.StageName, 'StageName');
                ctrl.datasource[index] = executionFlowStageObj;
            };

            VR_Queueing_ExecutionFlowStageService.editExecutionFlowStage(executionFlowStagedObj, onExecutionFlowStageUpdated, ctrl.datasource);
        }

        function deleteExecutionFlowStage(executionFlowStagedObj) {
            var onExecutionFlowStageDeleted = function (executionFlowStaged) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, executionFlowStagedObj.StageName, 'StageName');
                ctrl.datasource.splice(index, 1);
            };

            VR_Queueing_ExecutionFlowStageService.deleteExecutionFlowStage($scope, executionFlowStagedObj, onExecutionFlowStageDeleted);
        }

    }

    return directiveDefinitionObject;

}
]);