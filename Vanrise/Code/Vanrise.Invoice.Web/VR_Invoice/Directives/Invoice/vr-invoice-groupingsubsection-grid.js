"use strict";

app.directive("vrInvoiceGroupingsubsectionGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_ItemGroupingSectionAPIService", "VRUIUtilsService", "VRCommon_GridWidthFactorEnum", "VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_ItemGroupingSectionAPIService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VR_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var subSectionGrid = new GroupingSubSectionGrid($scope, ctrl, $attrs);
                subSectionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/GroupingSubSectionGridTemplate.html"

        };

        function GroupingSubSectionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridWidthFactors = [];
            var drillDownManager;
            var drillDownTabs = [];
            var subSectionConfigs = [];
            var itemGroupingId;
            var invoiceTypeId;
            var invoiceId;
            var invoiceItemGroupings;
            var parentFilter;
            var subSections;
            function initializeController() {
                gridWidthFactors = UtilsService.getArrayEnum(VRCommon_GridWidthFactorEnum);
                $scope.datastore = [];
                $scope.gridFields = [];
                $scope.showExpandIcon = function (dataItem) {
                    return (dataItem.drillDownExtensionObject != null && dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0);
                };
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.load = function (payload) {
                            var query = {};
                            if (payload != undefined) {
                                invoiceTypeId = payload.invoiceTypeId;
                                invoiceId = payload.invoiceId;
                                parentFilter = payload.ParentFilter;
                                var promiseDeferred = UtilsService.createPromiseDeferred();
                                if (payload.query != undefined)
                                    query = payload.query;

                                if (payload.settings.ItemGroupingId != undefined) {
                                    itemGroupingId = payload.settings.ItemGroupingId;
                                   

                                } else {
                                    itemGroupingId = payload.itemGroupingId;
                                }
                                if (payload.settings != undefined) {
                                    query.ItemGroupingId = itemGroupingId;
                                    invoiceItemGroupings = payload.invoiceItemGroupings;
                                    var input = {
                                        GridColumns: []
                                    };
                                    if (payload != undefined && payload.settings != undefined && payload.settings.Settings != undefined) {
                                       subSections = payload.settings.Settings.SubSections;
                                    }
                                    var itemGrouping = UtilsService.getItemByVal(payload.invoiceItemGroupings, itemGroupingId, "ItemGroupingId");
                                    if (payload.settings.Settings.GridDimesions != undefined)
                                    {
                                        query.DimensionIds = [];
                                        for (var i = 0; i < payload.settings.Settings.GridDimesions.length; i++) {
                                            var dimension = payload.settings.Settings.GridDimesions[i];
                                            var dimensionItem = UtilsService.getItemByVal(itemGrouping.DimensionItemFields, dimension.DimensionId, "DimensionItemFieldId");
                                            input.GridColumns.push({
                                                ID: dimension.DimensionId,
                                                Header: dimension.Header,
                                                FieldName: dimensionItem.FieldName,
                                                WidthFactor: dimension.WidthFactor,
                                                FieldType: dimensionItem.FieldType,
                                                Type: "Dimension"
                                            });
                                            query.DimensionIds.push(dimension.DimensionId);
                                        } 
                                    }
                                    if (payload.settings.Settings.GridMeasures != undefined) {
                                        query.MeasureIds = [];
                                        for (var i = 0; i < payload.settings.Settings.GridMeasures.length; i++) {
                                            var measure = payload.settings.Settings.GridMeasures[i];
                                            var measureItem = UtilsService.getItemByVal(itemGrouping.AggregateItemFields, measure.MeasureId, "AggregateItemFieldId");
                                            input.GridColumns.push({
                                                ID:measure.MeasureId,
                                                Header:measure.Header,
                                                FieldName:measureItem.FieldName,
                                                WidthFactor:measure.WidthFactor,
                                                FieldType:measureItem.FieldType,
                                                Type:"Measure"
                                            });
                                            query.MeasureIds.push(measure.MeasureId);
                                        }
                                    }
                                    VR_Invoice_InvoiceTypeAPIService.ConvertToGridColumnAttribute(input).then(function (response) {
                                        buildGridFields(response, input.GridColumns);

                                        drillDownTabs.length = 0;
                                        
                                        drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, []);
                                        gridAPI.retrieveData(query).then(function () {
                                            promiseDeferred.resolve();
                                        }).catch(function (error) {
                                            promiseDeferred.reject(error);
                                        });
                                    }).catch(function (error) {
                                        promiseDeferred.reject(error);
                                    });
                                }
                                return promiseDeferred.promise;

                            }
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_ItemGroupingSectionAPIService.GetFilteredGroupingInvoiceItems(dataRetrievalInput)
                        .then(function (response) {
                        
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    buildGridSubSections(subSections, response.Data[i].SubSectionsIds);
                                    drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }
            function buildGridFields(gridAttributes, gridColumns) {

                $scope.gridFields.length = 0;
                if (gridAttributes != undefined && gridColumns != undefined) {
                    for (var i = 0; i < gridColumns.length ; i++) {
                        var gridColumn = gridColumns[i];
                        var gridAttribute = UtilsService.getItemByVal(gridAttributes, gridColumn.FieldName, "Field");
                        if (gridAttribute != undefined)
                        {
                            gridAttribute.ID = gridColumn.ID;
                            gridAttribute.ItemType = gridColumn.Type;
                            if (gridColumn.Type == "Dimension")
                            {
                                gridAttribute.Field = "DimensionValues[" + i + "].Name";
                               
                            } else
                            {
                                gridAttribute.Field = "MeasureValues." + gridColumn.FieldName + ".Value";
                            }
                      
                            var gridWidthFactor = UtilsService.getItemByVal(gridWidthFactors, gridColumn.WidthFactor, "value");
                            if (gridWidthFactor != undefined)
                                gridAttribute.WidthFactor = gridWidthFactor.widthFactor;
                            $scope.gridFields.push(gridAttribute);
                        }
                    }
                }
            }
            function defineMenuActions() {
                var defaultMenuActions = [];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function buildGridSubSections(subSections, subSectionsIds) {
                if (subSections != undefined) {
                    for (var i = 0; i < subSectionsIds.length; i++)
                    {
                        var subSectionsId=subSectionsIds[i];
                        for (var i = 0; i < subSections.length ; i++) {

                            var subSection = subSections[i];
                            if(subSectionsId == subSection.InvoiceSubSectionId)
                            {
                                var tab = buildInvoiceItemsTab(subSection);
                                if (tab != undefined)
                                    drillDownTabs.push(tab);
                            }
                           
                        }
                    }
                   
                    function buildInvoiceItemsTab(subSection) {
                        var invoiceItemsTab = {};
                        invoiceItemsTab.title = subSection.SectionTitle;
                        invoiceItemsTab.directive = "vr-invoice-groupingsubsection-grid";
                        invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoiceItem) {
                            var filters = [];
                            if (parentFilter != undefined)
                            {
                                filters = parentFilter;
                            }
                            var dimensionCounter = 0;
                            for (var i = 0; i < $scope.gridFields.length; i++)
                            {
                                var gridField = $scope.gridFields[i];
                                if (gridField.ItemType == "Dimension")
                                {
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
                        return invoiceItemsTab;
                    }
                }
            }
        }

        return directiveDefinitionObject;

    }
]);