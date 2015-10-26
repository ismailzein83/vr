(function (appControllers) {

    "use strict";
    setSupplierRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function setSupplierRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredSetSupplierRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetSupplierRule", "GetFilteredSetSupplierRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetSupplierRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function AddRule(supplierRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetSupplierRule", "AddRule"), supplierRuleObject);
        }

        function UpdateRule(supplierRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetSupplierRule", "UpdateRule"), supplierRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetSupplierRule", "DeleteRule"), { ruleId: ruleId });
        }
        return ({
            GetFilteredSetSupplierRules: GetFilteredSetSupplierRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
        });
    }

    appControllers.service('WhS_CDRProcessing_SetSupplierRuleAPIService', setSupplierRuleAPIService);
})(appControllers);