(function (appControllers) {
    "use strict";
    complaintsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function complaintsAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Complaint";

        function GetComplaints() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetComplaints"));
        }

        return {
            GetComplaints: GetComplaints
        };
    };
    appControllers.service("Demo_Module_ComplaintAPIService", complaintsAPIService);

})(appControllers);