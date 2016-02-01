(function (appControllers) {

    "use strict";
    numberPrefixesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig', 'VRModalService'];

    function numberPrefixesAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig, VRModalService) {

        function GetPrefixesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "NumberPrefix", "GetPrefixesInfo"));
        }


        function UpdatePrefixes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, "NumberPrefix", "UpdatePrefixes"), input);
        }

        return ({
            GetPrefixesInfo: GetPrefixesInfo,
            UpdatePrefixes: UpdatePrefixes
        });
    }

    appControllers.service('FraudAnalysis_NumberPrefixAPIService', numberPrefixesAPIService);
})(appControllers);