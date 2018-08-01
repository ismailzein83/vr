'use strict';

app.directive('businessprocessVrWorkflowactivityIfelse', ['UtilsService', 'VRUIUtilsService',
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
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowIfElseTemplate.html'
	    };

	    function workflowActivityIfElseCtor(ctrl, $scope, $attrs) {
	        this.initializeController = initializeController;

	        var trueActivity, falseActivity;
	        var context;

	        var trueWorkflowContainerAPI;
	        var trueWorkflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	        var falseWorkflowContainerAPI;
	        var falseWorkflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

	            console.log(ctrl.dragdropsetting);

	            $scope.scopeModel.onTrueWorkflowContainerReady = function (api) {
	                trueWorkflowContainerAPI = api;
	                trueWorkflowContainerReadyPromiseDeferred.resolve();
	            };

	            $scope.scopeModel.onFalseWorkflowContainerReady = function (api) {
	                falseWorkflowContainerAPI = api;
	                falseWorkflowContainerReadyPromiseDeferred.resolve();
	            };

	            ctrl.getChildContext = function () {
	                var childContext = {};

	                if (context != undefined) {
	                    childContext.getWorkflowArguments = context.getWorkflowArguments;
	                    childContext.reserveVariableName = context.reserveVariableName;
	                    childContext.reserveVariableNames = context.reserveVariableNames;
	                    childContext.eraseVariableName = context.eraseVariableName;
	                    childContext.isVariableNameReserved = context.isVariableNameReserved;
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

	                console.log(payload);

	                if (payload != undefined) {
	                    if (payload.Settings != undefined && payload.Settings != undefined) {
	                        $scope.scopeModel.condition = payload.Settings.Condition;
	                        trueActivity = payload.Settings.TrueActivity;
	                        falseActivity = payload.Settings.FalseActivity;
	                    }
	                    if (payload.Context != undefined) {
	                        context = payload.Context;
	                        $scope.scopeModel.context = context;
	                    }
	                }

	                var promises = [];

	                var loadTrueWorkflowContainerPromise = loadTrueWorkflowContainer();
	                promises.push(loadTrueWorkflowContainerPromise);

	                var loadFalseWorkflowContainerPromise = loadFalseWorkflowContainer();
	                promises.push(loadFalseWorkflowContainerPromise);


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

	                        if (falseActivity == undefined) {
	                            falseActivity = {
	                                VRWorkflowActivityId: UtilsService.guid(),
	                                Settings: {
	                                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSequenceActivity, Vanrise.BusinessProcess.MainExtensions",
	                                    Editor: "businessprocess-vr-workflowactivity-sequence",
	                                    Title: "Sequence"
	                                }
	                            };
	                        }

	                        var falseWorkflowContainerPayload = {
	                            vRWorkflowActivity: falseActivity,
	                            getChildContext: ctrl.getChildContext
	                        };
	                        VRUIUtilsService.callDirectiveLoad(falseWorkflowContainerAPI, falseWorkflowContainerPayload, falseWorkflowContainerLoadDeferred);
	                    });

	                    return falseWorkflowContainerLoadDeferred.promise;
	                }

	                return UtilsService.waitMultiplePromises(promises);
	            };

	            api.getData = function () {
	                return {
	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowIfElseActivity, Vanrise.BusinessProcess.MainExtensions",
	                    Condition: $scope.scopeModel.condition,
	                    TrueActivity: trueWorkflowContainerAPI != undefined ? trueWorkflowContainerAPI.getData() : null,
	                    FalseActivity:falseWorkflowContainerAPI != undefined ? falseWorkflowContainerAPI.getData() : null
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }

	    return directiveDefinitionObject;
	}]);