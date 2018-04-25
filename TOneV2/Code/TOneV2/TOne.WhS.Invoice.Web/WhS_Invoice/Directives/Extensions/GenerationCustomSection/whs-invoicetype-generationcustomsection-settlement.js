"use strict";

app.directive("whsInvoicetypeGenerationcustomsectionSettlement", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_FinancialAccountAPIService", "WhS_BE_CommisssionTypeEnum",'WhS_Invoice_IncludedInvoicesInSettlementService','WhS_Invoice_IncludedInvoicesInSettlementAPIService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_FinancialAccountAPIService, WhS_BE_CommisssionTypeEnum, WhS_Invoice_IncludedInvoicesInSettlementService, WhS_Invoice_IncludedInvoicesInSettlementAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerGenerationCustomSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSettlementGeneration",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/GenerationCustomSection/Templates/SettlementGenerationCustomSectionTemplate.html"
        };

        function CustomerGenerationCustomSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var customPayload;
            var invoiceTypeId;
          //  var oldCommission;
            var financialAccountId;

            var isCustomerApplicable;
            var isSupplierApplicable;

            var fromDate;
            var toDate;

            var availableCustomerInvoices;
            var availableSupplierInvoices;

            var supplierAmountByCurrency;
            var customerAmountByCurrency;
            var errorMessage;

            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.showCustomerSummary = false;
                $scope.scopeModel.showSupplierSummary = false;


                //$scope.scopeModel.onCommissionFocusChanged = function () {
                //    if (oldCommission != $scope.scopeModel.commission)
                //        onvaluechanged();
                //    oldCommission != $scope.scopeModel.commission;
                //};

                $scope.scopeModel.openInvoicesEditor = function () {
                    var onIncludedInvoicesInSettlement = function (includedInvoicesInSettlement) {

                        if (includedInvoicesInSettlement != undefined)
                        {
                            availableCustomerInvoices = includedInvoicesInSettlement.selectedData.AvailableCustomerInvoices;
                            availableSupplierInvoices = includedInvoicesInSettlement.selectedData.AvailableSupplierInvoices;

                            evaluateSummarySection(includedInvoicesInSettlement.summary);

                            onvaluechanged();
                        }
                    };
                    WhS_Invoice_IncludedInvoicesInSettlementService.openIncludedInvoicesInSettlement(invoiceTypeId, financialAccountId, availableCustomerInvoices, availableSupplierInvoices, customPayload.isCustomerApplicable, customPayload.isSupplierApplicable,fromDate,toDate, onIncludedInvoicesInSettlement);
                };

                function onvaluechanged() {
                    if (context != undefined && context.onvaluechanged != undefined && typeof (context.onvaluechanged) == 'function') {
                        context.onvaluechanged();
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var invoice;
                    var promises = [];
                    if (payload != undefined) {
                        customPayload = payload.customPayload;
                        invoice = payload.invoice;
                        financialAccountId = payload.partnerId;
                        invoiceTypeId = payload.invoiceTypeId;
                        fromDate = payload.fromDate;
                        toDate = payload.toDate;

                        if (financialAccountId == undefined) {
                            financialAccountId = payload.financialAccountId;
                        }
                        if (customPayload != undefined) {

                          //  oldCommission = $scope.scopeModel.commission = customPayload.Commission;


                            availableCustomerInvoices = customPayload.AvailableCustomerInvoices;

                            availableSupplierInvoices = customPayload.AvailableSupplierInvoices;

                            $scope.scopeModel.showCustomerSummary = customPayload.IsCustomerApplicable;
                            $scope.scopeModel.showSupplierSummary = customPayload.IsSupplierApplicable;

                            isCustomerApplicable = customPayload.IsCustomerApplicable;
                            isSupplierApplicable = customPayload.IsSupplierApplicable;


                            evaluateSummarySection(customPayload.Summary);
                            
                        }
                        if (invoice != undefined) {

                            fromDate = invoice.FromDate;
                            toDate = invoice.ToDate;

                            var promise = loadCustomPayload(invoice).then(function () {
                                financialAccountId = invoice.PartnerId;
                                invoiceTypeId = invoice.InvoiceTypeId;
                                availableCustomerInvoices = customPayload.AvailableCustomerInvoices;
                                availableSupplierInvoices = customPayload.AvailableSupplierInvoices;

                                
                                $scope.scopeModel.showCustomerSummary = customPayload.IsCustomerApplicable;
                                $scope.scopeModel.showSupplierSummary = customPayload.IsSupplierApplicable;


                                evaluateSummarySection(customPayload.Summary);

                            });
                            promises.push(promise);

                            if (invoice.Details != undefined)
                            {
                              //  oldCommission = $scope.scopeModel.commission = invoice.Details.Commission;
                            }
                            
                        }
                        context = payload.context;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                    });
                };

                function loadCustomPayload(invoice)
                {
                    return WhS_Invoice_IncludedInvoicesInSettlementAPIService.EvaluatePartnerCustomPayload(invoice.PartnerId, invoice.InvoiceTypeId, invoice.FromDate, invoice.ToDate).then(function (response) {
                        customPayload = response;
                    });
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.SettlementGenerationCustomSectionPayload,TOne.WhS.Invoice.Entities",
                      //  Commission: $scope.scopeModel.commission,
                        AvailableCustomerInvoices: availableCustomerInvoices,
                        AvailableSupplierInvoices: availableSupplierInvoices,
                        Summary:{
                            SupplierAmountByCurrency: supplierAmountByCurrency,
                            CustomerAmountByCurrency: customerAmountByCurrency,
                            ErrorMessage: $scope.scopeModel.errorMessage
                        },
                        IsCustomerApplicable: isCustomerApplicable,
                        IsSupplierApplicable: isSupplierApplicable
                    };
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function buildSupplierSummary(summaryPayload)
            {
                $scope.scopeModel.supplierCountSummary = "";

                if (availableSupplierInvoices != undefined && availableSupplierInvoices.length > 0)
                {
                    var selected = 0;
                    for (var i = 0; i < availableSupplierInvoices.length; i++) {
                        var availableInvoice = availableSupplierInvoices[i];
                        if (availableInvoice.IsSelected)
                            selected++;
                    }
                    $scope.scopeModel.supplierCountSummary += selected + " of " + availableSupplierInvoices.length + " supplier invoices ";
                }
              //  $scope.scopeModel.supplierAmountSummary = "";
                if (summaryPayload != undefined)
                {
                    $scope.scopeModel.supplierCountSummary += "(";
                    var counter = 0;
                    for(var s in summaryPayload)
                    {
                        if (s != "$type")
                        {
                            if (counter > 0)
                                $scope.scopeModel.supplierCountSummary += ',';
                            $scope.scopeModel.supplierCountSummary += +summaryPayload[s] + ' ' + s;
                            counter++;
                        }
                    }
                    $scope.scopeModel.supplierCountSummary += ").";

                }
                if ($scope.scopeModel.supplierCountSummary == "")
                    $scope.scopeModel.supplierCountSummary = undefined;
            }

            function buildCustomerSummary(summaryPayload) {
                $scope.scopeModel.customerCountSummary = "";

                if (availableCustomerInvoices != undefined && availableCustomerInvoices.length > 0) {
                    var selected = 0;
                    for (var i = 0; i < availableCustomerInvoices.length; i++) {
                        var availableInvoice = availableCustomerInvoices[i];
                        if (availableInvoice.IsSelected)
                            selected++;
                    }
                    $scope.scopeModel.customerCountSummary += selected + " of " + availableCustomerInvoices.length + " customer invoices ";
                }
                //$scope.scopeModel.customerAmountSummary = "";
                if (summaryPayload != undefined) {
                    var counter = 0;

                    $scope.scopeModel.customerCountSummary += "(";
                    for (var s in summaryPayload) {
                        if (s != "$type")
                        {
                            if (counter > 0)
                                $scope.scopeModel.customerCountSummary += ',';

                            $scope.scopeModel.customerCountSummary += summaryPayload[s] + ' ' + s;
                            counter++;
                        }
                    }
                    $scope.scopeModel.customerCountSummary += ").";
                }
                if ($scope.scopeModel.customerCountSummary == "")
                    $scope.scopeModel.customerCountSummary = undefined;
            }

            function evaluateSummarySection(summary)
            {
                if (summary != undefined) {

                    $scope.scopeModel.errorMessage = summary.ErrorMessage;

                    supplierAmountByCurrency = summary.SupplierAmountByCurrency;
                    customerAmountByCurrency = summary.CustomerAmountByCurrency;


                    if ($scope.scopeModel.errorMessage == undefined) {
                        buildSupplierSummary(summary.SupplierAmountByCurrency);
                        buildCustomerSummary(summary.CustomerAmountByCurrency);
                    }
                }
            }
        }

        return directiveDefinitionObject;
    }
]);
