(function (appControllers) {
    "use strict";
    faultTicketAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];
    function faultTicketAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "FaultTicket";
       
        function GetCustomerFaultTicketDetails(customerFaultTicketInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerFaultTicketDetails"),  customerFaultTicketInput);
        }
        function GetSupplierFaultTicketDetails(supplierFaultTicketInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierFaultTicketDetails"), supplierFaultTicketInput);
        }
       
    
    return ({
        GetCustomerFaultTicketDetails: GetCustomerFaultTicketDetails,
        GetSupplierFaultTicketDetails: GetSupplierFaultTicketDetails
    });
}
    appControllers.service("WhS_BE_FaultTicketAPIService", faultTicketAPIService);
})(appControllers);