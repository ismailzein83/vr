(function (appControllers) {

    "use strict";
    operatorDeclaredInformationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function operatorDeclaredInformationAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredOperatorDeclaredInformations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorDeclaredInformation", "GetFilteredOperatorDeclaredInformations"), input);
        }

        function GetOperatorDeclaredInformation(operatorDeclaredInformationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorDeclaredInformation", "GetOperatorDeclaredInformation"), {
                operatorDeclaredInformationId: operatorDeclaredInformationId
            });

        }
       
        function UpdateOperatorDeclaredInformation(operatorDeclaredInformationObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorDeclaredInformation", "UpdateOperatorDeclaredInformation"), operatorDeclaredInformationObject);
        }
        function AddOperatorDeclaredInformation(operatorDeclaredInformationObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorDeclaredInformation", "AddOperatorDeclaredInformation"), operatorDeclaredInformationObject);
        }
        return ({
            GetFilteredOperatorDeclaredInformations: GetFilteredOperatorDeclaredInformations,
            GetOperatorDeclaredInformation: GetOperatorDeclaredInformation,
            AddOperatorDeclaredInformation: AddOperatorDeclaredInformation,
            UpdateOperatorDeclaredInformation: UpdateOperatorDeclaredInformation
        });
    }

    appControllers.service('Demo_OperatorDeclaredInformationAPIService', operatorDeclaredInformationAPIService);

})(appControllers);