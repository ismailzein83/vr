"use strict";

app.directive("vrInvoicetypeInvoiceattachments", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceAttachmentService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceAttachmentService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceAttachments($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceAttachments/Templates/InvoiceAttachmentsTemplate.html"

        };

        function InvoiceAttachments($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.infoType = "MainTemplate";
                ctrl.addAttachment = function () {
                    var onAttachmentAdded = function (attachment) {
                        ctrl.datasource.push({ Entity: attachment });
                    };

                    VR_Invoice_InvoiceAttachmentService.addAttachment(onAttachmentAdded, getContext());
                };
                ctrl.removeAttachment = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceAttachmentsEntity;
                    if (payload != undefined) {
                        invoiceAttachmentsEntity = payload.invoiceAttachments;
                        context = payload.context;
                        console.log(context);
                        if (invoiceAttachmentsEntity != undefined) {
                            for (var i = 0; i < invoiceAttachmentsEntity.length; i++) {
                                var attachment = invoiceAttachmentsEntity[i];
                                ctrl.datasource.push({ Entity: attachment });
                            }
                        }
                        if (invoiceActionEntity != undefined)
                            $scope.infoType = invoiceActionEntity.InfoType;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var attachments;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        attachments = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            attachments.push(currentItem.Entity);
                        }
                    }
                    return attachments;
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

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAttachment,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAttachment(attachmentObj) {
                var onAttachmentUpdated = function (attachment) {
                    var index = ctrl.datasource.indexOf(attachmentObj);
                    ctrl.datasource[index] = { Entity: attachment };
                };
                VR_Invoice_InvoiceAttachmentService.editAttachment(attachmentObj.Entity, onAttachmentUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);