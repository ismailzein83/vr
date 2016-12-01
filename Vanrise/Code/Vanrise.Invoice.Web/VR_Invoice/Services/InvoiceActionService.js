
app.service('VR_Invoice_InvoiceActionService', ['VRModalService','UtilsService','VR_Invoice_InvoiceAPIService','VRNotificationService','SecurityService',
    function (VRModalService, UtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, SecurityService) {

        var actionTypes = [];
        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }
        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }
        function registerInvoiceRDLCReport() {

            var actionType = {
                ActionTypeName: "OpenRDLCReportAction",
                actionMethod: function (payload) {
                    var context = getInvoiceActionContext(payload);
                    var paramsurl = "";
                    paramsurl += "invoiceActionContext=" + UtilsService.serializetoJson(context);
                    paramsurl += "&actionTypeName=" + "OpenRDLCReportAction";
                    paramsurl += "&actionId=" + payload.invoiceAction.InvoiceActionId;
                    paramsurl += "&Auth-Token=" + encodeURIComponent(SecurityService.getUserToken());
                   window.open("Client/Modules/VR_Invoice/Reports/InvoiceReport.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
                }
            };
            registerActionType(actionType);
        }
        function registerSetInvoicePaidAction() {
            var actionType = {
                ActionTypeName: "SetInvoicePaidAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            VR_Invoice_InvoiceAPIService.SetInvoicePaid(payload.invoice.Entity.InvoiceId, payload.invoiceAction.Settings.IsInvoicePaid).then(function (response) {
                                promiseDeffered.resolve(response);
                            });
                        } else {
                            promiseDeffered.resolve(response);
                        }
                    });
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }

        function registerSetInvoiceLockedAction() {
            var actionType = {
                ActionTypeName: "LockInvoiceAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            VR_Invoice_InvoiceAPIService.SetInvoiceLocked(payload.invoice.Entity.InvoiceId, payload.invoiceAction.Settings.SetLocked).then(function (response) {
                                promiseDeffered.resolve(response);
                            });
                        } else {
                            promiseDeffered.resolve(response);
                        }
                    });
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }

        function getInvoiceActionContext(payload)
        {
            var context;
            if(payload.isPreGenerateAction)
            {
                 context = {
                    $type: "Vanrise.Invoice.Business.PreviewInvoiceActionContext,Vanrise.Invoice.Business",
                    InvoiceTypeId: payload.generatorEntity.invoiceTypeId,
                    PartnerId: payload.generatorEntity.partnerId,
                    FromDate: payload.generatorEntity.fromDate,
                    ToDate: payload.generatorEntity.toDate,
                    IssueDate: payload.generatorEntity.issueDate
                };
            }else
            {
                 context = {
                    $type: "Vanrise.Invoice.Business.PhysicalInvoiceActionContext,Vanrise.Invoice.Business",
                    InvoiceId: payload.invoice.Entity.InvoiceId,
                };
            }
            return context;
        }

        function registerRecreateAction() {
            var actionType = {
                ActionTypeName: "RecreateInvoiceAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    var onGenerateInvoice = function (invoiceGenerated) {
                        promiseDeffered.resolve(invoiceGenerated);
                    };
                    reGenerateInvoice(onGenerateInvoice, payload.invoice.Entity.InvoiceTypeId, payload.invoice.Entity.InvoiceId);
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }

        function registerInvoiceNoteAction() {
            var actionType = {
                ActionTypeName: "InvoiceNoteAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    var onNoteAdded = function (note) {
                        promiseDeffered.resolve(note);
                    };
                    openInvoiceNote(onNoteAdded,payload.invoice.Entity.InvoiceId);
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }
        function registerSendEmailAction() {
            var actionType = {
                ActionTypeName: "SendEmailAction",
                actionMethod: function (payload) {
                    var onInvoiceEmailSend = function (response) {

                    };
                    openEmailTemplate(onInvoiceEmailSend, payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId);
                    //VRNotificationService.showConfirmation().then(function (response) {
                    //    if (response) {
                    //        VR_Invoice_InvoiceAPIService.SendEmail(payload.invoice.Entity.InvoiceId).then(function (response) {
                    //            promiseDeffered.resolve(response);
                    //        });
                    //    } else {
                    //        promiseDeffered.resolve(response);
                    //    }
                    //});
                }
            };
            registerActionType(actionType);
        }
        function openInvoiceNote(onInvoiceNoteAdded, invoiceId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceNoteAdded = onInvoiceNoteAdded;
            };
            var parameters = {
                invoiceId: invoiceId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/InvoiceNoteActionEditor.html', parameters, settings);
        }
        function openEmailTemplate(onInvoiceEmailSend, invoiceId,invoiceActionId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceEmailSend = onInvoiceEmailSend;
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceActionId: invoiceActionId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/InvoiceEmailTemplate.html', parameters, settings);
        }
        function addInvoiceAction(onInvoiceActionAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceActionAdded = onInvoiceActionAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceActions/InvoiceActionEditor.html', parameters, settings);
        }
        function editInvoiceAction(invoiceActionEntity, onInvoiceActionUpdated, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceActionUpdated = onInvoiceActionUpdated;
            };
            var parameters = {
                invoiceActionEntity: invoiceActionEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Definition/InvoiceActions/InvoiceActionEditor.html', parameters, settings);
        }

        function reGenerateInvoice(onGenerateInvoice, invoiceTypeId, invoiceId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId,
                invoiceId: invoiceId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/GenerateInvoiceEditor.html', parameters, settings);
        }
        function generateInvoice(onGenerateInvoice, invoiceTypeId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId,
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/GenerateInvoiceEditor.html', parameters, settings);
        }
        return ({
            addInvoiceAction: addInvoiceAction,
            editInvoiceAction: editInvoiceAction,
            registerSetInvoicePaidAction: registerSetInvoicePaidAction,
            registerSetInvoiceLockedAction:registerSetInvoiceLockedAction,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport,
            registerInvoiceNoteAction:registerInvoiceNoteAction,
            registerRecreateAction:registerRecreateAction,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            generateInvoice: generateInvoice,
            reGenerateInvoice: reGenerateInvoice,
            registerSendEmailAction: registerSendEmailAction
        });
    }]);
