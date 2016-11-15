
app.service('VR_Invoice_InvoiceService', ['VRModalService','SecurityService','UtilsService','VRUIUtilsService','VR_Invoice_InvoiceAPIService','VRNotificationService',
    function (VRModalService, SecurityService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceAPIService, VRNotificationService) {

        var actionTypes = [];
        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function getActionTypeIfExist(actionTypeName)
        {
            for(var i=0;i<actionTypes.length;i++)
            {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function generateInvoice(onGenerateInvoice, invoiceTypeId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/Runtime/GenerateInvoiceEditor.html', parameters, settings);
        }

        function registerInvoiceRDLCReport() {
            
            var actionType = {
                ActionTypeName: "OpenRDLCReportAction",
                actionMethod: function (payload) {
                    var context = {
                        $type: "Vanrise.Invoice.Business.PhysicalInvoiceActionContext,Vanrise.Invoice.Business",
                        InvoiceId: payload.invoice.Entity.InvoiceId,
                    };

                    var paramsurl = "";
                    paramsurl += "invoiceActionContext=" + UtilsService.serializetoJson(context);
                    paramsurl += "&actionTypeName=" + "OpenRDLCReportAction";
                    paramsurl += "&actionId=" + payload.invoiceGridAction.Settings.ActionId;
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
                            var context = {
                                $type: "Vanrise.Invoice.Business.PhysicalInvoiceActionContext,Vanrise.Invoice.Business",
                                InvoiceId: payload.invoice.Entity.InvoiceId
                            };
                            VR_Invoice_InvoiceAPIService.SetInvoicePaid(payload.invoice.Entity.InvoiceId, payload.invoiceGridAction.Settings.IsInvoicePaid).then(function (response) {
                                promiseDeffered.resolve(response);
                            });
                        }else
                        {
                            promiseDeffered.resolve(response);
                        }
                    });
                    return promiseDeffered.promise;
                }
            };
            registerActionType(actionType);
        }

        function defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs, invoiceTypeId) {
            if (subSections == null)
                return;
            
            var drillDownTabs = [];
            var menuActions = [];

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
                                UniqueSectionID: subSection.UniqueSectionID,
                                InvoiceTypeId: invoiceTypeId
                            },
                            invoiceTypeId: invoiceTypeId,
                            settings: subSection.Settings,
                            invoiceId: invoice.Entity.InvoiceId,
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
                    var actionType = getActionTypeIfExist(invoiceGridAction.Settings.ActionTypeName);
                    if (actionType != undefined) {
                        addgridMenuAction(invoiceGridAction, actionType);
                    }
                   
                }
                function addgridMenuAction(invoiceGridAction, actionType) {
                    dataItem.menuActions.push({
                        name: invoiceGridAction.Title,
                        clicked: function (dataItem) {
                            var payload = {
                                invoice: dataItem,
                                invoiceGridAction: invoiceGridAction
                            };
                            var promiseDeffered = UtilsService.createPromiseDeferred();

                            var promise = actionType.actionMethod(payload);
                            if (promise != undefined && promise.then != undefined) {
                              //  ctrl.isLodingGrid = true;

                                promise.then(function (response) {
                                    if (invoiceGridAction.ReloadGridItem && response) {
                                        var invoiceId = dataItem.Entity.InvoiceId;
                                        return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                                            promiseDeffered.resolve();
                                            defineInvoiceTabsAndMenuActions(response, gridAPI, subSections, subSectionConfigs);
                                            gridAPI.itemUpdated(response);
                                        }).catch(function (error) {
                                            promiseDeffered.reject(error);
                                        });
                                    } else {
                                        promiseDeffered.resolve();
                                    }
                                }).catch(function (error) {
                                    promiseDeffered.reject(error);
                                }).finally(function () {
                                    //ctrl.isLodingGrid = false;
                                });
                            } else {
                                promiseDeffered.resolve();
                            }
                            return promiseDeffered.promise;
                        }
                    });
                }
            }


        }
        
        return ({
            generateInvoice: generateInvoice,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport,
            registerSetInvoicePaidAction:registerSetInvoicePaidAction,
            defineInvoiceTabsAndMenuActions: defineInvoiceTabsAndMenuActions
        });
    }]);
