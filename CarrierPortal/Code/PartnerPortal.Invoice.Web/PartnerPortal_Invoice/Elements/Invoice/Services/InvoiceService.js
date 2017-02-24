"use strict";
app.service('PartnerPortal_Invoice_InvoiceService', ['VRModalService', 'SecurityService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'PartnerPortal_Invoice_InvoiceViewerTypeGridActionService',
    function (VRModalService, SecurityService, UtilsService, VRUIUtilsService, VRNotificationService, PartnerPortal_Invoice_InvoiceViewerTypeGridActionService) {

        function defineInvoiceTabsAndMenuActions(dataItem, gridAPI, invoiceTypeId, invoiceViewerTypeId, gridActions) {
            console.log(gridActions);
            if (gridActions == null)
                return;

            if (dataItem.menuActions != undefined)
                dataItem.menuActions.length = 0;

            setMenuActions();
          
            function setMenuActions() {
                dataItem.menuActions = [];

                if (dataItem.ActionsIds != undefined) {
                    for (var j = 0; j < dataItem.ActionsIds.length; j++) {
                        var actionId = dataItem.ActionsIds[j];
                        var invoiceAction = UtilsService.getItemByVal(gridActions, actionId, "InvoiceViewerTypeGridActionId");
                        if (invoiceAction != undefined) {
                            var actionType = PartnerPortal_Invoice_InvoiceViewerTypeGridActionService.getActionTypeIfExist(invoiceAction.Settings.ActionTypeName);
                            if (actionType != undefined) {
                                addgridMenuAction(actionId, invoiceAction, actionType);
                            }
                        }
                    }
                }

                function addgridMenuAction(actionId, invoiceAction, actionType) {
                    dataItem.menuActions.push({
                        name: invoiceAction.Title,
                        clicked: function (dataItem) {
                            var payloadContext = {
                                invoice: dataItem,
                                invoiceAction: invoiceAction,
                                invoiceViewerTypeId:invoiceViewerTypeId,
                                onItemAdded: function (item) {
                                    defineInvoiceTabsAndMenuActions(item, gridAPI, invoiceTypeId, invoiceViewerTypeId, gridActions);
                                    gridAPI.itemAdded(item);
                                },
                                onItemUpdated: function (item) {
                                    defineInvoiceTabsAndMenuActions(item, gridAPI, invoiceTypeId, invoiceViewerTypeId, gridActions);
                                    gridAPI.itemUpdated(item);
                                },
                                onItemDeleted: function (item) {
                                    gridAPI.itemDeleted(item);
                                }
                            };
                            var promiseDeffered = UtilsService.createPromiseDeferred();

                            var promise = actionType.actionMethod(payloadContext);
                            if (promise != undefined && promise.then != undefined) {
                                promise.then(function (response) {
                                        promiseDeffered.resolve();
                                }).catch(function (error) {
                                    promiseDeffered.reject(error);
                                }).finally(function () {
                                    gridAPI.hideLoader();
                                });
                            } else {
                                gridAPI.hideLoader();
                                promiseDeffered.resolve();
                            }
                            return promiseDeffered.promise;
                        }
                    });
                }
            }
        }

        return ({
            defineInvoiceTabsAndMenuActions: defineInvoiceTabsAndMenuActions
        });
    }]);



