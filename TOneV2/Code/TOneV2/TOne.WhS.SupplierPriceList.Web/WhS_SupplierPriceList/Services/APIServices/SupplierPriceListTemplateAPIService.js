(function (appControllers) {

    "use strict";
    SupplierPriceListTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'WhS_SupPL_ModuleConfig'];
    function SupplierPriceListTemplateAPIService(BaseAPIService, UtilsService, SecurityService, WhS_SupPL_ModuleConfig) {
        var controllerName = 'SupplierPriceListTemplate';

       
        function UpdateSupplierPriceListTemplate(supplierPriceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'UpdateSupplierPriceListTemplate'), supplierPriceListTemplateObj);
        }

        function AddSupplierPriceListTemplate(supplierPriceListTemplateObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'AddSupplierPriceListTemplate'), supplierPriceListTemplateObj);
        }

        function GetSupplierPriceListTemplate(supplierPriceListTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetSupplierPriceListTemplate'),
                {
                    supplierPriceListTemplateId: supplierPriceListTemplateId
                });
        }

        function GetSupplierPriceListTemplatesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetSupplierPriceListTemplatesInfo'),
                {
                    filter: filter
                });
        }

        function GetSupplierPriceListTemplateBySupplierId(supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetSupplierPriceListTemplateBySupplierId'),
                {
                    supplierId: supplierId
                });
        }

        function TestConversionForSupplierPriceList(input)
        {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'TestConversionForSupplierPriceList'),
                 input,{
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });

        }

        function GetSupplierPriceListConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, 'GetSupplierPriceListConfigurationTemplateConfigs'));
        }

        function HasaddSupplierPriceListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SupPL_ModuleConfig.moduleName, controllerName, ['AddSupplierPriceListTemplate']));
        }

        function HasUpdateSupplierPriceListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SupPL_ModuleConfig.moduleName, controllerName, ['UpdateSupplierPriceListTemplate']));
        }

        function HassaveSupplierPriceListConfigurationPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(moduleConfig.moduleName, controllerName, ['UpdateSupplierPriceListTemplate', 'AddSupplierPriceListTemplate']));
        }

        return ({
            UpdateSupplierPriceListTemplate: UpdateSupplierPriceListTemplate,
            AddSupplierPriceListTemplate: AddSupplierPriceListTemplate,
            GetSupplierPriceListTemplate: GetSupplierPriceListTemplate,
            GetSupplierPriceListConfigurationTemplateConfigs: GetSupplierPriceListConfigurationTemplateConfigs,
            GetSupplierPriceListTemplatesInfo: GetSupplierPriceListTemplatesInfo,
            HasaddSupplierPriceListTemplatePermission: HasaddSupplierPriceListTemplatePermission,
            HasUpdateSupplierPriceListTemplatePermission: HasUpdateSupplierPriceListTemplatePermission,
            HassaveSupplierPriceListConfigurationPermission: HassaveSupplierPriceListConfigurationPermission,
            GetSupplierPriceListTemplateBySupplierId: GetSupplierPriceListTemplateBySupplierId,
            TestConversionForSupplierPriceList: TestConversionForSupplierPriceList
        });
    }
    appControllers.service('WhS_SupPL_SupplierPriceListTemplateAPIService', SupplierPriceListTemplateAPIService);

})(appControllers);