(function (appControllers) {

    'use strict';

    IncludedInvoicesInSettlementService.$inject = ['VRModalService'];

    function IncludedInvoicesInSettlementService(VRModalService) {
        return ({
            openIncludedInvoicesInSettlement: openIncludedInvoicesInSettlement,
        });

        function openIncludedInvoicesInSettlement(invoiceTypeId, partnerId, availableCustomerInvoices, availableSupplierInvoices, isCustomerApplicable, isSupplierApplicable,fromDate,toDate, onIncludedInvoicesInSettlement) {
            var modalParameters = {
                invoiceTypeId: invoiceTypeId,
                partnerId:partnerId,
                availableCustomerInvoices: availableCustomerInvoices,
                availableSupplierInvoices: availableSupplierInvoices,
                isCustomerApplicable: isCustomerApplicable,
                isSupplierApplicable: isSupplierApplicable,
                fromDate: fromDate,
                toDate: toDate
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onIncludedInvoicesInSettlement = onIncludedInvoicesInSettlement;
            };

            VRModalService.showModal('/Client/Modules/WhS_Invoice/Directives/Extensions/GenerationCustomSection/Templates/IncludedInvoicesInSettlementEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('WhS_Invoice_IncludedInvoicesInSettlementService', IncludedInvoicesInSettlementService);

})(appControllers);
