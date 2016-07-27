
(function (appControllers) {

    "use strict";
    CreditClassAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function CreditClassAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "CreditClass";


        function GetFilteredCreditClass(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredCreditClass'), input);
        }

        function GetCreditClass(creditClassId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetCreditClass'), {
                creditClassId: creditClassId
            });
        }

        function AddCreditClass(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddCreditClass'), statusDefinitionItem);
        }

        function UpdateCreditClass(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateCreditClass'), statusDefinitionItem);
        }

        //function GetCreditClasssInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetCreditClasssInfo"), {
        //        filter: filter
        //    });
        //}

        return ({
            GetFilteredCreditClasss: GetFilteredCreditClass,
            GetCreditClass: GetCreditClass,
            AddCreditClass: AddCreditClass,
            UpdateCreditClass: UpdateCreditClass,
            //GetCreditClasssInfo: GetCreditClasssInfo,
        });
    }

    appControllers.service('Retail_BE_CreditClassAPIService', CreditClassAPIService);

})(appControllers);