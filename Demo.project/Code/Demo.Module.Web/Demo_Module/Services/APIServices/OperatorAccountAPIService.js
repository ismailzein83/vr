(function (appControllers) {

    "use strict";
    operatorAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function operatorAccountAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredOperatorAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorAccount", "GetFilteredOperatorAccounts"), input);
        }
        
        function GetOperatorAccount(operatorAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorAccount", "GetOperatorAccount"), {
                operatorAccountId: operatorAccountId
            });

        }

        function GetOperatorAccountsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorAccount", "GetOperatorAccountsInfo"));

        }

        
       
        function AddOperatorAccount(operatorAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorAccount", "AddOperatorAccount"), operatorAccountObject);
        }
        function UpdateOperatorAccount(operatorAccountObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorAccount", "UpdateOperatorAccount"), operatorAccountObject);
        }

        return ({
            GetFilteredOperatorAccounts: GetFilteredOperatorAccounts,
            GetOperatorAccountsInfo: GetOperatorAccountsInfo,
            GetOperatorAccount: GetOperatorAccount,
            AddOperatorAccount: AddOperatorAccount,
            UpdateOperatorAccount: UpdateOperatorAccount,
        });
    }

    appControllers.service('Demo_OperatorAccountAPIService', operatorAccountAPIService);

})(appControllers);