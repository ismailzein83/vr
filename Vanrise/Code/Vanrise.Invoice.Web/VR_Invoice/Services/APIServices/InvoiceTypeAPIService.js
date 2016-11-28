(function (appControllers) {

    "use strict";
    invoiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceTypeAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'InvoiceType';

        function GetInvoiceType(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceType"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetInvoiceTypeRuntime(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypeRuntime"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetFilteredInvoiceTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetFilteredInvoiceTypes"), input);
        }
        function AddInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "AddInvoiceType"), invoiceType);
        }
        function UpdateInvoiceType(invoiceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "UpdateInvoiceType"), invoiceType);
        }
        function ConvertToGridColumnAttribute(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "ConvertToGridColumnAttribute"), input);
        }
        function GetInvoiceTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceTypesInfo"), { filter: filter });
        }
        function GetGeneratorInvoiceTypeRuntime(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetGeneratorInvoiceTypeRuntime"), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetInvoiceGeneratorActions(generateInvoiceInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceGeneratorActions"), generateInvoiceInput);
        }

        return ({
            GetInvoiceType: GetInvoiceType,
            GetFilteredInvoiceTypes: GetFilteredInvoiceTypes,
            AddInvoiceType: AddInvoiceType,
            UpdateInvoiceType: UpdateInvoiceType,
            GetInvoiceTypeRuntime: GetInvoiceTypeRuntime,
            ConvertToGridColumnAttribute: ConvertToGridColumnAttribute,
            GetInvoiceTypesInfo: GetInvoiceTypesInfo,
            GetGeneratorInvoiceTypeRuntime: GetGeneratorInvoiceTypeRuntime,
            GetInvoiceGeneratorActions: GetInvoiceGeneratorActions
        });
    }

    appControllers.service('VR_Invoice_InvoiceTypeAPIService', invoiceTypeAPIService);

})(appControllers);