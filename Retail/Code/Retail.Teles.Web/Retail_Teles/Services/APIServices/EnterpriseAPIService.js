(function (appControllers) {

    "use strict";
    EnterpriseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function EnterpriseAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesEnterprise";

        function GetEnterprisesInfo(vrConnectionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetEnterprisesInfo"), {
                vrConnectionId: vrConnectionId,
                serializedFilter: serializedFilter,
            });
        }



        function MapEnterpriseToAccount(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "MapEnterpriseToAccount"), input);
        }
        function GetParentAccountEnterpriseId(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetParentAccountEnterpriseId"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
            });
        }
        function GetFilteredEnterpriseDIDs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetFilteredEnterpriseDIDs"), input);
        }
        function GetFilteredEnterpriseBusinessTrunks(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetFilteredEnterpriseBusinessTrunks"), input);
        }

        function GetFilteredAccountEnterprisesDIDs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetFilteredAccountEnterprisesDIDs"), input);
        }
        function SaveAccountEnterprisesDIDs() {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "SaveAccountEnterprisesDIDs"));
        }

        return ({
            GetEnterprisesInfo: GetEnterprisesInfo,
            MapEnterpriseToAccount: MapEnterpriseToAccount,
            GetParentAccountEnterpriseId: GetParentAccountEnterpriseId,
            GetFilteredEnterpriseDIDs: GetFilteredEnterpriseDIDs,
            GetFilteredEnterpriseBusinessTrunks: GetFilteredEnterpriseBusinessTrunks,
            GetFilteredAccountEnterprisesDIDs: GetFilteredAccountEnterprisesDIDs,
            SaveAccountEnterprisesDIDs: SaveAccountEnterprisesDIDs
        });
    }

    appControllers.service('Retail_Teles_EnterpriseAPIService', EnterpriseAPIService);

})(appControllers);