'use strict';

app.directive('businessprocessVrWorkflowactivityWhile', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

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

	        var workflowContainerAPI;
	        var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
	            $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

	            $scope.scopeModel.onWorkflowContainerReady = function (api) {
	                workflowContainerAPI = api;
	                workflowContainerReadyPromiseDeferred.resolve();
	            };

	            ctrl.getChildContext = function () {
	                var childContext = {};

	                if (context != undefined) {
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

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                if (payload != undefined) {
	                    if (payload.Settings != undefined) {
	                        $scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;

	                        $scope.scopeModel.condition = payload.Settings.Condition;
	                        activity = payload.Settings.Activity;
	                    }
	                    if (payload.Context != undefined) {
	                        context = payload.Context;
	                        $scope.scopeModel.context = context;
	                    }
	                }

	                var promises = [];

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
                        Condition: $scope.scopeModel.condition,
                        Activity: workflowContainerAPI != undefined ? workflowContainerAPI.getData() : undefined,
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
	                if ($scope.scopeModel.condition == undefined)
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