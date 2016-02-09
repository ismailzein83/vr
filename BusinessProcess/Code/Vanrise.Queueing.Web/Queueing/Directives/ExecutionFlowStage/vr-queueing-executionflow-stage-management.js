"use strict";

app.directive("vrQueueingExecutionflowStageManagement", ["UtilsService", "VRNotificationService", "VR_Queueing_ExecutionFlowDefinitionService",
function (UtilsService, VRNotificationService, VR_Queueing_ExecutionFlowDefinitionService) {

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
                        addNeededFields(executionFlowStage);
                        ctrl.datasource.push(executionFlowStage);
                    }

                    VR_Queueing_ExecutionFlowDefinitionService.addExecutionFlowStage(onExecutionFlowStageAdded, ctrl.datasource);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fields;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        fields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            fields.push({
                                StageName: ctrl.datasource[i].Name,
                                QueueTemplateName: ctrl.datasource[i].Type,
                                QueueTemplateTitle: ctrl.datasource[i].Type
                            });
                        }

                    }
                    var obj = {
                        Fields: fields,
                    }
                    return obj;
                }

                api.load = function (payload) {

                    return VR_Queueing_ExecutionFlowDefinitionService.GetDataRecordFieldTypes().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.fieldTypeConfigs.push(item);
                        });
                        if (payload != undefined) {
                            if (payload.Fields && payload.Fields.length > 0) {
                                for (var i = 0; i < payload.Fields.length; i++) {
                                    var dataItem = payload.Fields[i];
                                    addNeededFields(dataItem);
                                    ctrl.datasource.push(dataItem);
                                }
                            }
                        }
                    });
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededFields(dataItem) {
                var template = UtilsService.getItemByVal(ctrl.fieldTypeConfigs, dataItem.Type.ConfigId, "DataRecordFieldTypeConfigId");
                dataItem.TypeDescription = template != undefined ? template.Name : "";
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
                }
            }

            function editExecutionFlowStage(executionFlowStagedObj) {
                var onDataRecordFieldUpdated = function (executionFlowStagedObj) {
                    addNeededFields(executionFlowStagedObj);
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, executionFlowStagedObj.Name, 'Name');
                    ctrl.datasource[index] = executionFlowStagedObj;
                }

                VR_Queueing_ExecutionFlowDefinitionService.editExecutionFlowStage(dataRecordFieldObj, onExecutionFlowStageUpdated, ctrl.datasource);
            }

            function deleteExecutionFlowStage(executionFlowStagedObj) {
                var onDataRecordFieldDeleted = function (executionFlowStagedObj) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, executionFlowStagedObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                VR_Queueing_ExecutionFlowDefinitionService.deleteExecutionFlowStage($scope, executionFlowStagedObj, onExecutionFlowStageDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);