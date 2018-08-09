'use strict';

app.directive('businessprocessVrWorkflowactivitySequence', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '=',
	            isrequired: '=',
	            normalColNum: '@',
	            dragdropsetting: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;

	            ctrl.itemsSortable = { handle: '.handeldrag', animation: 100 };
	            ctrl.itemsSortable.sort = true;
	            if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
	                ctrl.itemsSortable.group = {
	                    name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
	                    pull: true,
	                    put: ctrl.dragdropsetting.canReceive
	                };

	                ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
	                    var itemAddedContext = (ctrl.getChildContext != undefined) ? ctrl.getChildContext() : undefined;
	                    var obj = evt.model;
	                    if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
	                        obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source, itemAddedContext);
	                    evt.models[evt.newIndex] = obj;
	                };
	            }

	            var ctor = new workflowSequence(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowSequenceTemplate.html'
	    };

	    function workflowSequence(ctrl, $scope, $attrs) {
	        this.initializeController = initializeController;

	        var context;
	        var variables;
	        var parentVariables;

	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.datasource = [];
	            $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

	            $scope.scopeModel.onRemove = function (vRWorkflowActivityId) {
	                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
	                    if ($scope.scopeModel.datasource[i].VRWorkflowActivityId == vRWorkflowActivityId) {
	                        $scope.scopeModel.datasource.splice(i, 1);
	                        break;
	                    }
	                }
	            };

	            ctrl.getChildContext = function () {
	                var childContext = { /*ParentVariables: []*/ };

	                if (context != undefined) {
	                    childContext.getWorkflowArguments = context.getWorkflowArguments;
	                    childContext.addToList = context.addToList;
	                    childContext.removeFromList = context.removeFromList;
	                    childContext.reserveVariableName = context.reserveVariableName;
	                    childContext.reserveVariableNames = context.reserveVariableNames;
	                    childContext.eraseVariableName = context.eraseVariableName;
	                    childContext.doesActivityhaveErrors = context.doesActivityhaveErrors;
	                    childContext.showErrorWarningIcon = context.showErrorWarningIcon;
	                    childContext.isVariableNameReserved = context.isVariableNameReserved;
	                    childContext.getParentVariables = function () {
	                        var parentVars = [];
	                        if (context.getParentVariables != undefined)
	                            parentVars = parentVars.concat(context.getParentVariables());
	                        if (variables != undefined)
	                            parentVars = parentVars.concat(variables);
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
	                    context = payload.Context;
	                    if (payload.Settings != undefined)
	                        variables = payload.Settings.Variables;

	                    if (context != undefined && context.reserveVariableNames != undefined && variables != undefined && variables.length > 0)
	                        context.reserveVariableNames(variables);

	                    if (payload.Settings != undefined && payload.Settings.Activities != undefined && payload.Settings.Activities.length > 0) {
	                        for (var i = 0; i < payload.Settings.Activities.length; i++) {
	                            extendDataItem(payload.Settings.Activities[i]);
	                        }
	                        $scope.scopeModel.datasource = payload.Settings.Activities;
	                    }

	                    if (payload.SetMenuAction != undefined) {
	                        payload.SetMenuAction(getVariableEditorAction());
	                        payload.SetMenuAction(getOpenEditorAction());
	                    }
	                }
	            };

	            api.getData = function () {

	                var activities = [];
	                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
	                    var item = $scope.scopeModel.datasource[i];
	                    if (item.directiveAPI != null)
	                        activities.push(item.directiveAPI.getData());
	                }
	                return {
	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSequenceActivity, Vanrise.BusinessProcess.MainExtensions",
	                    Activities: activities,
	                    Variables: variables
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }

	        function getVariableEditorAction() {
	            return {
	                name: "Variables",
	                clicked: openVariablesEditor
	            };
	        }
	        function getOpenEditorAction() {
	            return {
	                name: "Edit",
	                clicked: openActivityEditor
	            };
	        }

	        function openActivityEditor() {
	            var onActivityUpdated = function (sequenceSettings) {
	                var promises = [];
	                if (sequenceSettings == undefined || sequenceSettings.Activities == undefined || sequenceSettings.Activities.length < 1) {
	                    $scope.scopeModel.datasource = [];
	                }
	                else {
	                    for (var i = 0; i < sequenceSettings.Activities.length; i++) {
	                        extendDataItem(sequenceSettings.Activities[i]);
	                    }
	                    $scope.scopeModel.datasource = sequenceSettings.Activities;
	                }
	            };

	            var activities = [];
	            for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
	                var item = $scope.scopeModel.datasource[i];
	                if (item.directiveAPI != null)
	                    activities.push(item.directiveAPI.getData());
	            }

	            var settings = {
	                Activities: activities,
	                Variables: variables
	            };
	            BusinessProcess_VRWorkflowService.openSequenceEditor(ctrl.dragdropsetting, context, settings, onActivityUpdated);
	        }

	        function openVariablesEditor() {
	            var onSaveVariables = function (activityVariables) {
	                variables = activityVariables;
	            };

	            if (context != undefined)
	                BusinessProcess_VRWorkflowService.openVariablesEditor(onSaveVariables, variables, (context.getParentVariables != undefined) ? context.getParentVariables() : undefined, context.reserveVariableName, context.eraseVariableName, context.isVariableNameReserved, context.reserveVariableNames);
	            else
	                BusinessProcess_VRWorkflowService.openVariablesEditor(onSaveVariables, variables, undefined, undefined, undefined, undefined);
	        }
	        function extendDataItem(dataItem) {
	            dataItem.onDirectiveReady = function (api) {
	                if (dataItem.directiveAPI != null)
	                    return;

	                dataItem.directiveAPI = api;
	                var setLoader = function (value) { };
	                var directivePayload = {
	                    Context: (ctrl.getChildContext != undefined) ? ctrl.getChildContext() : undefined,
	                    VRWorkflowActivityId: dataItem.VRWorkflowActivityId,
	                    Settings: dataItem.Settings
	                };
	                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, directivePayload, setLoader);
	            };
	        }
	    }

	    return directiveDefinitionObject;
	}]);