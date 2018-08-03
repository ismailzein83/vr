(function (appControllers) {

	"use strict";

	BusinessProcess_VRWorkflowService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

	function BusinessProcess_VRWorkflowService(VRModalService, VRCommon_ObjectTrackingService) {
		var drillDownDefinitions = [];

		function addVRWorkflow(onVRWorkflowAdded) {

			var modalParameters = {

			};

			var modalSettings = {};
			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowAdded = onVRWorkflowAdded;
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/VRWorkflowEditor.html', modalParameters, modalSettings);
		}
		function editVRWorkflow(vrWorkflowId, onVRWorkflowUpdated) {
			var settings = {

			};
			settings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowUpdated = onVRWorkflowUpdated;
			};

			var parameters = {
				vrWorkflowId: vrWorkflowId
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/VRWorkflowEditor.html', parameters, settings);
		}

		function addVRWorkflowArgument(vrWorkflowArgumentNames, onVRWorkflowArgumentAdded, isVariableNameReserved) {

			var modalSettings = {};
			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowArgumentAdded = onVRWorkflowArgumentAdded;
				modalScope.isVariableNameReserved = isVariableNameReserved;
			};

			var modalParameters = {
				vrWorkflowArgumentNames: vrWorkflowArgumentNames
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Arguments/Templates/VRWorkflowArgumentEditor.html', modalParameters, modalSettings);
		}
		function editVRWorkflowArgument(vrWorkflowArgumentEntity, vrWorkflowArgumentNames, onVRWorkflowArgumentUpdated, isVariableNameReserved) {

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowArgumentUpdated = onVRWorkflowArgumentUpdated;
				modalScope.isVariableNameReserved = isVariableNameReserved;
			};

			var parameters = {
				vrWorkflowArgumentEntity: vrWorkflowArgumentEntity,
				vrWorkflowArgumentNames: vrWorkflowArgumentNames
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Arguments/Templates/VRWorkflowArgumentEditor.html', parameters, settings);
		}

		function addVRWorkflowVariable(onVRWorkflowVariableAdded, isVariableNameReserved) {

			var modalSettings = {};
			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowVariableAdded = onVRWorkflowVariableAdded;
				modalScope.isVariableNameReserved = isVariableNameReserved;
			};

			var modalParameters = {
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Variables/Templates/VRWorkflowVariableEditor.html', modalParameters, modalSettings);
		}
		function editVRWorkflowVariable(vrWorkflowVariableEntity, onVRWorkflowVariableUpdated) {

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowVariableUpdated = onVRWorkflowVariableUpdated;
			};

			var parameters = {
				vrWorkflowVariableEntity: vrWorkflowVariableEntity
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Variables/Templates/VRWorkflowVariableEditor.html', parameters, settings);
		}

		function registerObjectTrackingDrillDownToVRWorkflow() {
			var drillDownDefinition = {};

			drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
			drillDownDefinition.directive = "vr-common-objecttracking-grid";

			drillDownDefinition.loadDirective = function (directiveAPI, vrWorkflow) {
				vrWorkflow.objectTrackingGridAPI = directiveAPI;

				var query = {
					ObjectId: vrWorkflow.VRWorkflowID,
					EntityUniqueName: getEntityUniqueName()

				};
				return vrWorkflow.objectTrackingGridAPI.load(query);
			};

			addDrillDownDefinition(drillDownDefinition);
		}
		function addDrillDownDefinition(drillDownDefinition) {
			drillDownDefinitions.push(drillDownDefinition);
		}
		function getEntityUniqueName() {
			return "BusinessProcess_VR_Workflow";
		}

		function getDrillDownDefinition() {
			return drillDownDefinitions;
		}

		function openExpressionBuilder(onSetExpressionBuilder, activityArguments, activityVariables, expressionBuilderValue) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onSetExpressionBuilder = onSetExpressionBuilder;
			};
			var parameter = {
				Arguments: activityArguments,
				Variables: activityVariables
			};
			if (expressionBuilderValue != undefined)
				parameter.ExpressionValue = expressionBuilderValue;
			VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/ExpressionBuilderEditor.html', parameter, modalSettings);
		}

		function openVariablesEditor(onSaveVariables, variables, parentVariables, reserveVariableName, eraseVariableName, isVariableNameReserved, reserveVariableNames) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onSaveVariables = onSaveVariables;
				modalScope.reserveVariableName = reserveVariableName;
				modalScope.eraseVariableName = eraseVariableName;
				modalScope.isVariableNameReserved = isVariableNameReserved;
				modalScope.reserveVariableNames = reserveVariableNames;
			};
			var parameter = {
				Variables: variables,
				ParentVariables: parentVariables
			};
			VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/ActivityVariablesEditor.html', parameter, modalSettings);
		}

		function openForeachEditor(dragdropsetting, foreachObj, getChildContext, context, onActivityUpdated, remove) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onActivityUpdated = onActivityUpdated;
				modalScope.remove = remove;
			};
			var parameter = {
				dragdropsetting: dragdropsetting,
				getChildContext: getChildContext,
				context: context,
				foreachObj: foreachObj
			};
			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityForEachEditor.html', parameter, modalSettings);
		}

		function openSequenceEditor(dragdropsetting, context, settings, onActivityUpdated) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onActivityUpdated = onActivityUpdated;
			};
			var parameter = {
				dragdropsetting: dragdropsetting,
				context: context,
				settings: settings
			};
			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivitySequenceEditor.html', parameter, modalSettings);
		}

		function openCallHttpServiceEditor(obj, context, onActivityUpdated, remove) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onActivityUpdated = onActivityUpdated;
				modalScope.remove = remove;
			};
			var parameter = {
				obj: obj,
				context: context
			};
			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityCallHttpServiceEditor.html', parameter, modalSettings);
		}

		function tryCompilationResult(errorMessages, workflowObj) {
			var modalSettings = {};
			var modalParameters = {
				errorMessages: errorMessages,
				workflowObj: workflowObj
			};
			modalSettings.onScopeReady = function (modalScope) {
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/VRWorkflowCompilationResult.html', modalParameters, modalSettings);
		}

		return ({
			addVRWorkflow: addVRWorkflow,
			editVRWorkflow: editVRWorkflow,
			addVRWorkflowArgument: addVRWorkflowArgument,
			editVRWorkflowArgument: editVRWorkflowArgument,
			addVRWorkflowVariable: addVRWorkflowVariable,
			editVRWorkflowVariable: editVRWorkflowVariable,
			registerObjectTrackingDrillDownToVRWorkflow: registerObjectTrackingDrillDownToVRWorkflow,
			getDrillDownDefinition: getDrillDownDefinition,
			openExpressionBuilder: openExpressionBuilder,
			openVariablesEditor: openVariablesEditor,
			tryCompilationResult: tryCompilationResult,
			openForeachEditor: openForeachEditor,
			openSequenceEditor: openSequenceEditor,
			openCallHttpServiceEditor: openCallHttpServiceEditor
		});
	}

	appControllers.service('BusinessProcess_VRWorkflowService', BusinessProcess_VRWorkflowService);
})(appControllers);