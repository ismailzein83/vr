"use strict";

app.directive("vrInvoiceGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService","VR_Invoice_InvoiceFieldEnum","VRUIUtilsService","VR_Invoice_InvoiceService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService, VR_Invoice_InvoiceService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new InvoiceGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/InvoiceGridTemplate.html"

        };

        function InvoiceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var drillDownManager;
            var drillDownTabs = [];
            var gridAPI;
            function initializeController() {

                $scope.datastore = [];
                $scope.gridMenuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            var query;
                            drillDownTabs.length = 0;
                            if (payload != undefined)
                            {
                                buildGridFields(payload.mainGridColumns);
                                buildGridSubSections(payload.subSections);
                                query = payload.query;
                                defineMenuActions(payload.invoiceGridActions);
                            }
                            drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, []);
                            return gridAPI.retrieveData(query);
                        }
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoices(dataRetrievalInput)
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
            }

            function buildGridFields(mainGridColumns)
            {
                $scope.gridFields = [];
                if (mainGridColumns != undefined) {
                    for (var i = 0; i < mainGridColumns.length ; i++) {
                        var mainGridColumn = mainGridColumns[i];
                        var field;
                        var type;
                        switch (mainGridColumn.Field) {
                            case VR_Invoice_InvoiceFieldEnum.CustomField.value:
                                field = field = "Entity." + VR_Invoice_InvoiceFieldEnum.CustomField.fieldName + "." + mainGridColumn.CustomFieldName;
                                type = VR_Invoice_InvoiceFieldEnum.CustomField.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.InvoiceId.value:
                                field = field = "Entity." + VR_Invoice_InvoiceFieldEnum.InvoiceId.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.InvoiceId.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.Partner.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.Partner.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.Partner.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.SerialNumber.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.SerialNumber.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.SerialNumber.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.FromDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.FromDate.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.FromDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.ToDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.ToDate.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.ToDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.IssueDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.IssueDate.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.IssueDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.DueDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.DueDate.fieldName;
                                type = VR_Invoice_InvoiceFieldEnum.DueDate.type;
                                break;
                        }
                        $scope.gridFields.push({ header: mainGridColumn.Header, field: field, type: type });
                    }
                }
            }

            function buildGridSubSections(subSections) {
                if (subSections != undefined) {
                    for (var i = 0; i < subSections.length ; i++) {
                        var subSection = subSections[i];
                        drillDownTabs.push(buildInvoiceItemsTab(subSection));
                    }
                    function buildInvoiceItemsTab(subSection) {
                        var invoiceItemsTab = {};

                        invoiceItemsTab.title = subSection.SectionTitle;
                        invoiceItemsTab.directive = subSection.Directive;

                        invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoice) {
                            invoice.invoiceItemGridAPI = invoiceItemGridAPI;
                            var invoiceItemGridPayload = {
                                InvoiceId: invoice.Entity.InvoiceId
                            };
                            return invoice.invoiceItemGridAPI.load(invoiceItemGridPayload);
                        };
                        invoiceItemsTab.parentMenuActions = [];
                        return invoiceItemsTab;
                    }
                }
            }

            function defineMenuActions(invoiceGridActions) {
                $scope.gridMenuActions.length = 0;
                if (invoiceGridActions != undefined)
                {
                    for(var i=0;i<invoiceGridActions.length;i++)
                    {
                        var invoiceGridAction = invoiceGridActions[i];
                        var actionType = VR_Invoice_InvoiceService.getActionTypeIfExist(invoiceGridAction.ActionTypeName);
                        if(actionType != undefined)
                        {
                            $scope.gridMenuActions.push({
                                name: invoiceGridAction.ActionTypeName,
                                clicked: function (dataItem)
                                {
                                    var payload = {
                                        invoice: dataItem
                                    };
                                    return actionType.actionMethod(payload);
                                }

                            });
                        }
                    }
                }
            }
        }

        return directiveDefinitionObject;

    }
]);