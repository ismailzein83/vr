"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactionSaveinvoicetofileRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceActionService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SaveInvoiceToFileActionRuntime($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/SaveInvoiceToFileRuntimeTemplate.html"

        };

        function SaveInvoiceToFileActionRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            var invoiceAttachments;
            var isAutomatic;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.scopeModel.validateAttachments = function () {
                    if (!isAutomatic) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var dataItem = ctrl.datasource[i];
                            if (dataItem.Entity.IsEnabled)
                                return null;
                        }
                        return "Options not specified.";
                    }
                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};
           
                api.load = function (payload) {
                    var emailActionSettings;
                    var promises = [];
                    var actionValueSettings;
                    if (payload != undefined) {
                        invoiceAttachments = payload.invoiceAttachments;
                        emailActionSettings = payload.emailActionSettings;
                        actionValueSettings = payload.actionValueSettings;
                        context = payload.context;
                        isAutomatic = payload.isAutomatic;
                        $scope.scopeModel.isPathRequired = !payload.isAutomatic;
                        if (emailActionSettings != undefined)
                        {
                            if (emailActionSettings.InvoiceToFileActionSets != undefined) {
                                for (var i = 0; i < emailActionSettings.InvoiceToFileActionSets.length; i++) {
                                    var invoiceToFileActionSet = emailActionSettings.InvoiceToFileActionSets[i];
                                    var invoiceToFileActionSetPayload = {
                                        payload: invoiceToFileActionSet,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    promises.push(invoiceToFileActionSetPayload.loadPromiseDeferred.promise);
                                    var invoiceToFileActionSetValuePayload;
                                    if (actionValueSettings != undefined && actionValueSettings.InvoiceToFileActionSets != undefined)
                                    {
                                        invoiceToFileActionSetValuePayload = UtilsService.getItemByVal(actionValueSettings.InvoiceToFileActionSets, invoiceToFileActionSet.InvoiceToFileActionSetId, "InvoiceToFileActionSetId");
                                    }
                                    addDataItemAPI(invoiceToFileActionSetPayload, invoiceToFileActionSetValuePayload);
                                }
                            }
                        }
                        if(actionValueSettings != undefined)
                        {
                            $scope.scopeModel.locationPath = actionValueSettings.LocationPath;

                        }
                       
                    }
                    function addDataItemAPI(invoiceToFileActionSetPayload, invoiceToFileActionSetValuePayload)
                    {
                        var invoiceToFileActionSet = {
                            SetName: invoiceToFileActionSetPayload.payload.Name,
                            InvoiceToFileActionSetId: invoiceToFileActionSetPayload.payload.InvoiceToFileActionSetId,
                            IsEnabled: invoiceToFileActionSetValuePayload != undefined ? invoiceToFileActionSetValuePayload.IsEnabled : undefined,
                        };
                        invoiceToFileActionSet.onInvoiceAttachmentSelectorReady = function (api) {
                            invoiceToFileActionSet.invoiceAttachmentSelectorAPI = api;
                            invoiceToFileActionSetPayload.readyPromiseDeferred.resolve();
                        };
                        var attachmentIds;
                        if (invoiceToFileActionSetValuePayload != undefined && invoiceToFileActionSetValuePayload.Attachments != undefined)
                        {
                            attachmentIds = [];
                            for (var i = 0; i < invoiceToFileActionSetValuePayload.Attachments.length; i++)
                            {
                                var attachment = invoiceToFileActionSetValuePayload.Attachments[i];
                                attachmentIds.push(attachment.AttachmentId);
                            }
                        }
                        invoiceToFileActionSetPayload.readyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                context: getContext(invoiceToFileActionSetPayload.payload.AttachmentsIds),
                                selectedIds: attachmentIds
                            };
                            VRUIUtilsService.callDirectiveLoad(invoiceToFileActionSet.invoiceAttachmentSelectorAPI, directivePayload, invoiceToFileActionSetPayload.loadPromiseDeferred);
                        });
                        ctrl.datasource.push({ Entity: invoiceToFileActionSet });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var invoiceToFileActionSets = [];
                    for (var i = 0, length = ctrl.datasource.length; i < length; i++) {
                        var invoiceToFileActionSet = ctrl.datasource[i];
                        var attachmentIds = invoiceToFileActionSet.Entity.invoiceAttachmentSelectorAPI.getSelectedIds();
                        var attachments = [];
                        if (attachmentIds != undefined) {
                            for (var j = 0; j < attachmentIds.length ; j++) {
                                attachments.push({
                                    AttachmentId: attachmentIds[j]
                                });
                            }
                        }
                        invoiceToFileActionSets.push({
                            InvoiceToFileActionSetId: invoiceToFileActionSet.Entity.InvoiceToFileActionSetId,
                            Attachments: attachments,
                            IsEnabled: invoiceToFileActionSet.Entity.IsEnabled
                        });
                    }

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions.AutomaticSaveInvoiceToFileActionRuntimeSettings ,Vanrise.Invoice.MainExtensions",
                        InvoiceToFileActionSets: invoiceToFileActionSets,
                        LocationPath: $scope.scopeModel.locationPath
                    };

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext(attachmentsIds) {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.AttachmentsIds = attachmentsIds;
                currentContext.getInvoiceAttachmentsInfo = function () {
                    var invoiceAttachmentsInfo = [];
                    if (currentContext.AttachmentsIds != null) {
                        for (var i = 0, length = currentContext.AttachmentsIds.length; i < length; i++) {
                            var attachmentsId = currentContext.AttachmentsIds[i];
                            if (invoiceAttachments != null) {
                                var invoiceAttachment = UtilsService.getItemByVal(invoiceAttachments, attachmentsId, "InvoiceAttachmentId");
                                if (invoiceAttachment != null) {
                                    invoiceAttachmentsInfo.push({
                                        InvoiceAttachmentId: attachmentsId,
                                        Title: invoiceAttachment.Title
                                    });
                                }
                            }
                        }
                    }
                    return invoiceAttachmentsInfo;
                };
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);