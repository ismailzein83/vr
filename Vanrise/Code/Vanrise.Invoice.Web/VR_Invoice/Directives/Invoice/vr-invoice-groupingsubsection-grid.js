"use strict";

app.directive("vrInvoiceGroupingsubsectionGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemAPIService", "VRUIUtilsService","VRCommon_GridWidthFactorEnum","VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemAPIService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VR_Invoice_InvoiceTypeAPIService) {

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
                                    var itemGrouping = UtilsService.getItemByVal(payload.invoiceItemGroupings, itemGroupingId, "ItemGroupingId");
                                    if (payload.settings.Settings.GridDimesions != undefined)
                                    {
                                        for (var i = 0; i < payload.settings.Settings.GridDimesions.length; i++) {
                                            var dimension = payload.settings.Settings.GridDimesions[i];
                                            var dimensionItem = UtilsService.getItemByVal(itemGrouping.DimensionItemFields, dimension.DimensionId, "DimensionItemFieldId");
                                            input.GridColumns.push({
                                                Header: dimension.Header,
                                                FieldName: dimensionItem.FieldName,
                                                WidthFactor: dimension.WidthFactor,
                                                FieldType: dimensionItem.FieldType,
                                                Type: "Dimension"
                                            });
                                        }
                                    }
                                    if (payload.settings.Settings.GridMeasures != undefined) {
                                        for (var i = 0; i < payload.settings.Settings.GridMeasures.length; i++) {
                                            var measure = payload.settings.Settings.GridMeasures[i];
                                            var measureItem = UtilsService.getItemByVal(itemGrouping.AggregateItemFields, measure.MeasureId, "AggregateItemFieldId");
                                            input.GridColumns.push({
                                                Header:measure.Header,
                                                FieldName:measureItem.FieldName,
                                                WidthFactor:measure.WidthFactor,
                                                FieldType:measureItem.FieldType,
                                                Type:"Measure"
                                            });
                                        }
                                    }
                                    VR_Invoice_InvoiceTypeAPIService.ConvertToGridColumnAttribute(input).then(function (response) {
                                        buildGridFields(response, input.GridColumns);

                                        drillDownTabs.length = 0;
                                        if (payload != undefined && payload.settings != undefined && payload.settings.Settings != undefined) {
                                            buildGridSubSections(payload.settings.Settings.SubSections);
                                        }
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
                    return VR_Invoice_InvoiceItemAPIService.GetFilteredGroupingInvoiceItems(dataRetrievalInput)
                        .then(function (response) {
                        
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
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

            function buildGridSubSections(subSections) {
                if (subSections != undefined) {
                    for (var i = 0; i < subSections.length ; i++) {
                        var subSection = subSections[i];
                        var tab = buildInvoiceItemsTab(subSection);
                        if (tab != undefined)
                            drillDownTabs.push(tab);
                    }
                    function buildInvoiceItemsTab(subSection) {
                
                        var invoiceItemsTab = {};
                        invoiceItemsTab.title = subSection.SectionTitle;
                        invoiceItemsTab.directive = "vr-invoice-groupingsubsection-grid";
                        invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoiceItem) {
                            invoiceItem.invoiceItemGridAPI = invoiceItemGridAPI;
                            var invoiceItemGridPayload = {
                                query: {
                                    InvoiceId: invoiceId,
                                    InvoiceTypeId: invoiceTypeId,
                                    ItemGroupingId: itemGroupingId
                                },
                                settings: subSection,
                                invoiceId: invoiceId,
                                itemGroupingId: itemGroupingId,
                                invoiceItemGroupings: invoiceItemGroupings
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