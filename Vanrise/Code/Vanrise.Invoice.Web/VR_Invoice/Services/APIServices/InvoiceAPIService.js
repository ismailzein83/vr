﻿(function (appControllers) {

    "use strict";
    invoiceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Invoice_ModuleConfig', 'SecurityService'];

    function invoiceAPIService(BaseAPIService, UtilsService, VR_Invoice_ModuleConfig, SecurityService) {

        var controllerName = 'Invoice';

        function GetInvoice(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoice"), {
                invoiceId: invoiceId
            });
        }
        function GenerateInvoice(createInvoiceInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GenerateInvoice"), createInvoiceInput);
        }
        function ReGenerateInvoice(createInvoiceInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "ReGenerateInvoice"), createInvoiceInput);
        }
        function GetFilteredInvoices(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetFilteredInvoices'), input);
        }
        function SetInvoicePaid(invoiceId, isInvoicePaid) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'SetInvoicePaid'), {
                invoiceId: invoiceId,
                isInvoicePaid: isInvoicePaid
            });
        }
        function SetInvoiceLocked(invoiceId, setLocked) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'SetInvoiceLocked'), {
                invoiceId: invoiceId,
                setLocked: setLocked
            });
        }
        function UpdateInvoiceNote(invoiceId, invoiceNote) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'UpdateInvoiceNote'), {
                invoiceId: invoiceId,
                invoiceNote: invoiceNote
            });
        }
        function GetInvoiceDetail(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetInvoiceDetail'), {
                invoiceId: invoiceId,
            });
        }

        function DoesUserHaveGenerateAccess(invoiceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'DoesUserHaveGenerateAccess'), {
                invoiceTypeId: invoiceTypeId
            });
        }
        function GetBillingInterval(invoiceTypeId, partnerId, issueDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetBillingInterval'), {
                invoiceTypeId: invoiceTypeId,
                partnerId: partnerId,
                issueDate: issueDate
            });
        }
        function CheckGeneratedInvoicePeriodGaP(fromDate, invoiceTypeId, partnerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "CheckGeneratedInvoicePeriodGaP"), {
                fromDate: fromDate,
                invoiceTypeId: invoiceTypeId,
                partnerId: partnerId
            });
        }
        function CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "CheckInvoiceFollowBillingPeriod"), {
                invoiceTypeId: invoiceTypeId,
                partnerId: partnerId
            });
        }
        function GetInvoiceEditorRuntime(invoiceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetInvoiceEditorRuntime"), {
                invoiceId: invoiceId
            });
        }
        function GetPartnerGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, "GetPartnerGroupTemplates"));
        }

        function GenerateFilteredInvoiceGenerationDrafts(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GenerateFilteredInvoiceGenerationDrafts'), query);
        }

        function GetFilteredInvoiceGenerationDrafts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Invoice_ModuleConfig.moduleName, controllerName, 'GetFilteredInvoiceGenerationDrafts'), input);
        }

        return ({
            GetInvoice: GetInvoice,
            GenerateInvoice: GenerateInvoice,
            DoesUserHaveGenerateAccess: DoesUserHaveGenerateAccess,
            GetFilteredInvoices: GetFilteredInvoices,
            SetInvoicePaid: SetInvoicePaid,
            GetInvoiceDetail: GetInvoiceDetail,
            SetInvoiceLocked: SetInvoiceLocked,
            ReGenerateInvoice: ReGenerateInvoice,
            UpdateInvoiceNote: UpdateInvoiceNote,
            GetBillingInterval: GetBillingInterval,
            CheckGeneratedInvoicePeriodGaP: CheckGeneratedInvoicePeriodGaP,
            CheckInvoiceFollowBillingPeriod: CheckInvoiceFollowBillingPeriod,
            GetInvoiceEditorRuntime: GetInvoiceEditorRuntime,
            GetPartnerGroupTemplates: GetPartnerGroupTemplates,
            GenerateFilteredInvoiceGenerationDrafts: GenerateFilteredInvoiceGenerationDrafts,
            GetFilteredInvoiceGenerationDrafts: GetFilteredInvoiceGenerationDrafts
        });
    }

    appControllers.service('VR_Invoice_InvoiceAPIService', invoiceAPIService);

})(appControllers);