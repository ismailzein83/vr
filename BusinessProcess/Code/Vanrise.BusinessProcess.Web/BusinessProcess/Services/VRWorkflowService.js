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

		function addVRWorkflowArgument(vrWorkflowArgumentNames, onVRWorkflowArgumentAdded) {

			var modalSettings = {};
			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowArgumentAdded = onVRWorkflowArgumentAdded;
			};

			var modalParameters = {
				vrWorkflowArgumentNames: vrWorkflowArgumentNames
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowArgumentEditor.html', modalParameters, modalSettings);
		}
		function editVRWorkflowArgument(vrWorkflowArgumentEntity, vrWorkflowArgumentNames, onVRWorkflowArgumentUpdated) {

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onVRWorkflowArgumentUpdated = onVRWorkflowArgumentUpdated;
			};

			var parameters = {
				vrWorkflowArgumentEntity: vrWorkflowArgumentEntity,
				vrWorkflowArgumentNames: vrWorkflowArgumentNames
			};

			VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowArgumentEditor.html', parameters, settings);
		}

		function registerObjectTrackingDrillDownToVRWorkflow() {
			var drillDownDefinition = {};

			drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
			drillDownDefinition.directive = "vr-common-objecttracking-grid";

			drillDownDefinition.loadDirective = function (directiveAPI, vrWorkflow) {
				vrWorkflow.objectTrackingGridAPI = directiveAPI;

				var query = {
					ObjectId: vrWorkflow.VRWorkflowID,
					EntityUniqueName: getEntityUniqueName(),

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

		return ({
			addVRWorkflow: addVRWorkflow,
			editVRWorkflow: editVRWorkflow,
			addVRWorkflowArgument: addVRWorkflowArgument,
			editVRWorkflowArgument: editVRWorkflowArgument,
			registerObjectTrackingDrillDownToVRWorkflow: registerObjectTrackingDrillDownToVRWorkflow,
			getDrillDownDefinition: getDrillDownDefinition,
			openExpressionBuilder: openExpressionBuilder
		});
	}

	appControllers.service('BusinessProcess_VRWorkflowService', BusinessProcess_VRWorkflowService);
})(appControllers);