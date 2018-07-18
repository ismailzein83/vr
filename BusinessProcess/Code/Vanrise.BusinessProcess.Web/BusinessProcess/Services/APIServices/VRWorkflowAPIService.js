(function (appControllers) {

	"use strict";

	BusinessProcess_VRWorkflowAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

	function BusinessProcess_VRWorkflowAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {
		var controllerName = 'VRWorkflow';

		function GetFilteredVRWorkflows(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetFilteredVRWorkflows"), input);
		}

		function GetVRWorkflowEditorRuntime(vrWorkflowId) {
			return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowEditorRuntime"), { vrWorkflowId: vrWorkflowId });
		}

		function GetVRWorkflowVariablesTypeDescription(variables) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowVariablesTypeDescription"), variables);
		}

		function GetVRWorkflowVariableTypeExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowVariableTypeExtensionConfigs"), {});
		}

		function GetVRWorkflowActivityExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowActivityExtensionConfigs"), {});
		}

		function GetVRWorkflowArgumentTypeDescription(vrWorkflowArgumentType) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowArgumentTypeDescription"), vrWorkflowArgumentType);
		}

		function GetVRWorkflowVariableTypeDescription(vrWorkflowArgumentType) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetVRWorkflowVariableTypeDescription"), vrWorkflowArgumentType);
		}

		function InsertVRWorkflow(vrWorkflow) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "InsertVRWorkflow"), vrWorkflow);
		}

		function TryCompileWorkflow(vrWorkflow) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "TryCompileWorkflow"), vrWorkflow);
		}

		function ExportCompilationResult(vrWorkflow) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, 'ExportCompilationResult'), vrWorkflow, {
				returnAllResponseParameters: true,
				responseTypeAsBufferArray: true
			});
		}

		function UpdateVRWorkflow(vrWorkflow) {
			return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "UpdateVRWorkflow"), vrWorkflow);
		}

		function HasAddVRWorkflowPermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['InsertVRWorkflow']));
		}

		function HasEditVRWorkflowPermission() {
			return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['UpdateVRWorkflow']));
		}

		return ({
			GetFilteredVRWorkflows: GetFilteredVRWorkflows,
			GetVRWorkflowEditorRuntime: GetVRWorkflowEditorRuntime,
			GetVRWorkflowVariablesTypeDescription: GetVRWorkflowVariablesTypeDescription,
			GetVRWorkflowVariableTypeExtensionConfigs: GetVRWorkflowVariableTypeExtensionConfigs,
			GetVRWorkflowActivityExtensionConfigs: GetVRWorkflowActivityExtensionConfigs,
			GetVRWorkflowArgumentTypeDescription: GetVRWorkflowArgumentTypeDescription,
			GetVRWorkflowVariableTypeDescription: GetVRWorkflowVariableTypeDescription,
			InsertVRWorkflow: InsertVRWorkflow,
			UpdateVRWorkflow: UpdateVRWorkflow,
			HasAddVRWorkflowPermission: HasAddVRWorkflowPermission,
			HasEditVRWorkflowPermission: HasEditVRWorkflowPermission,
			TryCompileWorkflow: TryCompileWorkflow,
			ExportCompilationResult: ExportCompilationResult
		});
	}

	appControllers.service('BusinessProcess_VRWorkflowAPIService', BusinessProcess_VRWorkflowAPIService);

})(appControllers);