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

            var customerInvoiceIds;
            if (availableCustomerInvoices != undefined) {
                customerInvoiceIds = [];
                for (var i = 0; i < availableCustomerInvoices.length; i++) {
                    var availableCustomerInvoice = availableCustomerInvoices[i];
                    customerInvoiceIds.push({
                        InvoiceId: availableCustomerInvoice.InvoiceId,
                        CurrencyId: availableCustomerInvoice.CurrencyId
                    });
                }
            }


            var supplierInvoiceIds;
            if (availableSupplierInvoices != undefined) {
                supplierInvoiceIds = [];
                for (var i = 0; i < availableSupplierInvoices.length; i++) {
                    var availableSupplierInvoice = availableSupplierInvoices[i];
                    supplierInvoiceIds.push({
                        InvoiceId: availableSupplierInvoice.InvoiceId,
                        CurrencyId: availableSupplierInvoice.CurrencyId
                    });
                }

            }
            var loadInvoicesDetailsInput = {
                InvoiceTypeId: invoiceTypeId,
                CustomerInvoiceIds: customerInvoiceIds,
                SupplierInvoiceIds: supplierInvoiceIds,
                PartnerId: partnerId
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
                            for (var j = 0; j < availableCustomerInvoices.length; j++)
                            {
                                var availableCustomerInvoice = availableCustomerInvoices[j];
                                if(availableCustomerInvoice.InvoiceId == customerInvoiceDetails.InvoiceId && customerInvoiceDetails.CurrencyId == availableCustomerInvoice.CurrencyId)
                                {
                                    if (availableCustomerInvoice.IsSelected)
                                    {
                                        customerInvoiceDetails.isSelected = true;
                                    }
                                    break;
                                }
                            }
                            $scope.scopeModel.customerInvoices.push({ Entity: customerInvoiceDetails });
                        }
                    }
                    if (invoicesDetails.SupplierInvoiceDetails != undefined) {
                        $scope.scopeModel.supplierInvoices = [];
                        $scope.scopeModel.showSupplierInvoiceGrid = true;

                        for (var i = 0; i < invoicesDetails.SupplierInvoiceDetails.length; i++) {
                            var supplierInvoiceDetails = invoicesDetails.SupplierInvoiceDetails[i];
                                for (var j = 0; j < availableSupplierInvoices.length; j++) {
                                var availableSupplierInvoice = availableSupplierInvoices[j];
                                if (availableSupplierInvoice.InvoiceId == supplierInvoiceDetails.InvoiceId && supplierInvoiceDetails.CurrencyId == availableSupplierInvoice.CurrencyId) {
                                    if (availableSupplierInvoice.IsSelected) {
                                        supplierInvoiceDetails.isSelected = true;
                                    }
                                    break;
                                }
                            }
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
                        selectedCustomerInvoiceIds.push({
                            InvoiceId: customerInvoice.Entity.InvoiceId,
                            CurrencyId: customerInvoice.Entity.CurrencyId
                        });
                }
            }

            var selectedSupplierInvoiceIds;
            if ($scope.scopeModel.supplierInvoices != undefined && $scope.scopeModel.supplierInvoices.length >0 )
            {
                selectedSupplierInvoiceIds = [];
                for (var i = 0; i < $scope.scopeModel.supplierInvoices.length; i++) {
                    var supplierInvoice = $scope.scopeModel.supplierInvoices[i];
                    if (supplierInvoice.Entity.isSelected)
                        selectedSupplierInvoiceIds.push({
                            InvoiceId: supplierInvoice.Entity.InvoiceId,
                            CurrencyId: supplierInvoice.Entity.CurrencyId
                        });
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
                        CurrencyId: customerInvoice.Entity.CurrencyId,
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
                        CurrencyId: supplierInvoice.Entity.CurrencyId,
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
