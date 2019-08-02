'use strict';

app.directive('businessprocessVrWorkflowactivityWhile', ['UtilsService', 'VRUIUtilsService','VRCommon_FieldTypesService',
    function (UtilsService, VRUIUtilsService, VRCommon_FieldTypesService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                dragdropsetting: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new workflowActivityIfElseCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowWhileTemplate.html'
        };

        function workflowActivityIfElseCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var activity;
            var context;
            var settings;
            var workflowContainerAPI;
            var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var conditionExpressionBuilderDirectiveAPI;
            var conditionExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

                $scope.scopeModel.onWorkflowContainerReady = function (api) {
                    workflowContainerAPI = api;
                    workflowContainerReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConditionExpressionBuilderDirectiveReady = function (api) {
                    conditionExpressionBuilderDirectiveAPI = api;
                    conditionExpressionBuilderPromiseReadyDeffered.resolve();
                };
                ctrl.getChildContext = function () {
                    var childContext = {};

                    if (context != undefined) {
                        childContext.inEditor = context.inEditor; //AS : Added for hide errors if inEditor
                        childContext.vrWorkflowId = context.vrWorkflowId;
                        childContext.getWorkflowArguments = context.getWorkflowArguments;
                        childContext.addToList = context.addToList;
                        childContext.removeFromList = context.removeFromList;
                        childContext.reserveVariableName = context.reserveVariableName;
                        childContext.reserveVariableNames = context.reserveVariableNames;
                        childContext.eraseVariableName = context.eraseVariableName;
                        childContext.isVariableNameReserved = context.isVariableNameReserved;
                        childContext.doesActivityhaveErrors = context.doesActivityhaveErrors;
                        childContext.showErrorWarningIcon = context.showErrorWarningIcon;
                        childContext.getParentVariables = function () {
                            var parentVars = [];
                            if (context.getParentVariables != undefined)
                                parentVars = parentVars.concat(context.getParentVariables());
                            //if (variables != undefined)
                            //	parentVars = parentVars.concat(variables);
                            return parentVars;
                        };
                    }
                    return childContext;
                };

                defineAPI();
            }
            function loadConditionExpressionBuilder() {

                var conditionExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                conditionExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.Condition : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(conditionExpressionBuilderDirectiveAPI, payload, conditionExpressionBuilderPromiseLoadDeffered);
                });
                return conditionExpressionBuilderPromiseLoadDeffered.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        settings = payload.Settings;
                        if (settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = settings.IsDisabled;

                            $scope.scopeModel.condition = settings.Condition;
                            $scope.scopeModel.conditionDescription = settings.ConditionDescription;
                            activity = settings.Activity;
                        }
                        if (payload.Context != undefined) {
                            context = payload.Context;
                        }
                        promises.push(loadConditionExpressionBuilder());

                    }

                    var loadWorkflowContainerPromise = loadWorkflowContainer();
                    promises.push(loadWorkflowContainerPromise);


                    function loadWorkflowContainer() {
                        var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

                        workflowContainerReadyPromiseDeferred.promise.then(function () {

                            if (activity == undefined) {
                                activity = {
                                    VRWorkflowActivityId: UtilsService.guid(),
                                    Settings: {
                                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSequenceActivity, Vanrise.BusinessProcess.MainExtensions",
                                        Editor: "businessprocess-vr-workflowactivity-sequence",
                                        Title: "Sequence"
                                    }
                                };
                            }

                            var workflowContainerPayload = {
                                vRWorkflowActivity: activity,
                                getChildContext: ctrl.getChildContext
                            };
                            VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, workflowContainerPayload, workflowContainerLoadDeferred);
                        });

                        return workflowContainerLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowWhileActivity, Vanrise.BusinessProcess.MainExtensions",
                        Condition: conditionExpressionBuilderDirectiveAPI != undefined ? conditionExpressionBuilderDirectiveAPI.getData() : undefined,
                        Activity: workflowContainerAPI != undefined ? workflowContainerAPI.getData() : undefined,
                        ConditionDescription: $scope.scopeModel.conditionDescription
                    };
                };

                api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
                    $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;

                    if (workflowContainerAPI != undefined && workflowContainerAPI.changeActivityStatus != undefined)
                        workflowContainerAPI.changeActivityStatus(isVRWorkflowActivityDisabled);
                };

                api.getActivityStatus = function () {
                    return $scope.scopeModel.isVRWorkflowActivityDisabled;
                };

                api.isActivityValid = function () {
                    if (conditionExpressionBuilderDirectiveAPI == undefined || conditionExpressionBuilderDirectiveAPI.getData() == undefined)
                        return false;

                    var result = true;

                    if (workflowContainerAPI != undefined && workflowContainerAPI.isActivityValid != undefined)
                        result = result && workflowContainerAPI.isActivityValid();

                    return result;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);