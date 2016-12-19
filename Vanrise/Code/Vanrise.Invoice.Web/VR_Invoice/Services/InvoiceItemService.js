"use strict";
app.service('VR_Invoice_InvoiceItemService', ['VRModalService', 'SecurityService', 'UtilsService', 'VRUIUtilsService','VRNotificationService', 
    function (VRModalService, SecurityService, UtilsService, VRUIUtilsService, VRNotificationService) {


        function defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSectionIds, subSectionConfigs, gridFields, parentFilter, invoiceId, invoiceTypeId, itemGroupingId, invoiceItemGroupings) {
            if (subSectionIds == null)
                return;
            var drillDownTabs = [];

            for (var i = 0; i < subSectionIds.length; i++) {
                var subSectionId = subSectionIds[i];
                for (var i = 0; i < subSectionConfigs.length ; i++) {
                    var subSection = subSectionConfigs[i];
                    if (subSectionId == subSection.InvoiceSubSectionId) {
                        addDrillDownTab(subSection);
                    }
                }
            }

            setDrillDownTabs();
            setMenuActions();
            function addDrillDownTab(subSection) {
                var invoiceItemsTab = {};
                invoiceItemsTab.title = subSection.SectionTitle;
                invoiceItemsTab.directive = "vr-invoice-groupingsubsection-grid";
                invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoiceItem) {
                    var filters = [];
                    if (parentFilter != undefined) {
                        filters = parentFilter;
                    }
                    var dimensionCounter = 0;
                    for (var i = 0; i < gridFields.length; i++) {
                        var gridField = gridFields[i];
                        if (gridField.ItemType == "Dimension") {
                            filters.push({
                                DimensionId: gridField.ID,
                                FilterValue: invoiceItem.DimensionValues[dimensionCounter].Value
                            });
                            dimensionCounter++;
                        }
                    }
                    invoiceItem.invoiceItemGridAPI = invoiceItemGridAPI;
                    var invoiceItemGridPayload = {
                        query: {
                            InvoiceId: invoiceId,
                            InvoiceTypeId: invoiceTypeId,
                            ItemGroupingId: itemGroupingId,
                            Filters: filters,
                            ParentGroupingInvoiceItemDetails: invoiceItem,
                            SectionId: subSection.InvoiceSubSectionId
                        },
                        settings: subSection,
                        invoiceId: invoiceId,
                        itemGroupingId: itemGroupingId,
                        invoiceItemGroupings: invoiceItemGroupings,
                        ParentFilter: filters
                    };
                    return invoiceItem.invoiceItemGridAPI.load(invoiceItemGridPayload);
                };
                invoiceItemsTab.parentMenuActions = [];
                drillDownTabs.push(invoiceItemsTab);


            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, undefined);
                drillDownManager.setDrillDownExtensionObject(dataItem);
            }
            function setMenuActions() {
                dataItem.menuActions = [];
            }

        }

        return ({
            defineInvoiceTabsAndMenuActions: defineInvoiceTabsAndMenuActions,
        });
    }]);
