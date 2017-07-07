"use strict";

app.directive("vrInvoiceactionsSendemailAttachmenttypeRdlfileconverter", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_Invoice_InvoiceAttachmentTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceAttachmentTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new EmailAttachmentRDLCFileConverter($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/MainExtensions/SendEmail/MainExtensions/Templates/EmailAttachmentRDLCFileConverterTemplate.html"

        };

        function EmailAttachmentRDLCFileConverter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.rdlcInvoiceActions = [];
                $scope.scopeModel.invoiceAttachmentTypes = UtilsService.getArrayEnum(VR_Invoice_InvoiceAttachmentTypeEnum);
                $scope.scopeModel.selectedAttachmentType = VR_Invoice_InvoiceAttachmentTypeEnum.PDF;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
               
                    var invoiceFileConverter;
                    if (payload != undefined) {
                        invoiceFileConverter = payload.invoiceFileConverter;
                        context = payload.context;
                        if (context != undefined)
                        {
                            $scope.scopeModel.rdlcInvoiceActions = context.getActionsInfoByActionTypeName("OpenRDLCReportAction");
                            if(invoiceFileConverter != undefined)
                            {
                                $scope.scopeModel.selectedInvoiceAction = UtilsService.getItemByVal($scope.scopeModel.rdlcInvoiceActions, invoiceFileConverter.InvoiceActionId, "InvoiceActionId");
                                $scope.scopeModel.selectedAttachmentType = UtilsService.getItemByVal($scope.scopeModel.invoiceAttachmentTypes, invoiceFileConverter.AttachmentType, "value");

                            }
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceRDLCFileConverter ,Vanrise.Invoice.MainExtensions",
                        InvoiceActionId: $scope.scopeModel.selectedInvoiceAction.InvoiceActionId,
                        AttachmentType: $scope.scopeModel.selectedAttachmentType.value
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
        }

        return directiveDefinitionObject;

    }
]);