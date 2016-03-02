(function (appControllers) {

    "use strict";
    numberPrefixesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig', 'VRModalService'];

    function numberPrefixesAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig, VRModalService) {

        var controllerName = 'NumberPrefix';

        function GetPrefixes() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetPrefixes"));
        }


        function UpdatePrefixes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "UpdatePrefixes"), input);
        }

        return ({
            GetPrefixes: GetPrefixes,
            UpdatePrefixes: UpdatePrefixes
        });
    }

    appControllers.service('FraudAnalysis_NumberPrefixAPIService', numberPrefixesAPIService);
})(appControllers);