"use strict";
app.service('VR_Invoice_InvoiceService', ['VRModalService','SecurityService','UtilsService','VRUIUtilsService','VR_Invoice_InvoiceAPIService','VRNotificationService','VR_Invoice_InvoiceActionService',
    function (VRModalService, SecurityService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService, VR_Invoice_InvoiceActionService) {


        function defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions, invoiceItemGroupings) {
            if (subSections == null)
                return;
            if (dataItem.menuActions != undefined)
                dataItem.menuActions.length = 0;
            var drillDownTabs = [];

            for (var i = 0; i < subSections.length; i++) {
                var subSection = subSections[i];
                if (dataItem.SectionsTitle != undefined && UtilsService.contains(dataItem.SectionsTitle, subSection.SectionTitle))
                {
                    addDrillDownTab(subSection);
                }
            }

            setDrillDownTabs();
            setMenuActions();
            function addDrillDownTab(subSection) {
                var drillDownTab = {};
                var cofigItem = UtilsService.getItemByVal(subSectionConfigs, subSection.Settings.ConfigId, "ExtensionConfigurationId");
                if (cofigItem != undefined) {
                    drillDownTab.title = subSection.SectionTitle;
                    drillDownTab.directive = cofigItem.RuntimeEditor;
                    drillDownTab.loadDirective = function (invoiceItemGridAPI, invoice) {
                        invoice.invoiceItemGridAPI = invoiceItemGridAPI;
                        var invoiceItemGridPayload = {
                            query: {
                                InvoiceId: invoice.Entity.InvoiceId,
                                UniqueSectionID: subSection.InvoiceSubSectionId,
                                InvoiceTypeId: invoiceTypeId
                            },
                            invoiceTypeId: invoiceTypeId,
                            settings: subSection.Settings,
                            invoiceId: invoice.Entity.InvoiceId,
                            invoiceItemGroupings: invoiceItemGroupings
                        };
                        return invoice.invoiceItemGridAPI.load(invoiceItemGridPayload);
                    };
                    drillDownTabs.push(drillDownTab);
                }
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, undefined);
                drillDownManager.setDrillDownExtensionObject(dataItem);
            }
            function setMenuActions() {
                dataItem.menuActions = [];
                for (var j = 0; j < dataItem.ActionTypeNames.length; j++)
                {
                    var invoiceGridAction = dataItem.ActionTypeNames[j];
                    var invoiceAction = UtilsService.getItemByVal(invoiceActions, invoiceGridAction.InvoiceGridActionId, "InvoiceActionId");
                    if (invoiceAction != undefined)
                    {
                        var actionType = VR_Invoice_InvoiceActionService.getActionTypeIfExist(invoiceAction.Settings.ActionTypeName);
                        if (actionType != undefined) {
                            addgridMenuAction(invoiceGridAction, invoiceAction, actionType);
                        }
                    }
                   
                   
                }
                function addgridMenuAction(invoiceGridAction, invoiceAction, actionType) {
                    dataItem.menuActions.push({
                        name: invoiceGridAction.Title,
                        clicked: function (dataItem) {
                            var payload = {
                                invoice: dataItem,
                                invoiceAction: invoiceAction
                            };
                            var promiseDeffered = UtilsService.createPromiseDeferred();

                            var promise = actionType.actionMethod(payload);
                            if (promise != undefined && promise.then != undefined) {
                                promise.then(function (response) {
                                    if (invoiceGridAction.ReloadGridItem && response) {
                                        gridAPI.showLoader();
                                        var invoiceId = dataItem.Entity.InvoiceId;
                                        return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                                            gridAPI.itemUpdated(response);
                                            defineInvoiceTabsAndMenuActions(response, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions);
                                            promiseDeffered.resolve();
                                        }).catch(function (error) {
                                            promiseDeffered.reject(error);
                                        });
                                    } else {
                                        promiseDeffered.resolve();
                                    }
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

            defineInvoiceTabsAndMenuActions: defineInvoiceTabsAndMenuActions,
        });
    }]);
