'use strict';

app.directive('businessprocessVrWorkflowactivityIfelse', ['UtilsService', 'VRUIUtilsService','VRCommon_FieldTypesService',
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
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowIfElseTemplate.html'
        };

        function workflowActivityIfElseCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var trueActivity, falseActivity;
            var context;
            var settings;
            var trueWorkflowContainerAPI;
            var trueWorkflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var falseWorkflowContainerAPI;
            var falseWorkflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var conditionExpressionBuilderDirectiveAPI;
            var conditionExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;
                $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

                $scope.scopeModel.onTrueWorkflowContainerReady = function (api) {
                    trueWorkflowContainerAPI = api;
                    trueWorkflowContainerReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFalseWorkflowContainerReady = function (api) {
                    falseWorkflowContainerAPI = api;
                    falseWorkflowContainerReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConditionExpressionBuilderDirectiveReady = function (api) {
                    conditionExpressionBuilderDirectiveAPI = api;
                    conditionExpressionBuilderPromiseReadyDeffered.resolve();
                };
                ctrl.getChildContext = function () {
                    var childContext = {};

                    if (context != undefined) {
                        childContext.inEditor = context.inEditor;  //AS : Added for hide errors if inEditor
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
                        value: settings != undefined ? settings.Condition : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(conditionExpressionBuilderDirectiveAPI, payload, conditionExpressionBuilderPromiseLoadDeffered);
                });
                return conditionExpressionBuilderPromiseLoadDeffered.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.Context;
                        settings = payload.Settings;
                        if (settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = settings.IsDisabled;

                            $scope.scopeModel.conditionDescription = settings.ConditionDescription;
                            trueActivity = settings.TrueActivity;
                            falseActivity = settings.FalseActivity;
                        }
                    }

                    var promises = [];

                    var loadTrueWorkflowContainerPromise = loadTrueWorkflowContainer();
                    promises.push(loadTrueWorkflowContainerPromise);

                    var loadFalseWorkflowContainerPromise = loadFalseWorkflowContainer();
                    promises.push(loadFalseWorkflowContainerPromise);
                    promises.push(loadConditionExpressionBuilder());

                    function loadTrueWorkflowContainer() {
                        var trueWorkflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

                        trueWorkflowContainerReadyPromiseDeferred.promise.then(function () {

                            if (trueActivity == undefined) {
                                trueActivity = {
                                    VRWorkflowActivityId: UtilsService.guid(),
                                    Settings: {
                                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSequenceActivity, Vanrise.BusinessProcess.MainExtensions",
                                        Editor: "businessprocess-vr-workflowactivity-sequence",
                                        Title: "Sequence"
                                    }
                                };
                            }

                            var trueWorkflowContainerPayload = {
                                vRWorkflowActivity: trueActivity,
                                getChildContext: ctrl.getChildContext
                            };
                            VRUIUtilsService.callDirectiveLoad(trueWorkflowContainerAPI, trueWorkflowContainerPayload, trueWorkflowContainerLoadDeferred);
                        });

                        return trueWorkflowContainerLoadDeferred.promise;
                    }

                    function loadFalseWorkflowContainer() {
                        var falseWorkflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

                        falseWorkflowContainerReadyPromiseDeferred.promise.then(function () {

                            if (falseActivity != undefined) {
                                var falseWorkflowContainerPayload = {
                                    vRWorkflowActivity: falseActivity,
                                    getChildContext: ctrl.getChildContext
                                };
                                VRUIUtilsService.callDirectiveLoad(falseWorkflowContainerAPI, falseWorkflowContainerPayload, falseWorkflowContainerLoadDeferred);
                            }
                            else {
                                falseWorkflowContainerLoadDeferred.resolve();
                            }


                        });

                        return falseWorkflowContainerLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowIfElseActivity, Vanrise.BusinessProcess.MainExtensions",
                        Condition: conditionExpressionBuilderDirectiveAPI != undefined ? conditionExpressionBuilderDirectiveAPI.getData() : undefined,
                        ConditionDescription: $scope.scopeModel.conditionDescription,
                        TrueActivity: trueWorkflowContainerAPI != undefined ? trueWorkflowContainerAPI.getData() : null,
                        FalseActivity: falseWorkflowContainerAPI != undefined ? falseWorkflowContainerAPI.getData() : null
                    };
                };

                api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
                    $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
                    if (trueWorkflowContainerAPI != undefined && trueWorkflowContainerAPI.changeActivityStatus != undefined)
                        trueWorkflowContainerAPI.changeActivityStatus(isVRWorkflowActivityDisabled);

                    if (falseWorkflowContainerAPI != undefined && falseWorkflowContainerAPI.changeActivityStatus != undefined)
                        falseWorkflowContainerAPI.changeActivityStatus(isVRWorkflowActivityDisabled);
                };

                api.getActivityStatus = function () {
                    return $scope.scopeModel.isVRWorkflowActivityDisabled;
                };

                api.isActivityValid = function () {
                    if (conditionExpressionBuilderDirectiveAPI == undefined || conditionExpressionBuilderDirectiveAPI.getData() == undefined)
                        return false;

                    var result = true;
                    if (trueWorkflowContainerAPI != undefined && trueWorkflowContainerAPI.isActivityValid != undefined)
                        result = result && trueWorkflowContainerAPI.isActivityValid();

                    if (falseWorkflowContainerAPI != undefined && falseWorkflowContainerAPI.isActivityValid != undefined)
                        result = result && falseWorkflowContainerAPI.isActivityValid();

                    return result;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);