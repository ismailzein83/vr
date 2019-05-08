//(function (app) {

//    "use strict";
//    accountManagerAssignmentAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

//    function accountManagerAssignmentAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

//        var controllerName = "AccountManagerAssignment";

//        function GetAccountManagerAssignmentByCarrierAccountId(carrierAccountId) {
//            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAccountManagerAssignmentByCarrierAccountId"), carrierAccountId);
//        }
 
//        return ({
//            GetAccountManagerAssignmentByCarrierAccountId: GetAccountManagerAssignmentByCarrierAccountId
//        });
//    }

//    app.service("WhS_BE_AccountManagerAssignmentAPIService", accountManagerAssignmentAPIService);
//})(app);