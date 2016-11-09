"use strict";

app.directive("vrInvoiceSubsectionGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemAPIService", "VRUIUtilsService","VRCommon_GridWidthFactorEnum","VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemAPIService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VR_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var subSectionGrid = new SubSectionGrid($scope, ctrl, $attrs);
                subSectionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/SubSectionGridTemplate.html"

        };

        function SubSectionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridWidthFactors = [];
            var drillDownManager;
            var drillDownTabs = [];
            var subSectionConfigs = [];

            var invoiceTypeId;
            var invoiceId;
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
                            console.log(payload);
                            var query = {};
                            if (payload != undefined) {
                                invoiceTypeId = payload.invoiceTypeId;
                                invoiceId = payload.invoiceId;
                                var promiseDeferred = UtilsService.createPromiseDeferred();
                                if (payload.query != undefined)
                                    query = payload.query;
                                if (payload.settings != undefined) {
                                    if (query.ItemSetName == undefined)
                                        query.ItemSetName = payload.settings.ItemSetName;
                                    query.ItemSetNameParts = payload.settings.ItemSetNameParts;
                                    var input = {
                                        GridColumns: payload.settings.GridColumns
                                    };
                                    VR_Invoice_InvoiceTypeAPIService.CovertToGridColumnAttribute(input).then(function (response) {
                                        buildGridFields(response, payload.settings.GridColumns);

                                        drillDownTabs.length = 0;
                                            if (payload != undefined) {
                                                buildGridSubSections(payload.settings.SubSections);
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
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceItemAPIService.GetFilteredInvoiceItems(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                console.log(response.Data);
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
                            gridAttribute.Field = "Items["+i+"].Description";
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
                }
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
                        console.log(subSection);
                        var invoiceItemsTab = {};
                        invoiceItemsTab.title = subSection.SectionTitle;
                        invoiceItemsTab.directive = "vr-invoice-subsection-grid";
                        invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoiceItem) {
                            console.log(invoiceItem);
                            invoiceItem.invoiceItemGridAPI = invoiceItemGridAPI;
                            var invoiceItemGridPayload = {
                                query: {
                                    InvoiceId: invoiceId,
                                    ItemSetName: invoiceItem.Entity.ItemSetName,
                                    InvoiceItemDetails: invoiceItem.Entity.Details,
                                    UniqueSectionID: subSection.UniqueSectionID,
                                    InvoiceTypeId: invoiceTypeId
                                },
                                settings: subSection.Settings,
                                invoiceId: invoiceId,
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