(function (appControllers) {

    "use strict";
    operatorAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'InterConnect_BE_ModuleConfig'];

    function operatorAccountAPIService(BaseAPIService, UtilsService, InterConnect_BE_ModuleConfig) {

        function GetFilteredOperatorAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorAccount", "GetFilteredOperatorAccounts"), input);
        }


        function GetOperatorAccountsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorAccount", "GetOperatorAccountsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetOperatorAccount(operatorAccountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorAccount", "GetOperatorAccount"), {
                operatorAccountId: operatorAccountId
            });
        }

        function UpdateOperatorAccount(operatorAccount) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorAccount", "UpdateOperatorAccount"), operatorAccount);
        }

        function AddOperatorAccount(operatorAccount) {
            return BaseAPIService.post(UtilsService.getServiceURL(InterConnect_BE_ModuleConfig.moduleName, "OperatorAccount", "AddOperatorAccount"), operatorAccount);
        }

        return ({
            GetFilteredOperatorAccounts: GetFilteredOperatorAccounts,
            GetOperatorAccountsInfo:GetOperatorAccountsInfo,
            GetOperatorAccount: GetOperatorAccount,
            AddOperatorAccount: AddOperatorAccount,
            UpdateOperatorAccount: UpdateOperatorAccount
        });
    }

    appControllers.service('InterConnect_BE_OperatorAccountAPIService', operatorAccountAPIService);

})(appControllers);