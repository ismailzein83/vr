(function (appControllers) {

    "use strict";

    BusinessProcess_ProcessSynchronisationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

    function BusinessProcess_ProcessSynchronisationAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {

        var controllerName = "ProcessSynchronisation";

        function GetFilteredProcessesSynchronisations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetFilteredProcessesSynchronisations"), input);
        }

        function GetProcessSynchronisation(processSynchronisationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetProcessSynchronisation"), {
                processSynchronisationId: processSynchronisationId
            });
        }

        function AddProcessSynchronisation(processSynchronisation) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, 'AddProcessSynchronisation'), processSynchronisation);
        }

        function UpdateProcessSynchronisation(processSynchronisation) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, 'UpdateProcessSynchronisation'), processSynchronisation);
        }

        function HasAddProcessSynchronisationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['AddProcessSynchronisation']));
        }

        function HasUpdateProcessSynchronisationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, ['UpdateProcessSynchronisation']));
        }

        function EnableProcessSynchronisation(processSynchronisationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "EnableProcessSynchronisation"), {
                processSynchronisationId: processSynchronisationId
            });
        }

        function DisableProcessSynchronisation(processSynchronisationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "DisableProcessSynchronisation"), {
                processSynchronisationId: processSynchronisationId
            });
        }

        return ({
            GetFilteredProcessesSynchronisations: GetFilteredProcessesSynchronisations,
            GetProcessSynchronisation: GetProcessSynchronisation,
            AddProcessSynchronisation: AddProcessSynchronisation,
            UpdateProcessSynchronisation: UpdateProcessSynchronisation,
            HasAddProcessSynchronisationPermission: HasAddProcessSynchronisationPermission,
            HasUpdateProcessSynchronisationPermission: HasUpdateProcessSynchronisationPermission,
            EnableProcessSynchronisation: EnableProcessSynchronisation,
            DisableProcessSynchronisation: DisableProcessSynchronisation
        });
    }

    appControllers.service('BusinessProcess_ProcessSynchronisationAPIService', BusinessProcess_ProcessSynchronisationAPIService);
})(appControllers);