(function (appControllers) {

    "use strict";

    PricingTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig', 'SecurityService'];

    function PricingTemplateAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig, SecurityService) {

        var controllerName = "PricingTemplate";


        function GetFilteredPricingTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'GetFilteredPricingTemplates'), input);
        }

        function GetPricingTemplate(pricingTemplated) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'GetPricingTemplate'), {
                PricingTemplateId: pricingTemplated
            });
        }

        function AddPricingTemplate(pricingTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'AddPricingTemplate'), pricingTemplateItem);
        }

        function UpdatePricingTemplate(pricingTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, 'UpdatePricingTemplate'), pricingTemplateItem);
        }

        //function GetPricingTemplatesInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetPricingTemplatesInfo"), {
        //        filter: filter
        //    });
        //}


        return ({
            GetFilteredPricingTemplates: GetFilteredPricingTemplates,
            GetPricingTemplate: GetPricingTemplate,
            AddPricingTemplate: AddPricingTemplate,
            UpdatePricingTemplate: UpdatePricingTemplate,
            //GetPricingTemplatesInfo: GetPricingTemplatesInfo
        });
    }

    appControllers.service('WhS_Sales_PricingTemplateAPIService', PricingTemplateAPIService);

})(appControllers);