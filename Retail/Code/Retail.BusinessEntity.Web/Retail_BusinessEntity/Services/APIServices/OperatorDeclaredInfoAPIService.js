(function (appControllers) {

    'use strict';

    Retail_BE_OperatorDeclaredInfoAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function Retail_BE_OperatorDeclaredInfoAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'OperatorDeclaredInfo';

        function GetFilteredOperatorDeclaredInfos(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredOperatorDeclaredInfos'), input);
        }

        function GetOperatorDeclaredInfo(operatorDeclaredInfoId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetOperatorDeclaredInfo'), {
                operatorDeclaredInfoId: operatorDeclaredInfoId
            });
        }

        function GetOperatorDeclaredInfosInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetOperatorDeclaredInfosInfo"), {
                filter: filter
            });
        }
        function AddOperatorDeclaredInfo(operatorDeclaredInfo) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddOperatorDeclaredInfo'), operatorDeclaredInfo);
        }

        function UpdateOperatorDeclaredInfo(operatorDeclaredInfo) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateOperatorDeclaredInfo'), operatorDeclaredInfo);
        }

     
        return {
            GetFilteredOperatorDeclaredInfos: GetFilteredOperatorDeclaredInfos,
            GetOperatorDeclaredInfo: GetOperatorDeclaredInfo,
            AddOperatorDeclaredInfo: AddOperatorDeclaredInfo,
            UpdateOperatorDeclaredInfo: UpdateOperatorDeclaredInfo
        };
    }

    appControllers.service('Retail_BE_OperatorDeclaredInfoAPIService', Retail_BE_OperatorDeclaredInfoAPIService);

})(appControllers);