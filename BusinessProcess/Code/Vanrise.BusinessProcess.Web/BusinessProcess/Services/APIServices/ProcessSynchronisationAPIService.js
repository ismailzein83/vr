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

        function GetProcessSynchronisations() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetProcessSynchronisations"));
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

        return ({
            GetFilteredProcessesSynchronisations: GetFilteredProcessesSynchronisations,
            GetProcessSynchronisation: GetProcessSynchronisation,
            GetProcessSynchronisations: GetProcessSynchronisations,
            AddProcessSynchronisation: AddProcessSynchronisation,
            UpdateProcessSynchronisation: UpdateProcessSynchronisation,
            HasAddProcessSynchronisationPermission: HasAddProcessSynchronisationPermission,
            HasUpdateProcessSynchronisationPermission: HasUpdateProcessSynchronisationPermission
        });
    }

    appControllers.service('BusinessProcess_ProcessSynchronisationAPIService', BusinessProcess_ProcessSynchronisationAPIService);
})(appControllers);