"use strict";

app.directive("businessprocessVrWorkflowActivityDirective", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService) {
	    var directiveDefinitionObject = {
	        restrict: "E",
	        scope: {
	            dragdropsetting: '=',
	            onRemove: '=',
	            onReady: "="
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;

	            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
	            directiveConstructor.initializeController();
	        },
	        controllerAs: "ctrl",
	        bindToController: true,
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityDirectiveTemplate.html'
	    };

	    function DirectiveConstructor($scope, ctrl) {
	        var vRWorkflowActivityId;
	        var vRWorkflowActivitySettings;
	        var context;

	        var directiveWraperAPI;
	        var directiveWraperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	        var actions = [];

	        var enableAction = {
	            name: "Enable",
	            clicked: enableActivity
	        };

	        function enableActivity() {
	            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
	            $scope.scopeModel.actions.length = 0;
	            $scope.scopeModel.actions.push(disableAction);
	            if (actions != undefined && actions.length > 0)
	                for (var x = 0; x < actions.length; x++) {
	                    $scope.scopeModel.actions.push(actions[x]);
	                }
	            if (directiveWraperAPI != undefined && directiveWraperAPI.changeActivityStatus != undefined)
	                directiveWraperAPI.changeActivityStatus(false);
	        }

	        var disableAction = {
	            name: "Disable",
	            clicked: disableActivity
	        };

	        function disableActivity() {
	            if (directiveWraperAPI != undefined && directiveWraperAPI.changeActivityStatus != undefined)
	                directiveWraperAPI.changeActivityStatus(true);

	            $scope.scopeModel.isVRWorkflowActivityDisabled = true;
	            $scope.scopeModel.actions.length = 0;
	            $scope.scopeModel.actions.push(enableAction);
	        }

	        this.initializeController = initializeController;

	        function showErrorWarningIcon(value) {
	            $scope.scopeModel.hasError = value;
	            if (context != undefined && context.showErrorWarningIcon != undefined)
	                context.showErrorWarningIcon(value);
	        }

	        function initializeController() {
	            $scope.scopeModel = { isVRWorkflowActivityDisabled: false };
	            $scope.scopeModel.actions = [];
	            $scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

	            $scope.scopeModel.hasErrorsValidator = function () {
	                if (context != undefined && (context.inEditor == undefined || context.inEditor == false) && vRWorkflowActivityId != undefined) {
	                    var errors = context.doesActivityhaveErrors(vRWorkflowActivityId);
	                    if (errors != undefined) {
	                        if (context != undefined && context.showErrorWarningIcon != undefined)
	                            context.showErrorWarningIcon(true);
	                        var errorMessage = '';
	                        for (var i = 0; i < errors.length; i++) {
	                            errorMessage += (i + 1) + ') ' + errors[i] + '\n';
	                        }
	                        return errorMessage;
	                    }
	                }
	                return null;
	            };
	            $scope.scopeModel.onDirectiveWraperReady = function (api) {
	                directiveWraperAPI = api;
	                directiveWraperReadyPromiseDeferred.resolve();
	            };

	            defineAPI();
	        }

	        function resetErrorsFunction() {
	            $scope.scopeModel.hasError = false;
	        }
	        function openActivityEditor() { }
	        function defineAPI() {

	            var api = {};
	            api.load = function (payload) {

	                var setMenuAction = function (menuAction) {
	                    actions.push(menuAction);
	                    $scope.scopeModel.actions.push(menuAction);
	                };
	                var loadPromiseDeferred = UtilsService.createPromiseDeferred();

	                if (payload != undefined && payload.Settings != null) {
	                    $scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;
	                    if (payload.Settings.IsDisabled)
	                        $scope.scopeModel.actions.push(enableAction);
	                    else
	                        $scope.scopeModel.actions.push(disableAction);

	                    context = payload.Context;
	                    vRWorkflowActivityId = payload.VRWorkflowActivityId;
	                    $scope.scopeModel.onRemove = function () {
	                        ctrl.onRemove(payload.VRWorkflowActivityId);
	                        if (context != undefined && context.removeFromList != undefined) {

	                            context.removeFromList(vRWorkflowActivityId);
	                        }
	                    };
	                    $scope.scopeModel.editor = payload.Settings.Editor;
	                    $scope.scopeModel.header = payload.Settings.Title;

	                    vRWorkflowActivitySettings = payload.Settings;
	                    if (context != undefined) {
	                        if (context.addToList != undefined)
	                            context.addToList(vRWorkflowActivityId, resetErrorsFunction);
	                    }

	                    var childContext = {};
	                    childContext.showErrorWarningIcon = showErrorWarningIcon;
	                    if (context != undefined) {
	                        childContext.inEditor = context.inEditor;
	                        childContext.vrWorkflowId = context.vrWorkflowId;
	                        childContext.getWorkflowArguments = context.getWorkflowArguments;
	                        childContext.addToList = context.addToList;
	                        childContext.removeFromList = context.removeFromList;
	                        childContext.reserveVariableName = context.reserveVariableName;
	                        childContext.reserveVariableNames = context.reserveVariableNames;
	                        childContext.eraseVariableName = context.eraseVariableName;
	                        childContext.isVariableNameReserved = context.isVariableNameReserved;
	                        childContext.doesActivityhaveErrors = context.doesActivityhaveErrors;
	                        childContext.getParentVariables = function () {
	                            var parentVars = [];
	                            if (context.getParentVariables != undefined)
	                                parentVars = parentVars.concat(context.getParentVariables());
	                            return parentVars;
	                        };
	                    }
	                    directiveWraperReadyPromiseDeferred.promise.then(function () {
	                        var payload = {
	                            Context: childContext,
	                            SetMenuAction: setMenuAction,
	                            Settings: vRWorkflowActivitySettings
	                        };
	                        directiveWraperAPI.load(payload);
	                        loadPromiseDeferred.resolve();
	                    });
	                }
	                else {
	                    $scope.scopeModel.actions.push(disableAction);
	                    loadPromiseDeferred.resolve();
	                }
	                return loadPromiseDeferred.promise;
	            };

	            api.getData = function () {
	                var settings = {};
	                if (directiveWraperAPI != null)
	                    settings = directiveWraperAPI.getData();
	                settings.Title = $scope.scopeModel.header;
	                settings.Editor = $scope.scopeModel.editor;
	                settings.IsDisabled = $scope.scopeModel.isVRWorkflowActivityDisabled;

	                return {
	                    VRWorkflowActivityId: vRWorkflowActivityId,
	                    Settings: settings
	                };
	            };

	            api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
	                $scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
	                if (!isVRWorkflowActivityDisabled)
	                    enableActivity();
	                else
	                    disableActivity();
	                if (directiveWraperAPI != null && directiveWraperAPI.changeActivityStatus != undefined)
	                    directiveWraperAPI.changeActivityStatus(isVRWorkflowActivityDisabled);
	            };

	            api.getActivityStatus = function () {
	                return $scope.scopeModel.isVRWorkflowActivityDisabled;
	            };

	            api.isActivityValid = function () {
	                if (directiveWraperAPI != null && directiveWraperAPI.isActivityValid != undefined)
	                    return directiveWraperAPI.isActivityValid();

	                return true;
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }

	    }

	    return directiveDefinitionObject;
	}]);
