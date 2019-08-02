"use strict";

app.directive("businessprocessVrWorkflowSpecificusersTaskassignees", ["UtilsService", "VRUIUtilsService", "VR_Sec_UserService","VRCommon_FieldTypesService",
    function (UtilsService, VRUIUtilsService, VR_Sec_UserService, VRCommon_FieldTypesService) {

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

            var userIdsExpressionBuilderDirectiveAPI;
            var userIdsExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();
            var settings;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onUserIdsExpressionBuilderDirectiveReady = function (api) {
                    userIdsExpressionBuilderDirectiveAPI = api;
                    userIdsExpressionBuilderPromiseReadyDeffered.resolve();
                };
                defineAPI()
            }
            function loadUserIdsExpressionBuilder() {

                var userIdsExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                userIdsExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.UserIds : undefined,
                        fieldEntity: {
                            fieldType: VRCommon_FieldTypesService.getArrayFieldType(VR_Sec_UserService.getUserIdFieldType()),
                            fieldTitle: "Users"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(userIdsExpressionBuilderDirectiveAPI, payload, userIdsExpressionBuilderPromiseLoadDeffered);
                });
                return userIdsExpressionBuilderPromiseLoadDeffered.promise;
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
                    promises.push(loadUserIdsExpressionBuilder());
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflow_TaskAssignees.VRWorkflowSpecificUsersTaskAssignees,  Vanrise.BusinessProcess.MainExtensions",
                        UserIds: userIdsExpressionBuilderDirectiveAPI != undefined ? userIdsExpressionBuilderDirectiveAPI.getData() : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);