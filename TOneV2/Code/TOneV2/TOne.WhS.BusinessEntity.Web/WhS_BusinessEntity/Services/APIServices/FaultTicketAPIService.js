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
        function GetAccountManagerName(accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerName"), { accountId: accountId });
        }
    
    return ({
        GetCustomerFaultTicketDetails: GetCustomerFaultTicketDetails,
        GetSupplierFaultTicketDetails: GetSupplierFaultTicketDetails,
        GetAccountManagerName: GetAccountManagerName
    });
}
    appControllers.service("WhS_BE_FaultTicketAPIService", faultTicketAPIService);
})(appControllers);