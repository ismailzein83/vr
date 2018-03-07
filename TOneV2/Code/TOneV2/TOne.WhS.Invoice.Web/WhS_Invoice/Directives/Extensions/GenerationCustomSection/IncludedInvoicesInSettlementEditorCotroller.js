(function (appControllers) {

    "use strict";

    includedInvoicesInSettlementEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", 'WhS_Invoice_IncludedInvoicesInSettlementAPIService'];

    function includedInvoicesInSettlementEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_Invoice_IncludedInvoicesInSettlementAPIService) {
       
        var customerInvoiceIds;
        var supplierInvoiceIds;

        var availableCustomerInvoices;
        var availableSupplierInvoices;

        var isCustomerApplicable;
        var isSupplierApplicable;

        var invoiceTypeId;
        var partnerId;

        var fromDate;
        var toDate;

        var invoicesDetails;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                customerInvoiceIds = parameters.customerInvoiceIds;
                supplierInvoiceIds = parameters.supplierInvoiceIds;

                availableCustomerInvoices = parameters.availableCustomerInvoices;
                availableSupplierInvoices = parameters.availableSupplierInvoices;

                isCustomerApplicable = parameters.isCustomerApplicable;
                isSupplierApplicable = parameters.isSupplierApplicable;

                invoiceTypeId = parameters.invoiceTypeId;
                partnerId = parameters.partnerId;

                fromDate = parameters.fromDate;
                toDate = parameters.toDate;

            }
        }
       
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.showCustomerInvoiceGrid = isCustomerApplicable;

            $scope.scopeModel.showSupplierInvoiceGrid = isSupplierApplicable;

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.save = function () {
                return onIncludedInvoicesInSettlement();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadInvoicesDetails().then(function () {
                loadAllControls();
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadInvoicesDetails()
        {
            var loadInvoicesDetailsInput= {
                InvoiceTypeId: invoiceTypeId,
                CustomerInvoiceIds: availableCustomerInvoices != undefined ? UtilsService.getPropValuesFromArray(availableCustomerInvoices, "InvoiceId") : undefined,
                SupplierInvoiceIds: availableSupplierInvoices != undefined? UtilsService.getPropValuesFromArray(availableSupplierInvoices, "InvoiceId"):undefined,
                PartnerId:partnerId
            };
            return WhS_Invoice_IncludedInvoicesInSettlementAPIService.LoadInvoicesDetails(loadInvoicesDetailsInput).then(function (response) {
                invoicesDetails = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = 'Included Invoices In Settlement: ' + invoicesDetails.PartnerName;
            }

            function loadStaticData() {

                if(invoicesDetails != undefined)
                {
                    if (invoicesDetails.CustomerInvoiceDetails != undefined)
                    {
                        $scope.scopeModel.customerInvoices = [];
                        $scope.scopeModel.showCustomerInvoiceGrid = true;
                        for (var i = 0; i < invoicesDetails.CustomerInvoiceDetails.length; i++) {
                            var customerInvoiceDetails = invoicesDetails.CustomerInvoiceDetails[i];
                            var selectedCustomerInvoice = UtilsService.getItemByVal(availableCustomerInvoices, customerInvoiceDetails.InvoiceId, "InvoiceId");
                            if (selectedCustomerInvoice != undefined && selectedCustomerInvoice.IsSelected)
                                customerInvoiceDetails.isSelected = true;
                            $scope.scopeModel.customerInvoices.push({ Entity: customerInvoiceDetails });
                        }
                    }
                    if (invoicesDetails.SupplierInvoiceDetails != undefined) {
                        $scope.scopeModel.supplierInvoices = [];
                        $scope.scopeModel.showSupplierInvoiceGrid = true;

                        for (var i = 0; i < invoicesDetails.SupplierInvoiceDetails.length; i++) {
                            var supplierInvoiceDetails = invoicesDetails.SupplierInvoiceDetails[i];
                            var selectedSupplierInvoice = UtilsService.getItemByVal(availableSupplierInvoices, supplierInvoiceDetails.InvoiceId, "InvoiceId");
                            if (selectedSupplierInvoice != undefined && selectedSupplierInvoice.IsSelected)
                                supplierInvoiceDetails.isSelected = true;
                            $scope.scopeModel.supplierInvoices.push({Entity: supplierInvoiceDetails });
                        }
                    }
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle,loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
     
        function buildSummaryObjectFromScope() {
            var selectedCustomerInvoiceIds;
            if ($scope.scopeModel.customerInvoices != undefined && $scope.scopeModel.customerInvoices.length > 0) {
                selectedCustomerInvoiceIds = [];
                for (var i = 0; i < $scope.scopeModel.customerInvoices.length; i++) {
                    var customerInvoice = $scope.scopeModel.customerInvoices[i];
                    if (customerInvoice.Entity.isSelected)
                        selectedCustomerInvoiceIds.push(customerInvoice.Entity.InvoiceId);
                }
            }

            var selectedSupplierInvoiceIds;
            if ($scope.scopeModel.supplierInvoices != undefined && $scope.scopeModel.supplierInvoices.length >0 )
            {
                selectedSupplierInvoiceIds = [];
                for (var i = 0; i < $scope.scopeModel.supplierInvoices.length; i++) {
                    var supplierInvoice = $scope.scopeModel.supplierInvoices[i];
                    if (supplierInvoice.Entity.isSelected)
                        selectedSupplierInvoiceIds.push(supplierInvoice.Entity.InvoiceId);
                }
            }

            return {
                InvoiceTypeId: invoiceTypeId,
                PartnerId:partnerId,
                CustomerInvoiceIds: selectedCustomerInvoiceIds,
                SupplierInvoiceIds: selectedSupplierInvoiceIds,
                FromDate: fromDate,
                ToDate:toDate
            };
        }

        function onIncludedInvoicesInSettlement() {
            $scope.scopeModel.isLoading = true;
            var summaryObjectInput = buildSummaryObjectFromScope();
            return WhS_Invoice_IncludedInvoicesInSettlementAPIService.TryLoadInvoicesAndGetAmountByCurrency(summaryObjectInput).then(function (response) {
                if ($scope.onIncludedInvoicesInSettlement != undefined)
                    $scope.onIncludedInvoicesInSettlement({
                        summary: response,
                        selectedData: buildSelectedDataFromScope()
                    });
                $scope.modalContext.closeModal();
              
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSelectedDataFromScope() {
            var availableCustomerInvoices;
            if ($scope.scopeModel.customerInvoices != undefined && $scope.scopeModel.customerInvoices.length > 0) {
                availableCustomerInvoices = [];
                for (var i = 0; i < $scope.scopeModel.customerInvoices.length; i++) {
                    var customerInvoice = $scope.scopeModel.customerInvoices[i];
                    availableCustomerInvoices.push({
                        InvoiceId: customerInvoice.Entity.InvoiceId,
                        IsSelected: customerInvoice.Entity.isSelected
                    });
                }
            }

            var availableSupplierInvoices;
            if ($scope.scopeModel.supplierInvoices != undefined && $scope.scopeModel.supplierInvoices.length > 0) {
                availableSupplierInvoices = [];
                for (var i = 0; i < $scope.scopeModel.supplierInvoices.length; i++) {
                    var supplierInvoice = $scope.scopeModel.supplierInvoices[i];

                    availableSupplierInvoices.push({
                        InvoiceId: supplierInvoice.Entity.InvoiceId,
                        IsSelected: supplierInvoice.Entity.isSelected
                    });

                }
            }

            return {
                InvoiceTypeId: invoiceTypeId,
                AvailableCustomerInvoices: availableCustomerInvoices,
                AvailableSupplierInvoices: availableSupplierInvoices
            };
        }

    }

    appControllers.controller("WhS_Invoice_IncludedInvoicesInSettlementEditorController", includedInvoicesInSettlementEditorController);
})(appControllers);
