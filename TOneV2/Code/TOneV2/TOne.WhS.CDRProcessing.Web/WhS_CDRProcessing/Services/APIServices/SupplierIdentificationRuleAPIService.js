(function (appControllers) {

    "use strict";
    supplierIdentificationRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function supplierIdentificationRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredSupplierIdentificationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SupplierIdentificationRule", "GetFilteredSupplierIdentificationRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SupplierIdentificationRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function AddRule(supplierRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SupplierIdentificationRule", "AddRule"), supplierRuleObject);
        }

        function UpdateRule(supplierRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SupplierIdentificationRule", "UpdateRule"), supplierRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SupplierIdentificationRule", "DeleteRule"), { ruleId: ruleId });
        }
        return ({
            GetFilteredSupplierIdentificationRules: GetFilteredSupplierIdentificationRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
        });
    }

    appControllers.service('WhS_CDRProcessing_SupplierIdentificationRuleAPIService', supplierIdentificationRuleAPIService);
})(appControllers);