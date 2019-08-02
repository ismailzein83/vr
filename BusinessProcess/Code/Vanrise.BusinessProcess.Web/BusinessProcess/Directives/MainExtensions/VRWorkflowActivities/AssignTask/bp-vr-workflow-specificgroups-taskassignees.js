"use strict";

app.directive("businessprocessVrWorkflowSpecificgroupsTaskassignees", ["UtilsService", "VRUIUtilsService", "VR_Sec_GroupService","VRCommon_FieldTypesService",
    function (UtilsService, VRUIUtilsService, VR_Sec_GroupService, VRCommon_FieldTypesService) {

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

            var groupIdsExpressionBuilderDirectiveAPI;
            var groupIdsExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onGroupIdsExpressionBuilderDirectiveReady = function (api) {
                    groupIdsExpressionBuilderDirectiveAPI = api;
                    groupIdsExpressionBuilderPromiseReadyDeffered.resolve();
                };
                defineAPI()
            }

            function loadGroupIdsExpressionBuilder() {

                var groupIdsExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                groupIdsExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.GroupIds : undefined,
                        fieldEntity: {
                            fieldType: VRCommon_FieldTypesService.getArrayFieldType(VR_Sec_GroupService.getGroupIdFieldType()),
                            fieldTitle:"Groups"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(groupIdsExpressionBuilderDirectiveAPI, payload, groupIdsExpressionBuilderPromiseLoadDeffered);
                });
                return groupIdsExpressionBuilderPromiseLoadDeffered.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        settings = payload.settings;
                        context = {
                            getParentVariables: payload.getParentVariables,
                            getWorkflowArguments: payload.getWorkflowArguments
                        };
                        $scope.scopeModel.isVRWorkflowActivityDisabled = payload.isVRWorkflowActivityDisabled;
                    }
                    promises.push(loadGroupIdsExpressionBuilder());
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees.VRWorkflowSpecificGroupsTaskAssignees,  Vanrise.BusinessProcess.MainExtensions",
                        GroupIds: groupIdsExpressionBuilderDirectiveAPI != undefined ? groupIdsExpressionBuilderDirectiveAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);