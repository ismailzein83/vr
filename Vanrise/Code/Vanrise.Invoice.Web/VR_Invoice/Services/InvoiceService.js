
app.service('VR_Invoice_InvoiceService', ['VRModalService','SecurityService','UtilsService','VRUIUtilsService',
    function (VRModalService, SecurityService, UtilsService, VRUIUtilsService) {

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
                ActionTypeName: "Download",
                actionMethod: function (payload) {

                    var context = {
                        $type: "Vanrise.Invoice.Business.PhysicalInvoiceActionContext,Vanrise.Invoice.Business",
                        InvoiceId: payload.invoice.Entity.InvoiceId
                    };

                    var paramsurl = "";
                    paramsurl += "invoiceActionContext=" + UtilsService.serializetoJson(context);
                    paramsurl += "&actionTypeName=" + "Download";
                    paramsurl += "&Auth-Token=" + encodeURIComponent(SecurityService.getUserToken());
                    window.open("Client/Modules/VR_Invoice/Reports/InvoiceReport.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
                }
            };
            registerActionType(actionType);
        }

        function defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs) {
            if (subSections == null)
                return;
            
            var drillDownTabs = [];
            var menuActions = [];

            for (var i = 0; i < subSections.length; i++) {
                var subSection = subSections[i];
                if (dataItem.SectionTitle == subSection.SectionTitle)
                    addDrillDownTab(subSection);
            }

            setDrillDownTabs();

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
                            },
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
        }
        
        return ({
            generateInvoice: generateInvoice,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport,
            defineInvoiceTabsAndMenuActions: defineInvoiceTabsAndMenuActions
        });
    }]);
