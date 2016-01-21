(function (appControllers) {

    "use strict";
    defineFixedPrefixesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Fzero_FraudAnalysis_ModuleConfig'];

    function defineFixedPrefixesAPIService(BaseAPIService, UtilsService, Fzero_FraudAnalysis_ModuleConfig) {

        function GetFilteredFixedPrefixes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "GetFilteredFixedPrefixes"), input);
        }

        function UpdateFixedPrefix(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "UpdateFixedPrefix"), input);
        }

        function AddFixedPrefix(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "AddFixedPrefix"), input);
        }
        
        function GetFixedPrefix(fixedPrefixId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "GetFixedPrefix"), { fixedPrefixId: fixedPrefixId });
        }
        function DeleteFixedPrefix(fixedPrefixId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "DeleteFixedPrefix"), { fixedPrefixId: fixedPrefixId });
        }

        function GetPrefixesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Fzero_FraudAnalysis_ModuleConfig.moduleName, "DefineFixedPrefixes", "GetPrefixesInfo"));
        }

        return ({
            GetFilteredFixedPrefixes: GetFilteredFixedPrefixes,
            UpdateFixedPrefix: UpdateFixedPrefix,
            AddFixedPrefix: AddFixedPrefix,
            GetFixedPrefix: GetFixedPrefix,
            DeleteFixedPrefix: DeleteFixedPrefix,
            GetPrefixesInfo: GetPrefixesInfo
        });
    }

    appControllers.service('Fzero_FraudAnalysis_DefineFixedPrefixesAPIService', defineFixedPrefixesAPIService);
})(appControllers);