
app.service('VR_Invoice_InvoiceActionService', ['VRModalService', 'UtilsService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'SecurityService', 'FileAPIService','VRCommon_VRTempPayloadAPIService','InsertOperationResultEnum',
    function (VRModalService, UtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, SecurityService, FileAPIService, VRCommon_VRTempPayloadAPIService, InsertOperationResultEnum) {

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

                    var promiseDeffered = UtilsService.createPromiseDeferred();


                    var varTempPayload = getInvoiceActionContext(payload);
                    VRCommon_VRTempPayloadAPIService.AddVRTempPayload(varTempPayload).then(function (response) {
                        promiseDeffered.resolve();
                        if (response.Result == InsertOperationResultEnum.Succeeded.value) {
                            var tempPayloadId = response.InsertedObject;

                            var paramsurl = "";
                            paramsurl += "tempPayloadId=" + tempPayloadId;
                            paramsurl += "&actionTypeName=" + "OpenRDLCReportAction";
                            paramsurl += "&actionId=" + payload.invoiceAction.InvoiceActionId;

                            var screenWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                            var left = ((screenWidth / 2) - (1000 / 2));

                            window.open("Client/Modules/VR_Invoice/Reports/InvoiceReport.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1,top = 125, left = " + left + "");
                        }
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });
                    return promiseDeffered.promise;
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
                            VR_Invoice_InvoiceAPIService.SetInvoicePaid(payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceId, payload.invoiceAction.Settings.IsInvoicePaid).then(function (response) {
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
                            VR_Invoice_InvoiceAPIService.SetInvoiceLocked(payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceId, payload.invoiceAction.Settings.SetLocked).then(function (response) {
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

        function getInvoiceActionContext(payload) {

            var context;
            if (payload.isPreGenerateAction) {
                context = {
                    $type: "Vanrise.Invoice.Business.PreviewInvoiceActionContext,Vanrise.Invoice.Business",
                    InvoiceTypeId: payload.generatorEntity.invoiceTypeId,
                    PartnerId: payload.generatorEntity.partnerId,
                    FromDate: payload.generatorEntity.fromDate,
                    ToDate: payload.generatorEntity.toDate,
                    IssueDate: payload.generatorEntity.issueDate,
                    CustomSectionPayload: payload.generatorEntity.customSectionPayload,
                };
            } else {
                context = {
                    $type: "Vanrise.Invoice.Business.PhysicalInvoiceActionContext,Vanrise.Invoice.Business",
                    InvoiceId: payload.invoice.Entity.InvoiceId,
                };
            }

            return {
                Settings: {
                    $type: "Vanrise.Invoice.Entities.InvoiceActionPayloadSettings,Vanrise.Invoice.Entities",
                    Context: context
                }
            };
        }

        function registerRecreateAction() {
            var actionType = {
                ActionTypeName: "RecreateInvoiceAction",
                actionMethod: function (payload) {
                    var onGenerateInvoice = function (invoiceGenerated) {
                        if (payload.onItemAdded != undefined) {
                            payload.onItemAdded(invoiceGenerated);
                        }
                        if (payload.onItemDeleted != undefined) {
                            payload.onItemDeleted(payload.invoice);
                        }
                    };
                    reGenerateInvoice(onGenerateInvoice, payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceTypeId, payload.invoice.Entity.InvoiceId);
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
                    openInvoiceNote(onNoteAdded, payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId);
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }
        function registerSendEmailAction() {
            var actionType = {
                ActionTypeName: "SendEmailAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();

                    var onInvoiceEmailSend = function (response) {
                        promiseDeffered.resolve(true);
                    };
                    openEmailTemplate(onInvoiceEmailSend, payload.invoice.Entity.InvoiceId, payload.invoiceAction.InvoiceActionId, payload.invoice.Entity.InvoiceTypeId);
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }
        function openInvoiceNote(onInvoiceNoteAdded, invoiceId, invoiceActionId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceNoteAdded = onInvoiceNoteAdded;
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceActionId: invoiceActionId
            };
            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/InvoiceNoteActionEditor.html', parameters, settings);
        }
        function openEmailTemplate(onInvoiceEmailSend, invoiceId, invoiceActionId, invoiceTypeId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onInvoiceEmailSend = onInvoiceEmailSend;
            };
            var parameters = {
                invoiceId: invoiceId,
                invoiceActionId: invoiceActionId,
                invoiceTypeId: invoiceTypeId
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
            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/Templates/InvoiceActionEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/Templates/InvoiceActionEditor.html', parameters, settings);
        }
        function registerDownloadFileInvoiceAction() {
            var actionType = {
                ActionTypeName: "DownloadFileInvoiceAction",
                actionMethod: function (payload) {
                    if (payload.invoice.Entity.Settings != undefined && payload.invoice.Entity.Settings.FileId != undefined)
                        FileAPIService.DownloadFile(payload.invoice.Entity.Settings.FileId).then(function (response) {
                            if (response != undefined)
                                UtilsService.downloadFile(response.data, response.headers);
                        });
                }
            };
            registerActionType(actionType);
        }

        function reGenerateInvoice(onGenerateInvoice, invoiceActionId, invoiceTypeId, invoiceId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceActionId: invoiceActionId,
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

        function generateInvoices(onGenerateInvoice, invoiceTypeId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId,
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/InvoiceGenerationProcessEditor.html', parameters, settings);
        }

        function registerSetInvoiceDeletedAction() {
            var actionType = {
                ActionTypeName: "SetInvoiceDeletedAction",
                actionMethod: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            VR_Invoice_InvoiceAPIService.DeleteGeneratedInvoice(payload.invoiceAction.InvoiceActionId,payload.invoice.Entity.InvoiceId).then(function (response) {
                                if (payload.onItemDeleted != undefined) {
                                    payload.onItemDeleted(payload.invoice);
                                }
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
        return ({
            addInvoiceAction: addInvoiceAction,
            editInvoiceAction: editInvoiceAction,
            registerSetInvoicePaidAction: registerSetInvoicePaidAction,
            registerSetInvoiceLockedAction: registerSetInvoiceLockedAction,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport,
            registerInvoiceNoteAction: registerInvoiceNoteAction,
            registerRecreateAction: registerRecreateAction,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            generateInvoice: generateInvoice,
            generateInvoices: generateInvoices,
            reGenerateInvoice: reGenerateInvoice,
            registerSendEmailAction: registerSendEmailAction,
            registerDownloadFileInvoiceAction: registerDownloadFileInvoiceAction,
            registerSetInvoiceDeletedAction: registerSetInvoiceDeletedAction
        });
    }]);
