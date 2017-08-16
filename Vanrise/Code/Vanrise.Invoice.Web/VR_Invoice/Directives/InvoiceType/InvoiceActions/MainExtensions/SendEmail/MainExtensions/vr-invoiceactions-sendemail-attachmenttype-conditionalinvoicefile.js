"use strict";

app.directive("vrInvoiceactionsSendemailAttachmenttypeConditionalinvoicefile", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_Invoice_InvoiceAttachmentService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceAttachmentService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ConditionalInvoiceFileTemplate($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/MainExtensions/SendEmail/MainExtensions/Templates/ConditionalInvoiceFileTemplate.html"

        };

        function ConditionalInvoiceFileTemplate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one attachment.";
                };

                ctrl.addConditionalAttachment = function () {
                    var onGridConditionalAttachmentAdded = function (conditionalAttachment) {
                        ctrl.datasource.push({ Entity: conditionalAttachment });
                    };

                    VR_Invoice_InvoiceAttachmentService.addConditionalAttachment(onGridConditionalAttachmentAdded, getContext());
                };
                ctrl.removeConditionalAttachment = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
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
                            if (invoiceFileConverter != undefined && invoiceFileConverter.ConditionalAttachments != undefined) {
                                for (var i = 0; i < invoiceFileConverter.ConditionalAttachments.length; i++) {
                                    var conditionalAttachment = invoiceFileConverter.ConditionalAttachments[i];
                                    ctrl.datasource.push({ Entity: conditionalAttachment });
                                }
                            }
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var conditionalAttachments;
                    if (ctrl.datasource != undefined) {
                        conditionalAttachments = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            conditionalAttachments.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ConditionalInvoiceFile ,Vanrise.Invoice.MainExtensions",
                        ConditionalAttachments: conditionalAttachments,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editConditionalAttachment,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editConditionalAttachment(conditionalAttachment) {
                var onGridConditionalAttachmentUpdated = function (conditionalAttachmentObj) {
                    var index = ctrl.datasource.indexOf(conditionalAttachment);
                    ctrl.datasource[index] = { Entity: conditionalAttachmentObj };
                };

                VR_Invoice_InvoiceAttachmentService.editConditionalAttachment(conditionalAttachment.Entity, onGridConditionalAttachmentUpdated, getContext());
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