
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


        return ({
            addInvoiceAction: addInvoiceAction,
            editInvoiceAction: editInvoiceAction,
            registerSetInvoicePaidAction: registerSetInvoicePaidAction,
            registerSetInvoiceLockedAction:registerSetInvoiceLockedAction,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
        });
    }]);
