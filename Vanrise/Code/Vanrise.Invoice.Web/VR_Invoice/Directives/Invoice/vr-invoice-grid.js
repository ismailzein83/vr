"use strict";

app.directive("vrInvoiceGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService","VR_Invoice_InvoiceFieldEnum","VRUIUtilsService","VR_Invoice_InvoiceService","VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService, VR_Invoice_InvoiceService, VR_Invoice_InvoiceTypeAPIService) {

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
            var gridAPI;
            var subSectionConfigs = [];
            var subSections = [];
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
                            var promiseDeferred = UtilsService.createPromiseDeferred();
                            VR_Invoice_InvoiceTypeAPIService.GetInvoiceUISubSectionSettingsConfigs().then(function (response) {
                                if (response != undefined)
                                {
                                    subSectionConfigs = response;
                                }
                                if (payload != undefined) {
                                    buildGridFields(payload.mainGridColumns);
                                    subSections = payload.subSections;
                                    query = payload.query;
                                    defineMenuActions(payload.invoiceGridActions);
                                }
                                gridAPI.retrieveData(query).then(function()
                                {
                                    promiseDeferred.resolve();
                                }).catch(function (error) {
                                    promiseDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                promiseDeferred.reject(error);
                            });
                            return promiseDeferred.promise;
                        }

                        directiveAPI.onGenerateInvoice = function (invoice)
                        {
                            gridAPI.itemAdded(invoice);
                        }

                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoices(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var dataItem = response.Data[i];
                                    VR_Invoice_InvoiceService.defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs);

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
                        var attribute = {
                            type: undefined,
                            numberprecision: undefined,
                        };
                        switch (mainGridColumn.Field) {
                            case VR_Invoice_InvoiceFieldEnum.CustomField.value:
                                field = field = "Entity." + VR_Invoice_InvoiceFieldEnum.CustomField.fieldName + "." + mainGridColumn.CustomFieldName;
                                attribute = {
                                    type: mainGridColumn.Attribute.Type,
                                    numberprecision: mainGridColumn.Attribute.NumberPrecision
                                };
                                break;
                            case VR_Invoice_InvoiceFieldEnum.InvoiceId.value:
                                field = field = "Entity." + VR_Invoice_InvoiceFieldEnum.InvoiceId.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.InvoiceId.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.Partner.value:
                                field = VR_Invoice_InvoiceFieldEnum.Partner.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.Partner.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.SerialNumber.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.SerialNumber.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.SerialNumber.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.FromDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.FromDate.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.FromDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.ToDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.ToDate.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.ToDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.IssueDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.IssueDate.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.IssueDate.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.DueDate.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.DueDate.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.DueDate.type;
                                break;
                        }
                        $scope.gridFields.push({ HeaderText: mainGridColumn.Header, Field: field, Type: attribute.type, NumberPrecision: attribute.numberprecision });
                    }
                }
            }

            //function buildGridSubSections(subSections) {
            //    if (subSections != undefined) {
            //        for (var i = 0; i < subSections.length ; i++) {
            //            var subSection = subSections[i];
            //            var tab = buildInvoiceItemsTab(subSection);
            //            if (tab != undefined)
            //                drillDownTabs.push(tab);
            //        }
            //        function buildInvoiceItemsTab(subSection) {
            //            var invoiceItemsTab = {};
            //            var cofigItem = UtilsService.getItemByVal(subSectionConfigs, subSection.Settings.ConfigId, "ExtensionConfigurationId");
            //            if(cofigItem != undefined)
            //            {
            //                invoiceItemsTab.title = subSection.SectionTitle;
            //                invoiceItemsTab.directive = cofigItem.RuntimeEditor;
            //                invoiceItemsTab.loadDirective = function (invoiceItemGridAPI, invoice) {
            //                    invoice.invoiceItemGridAPI = invoiceItemGridAPI;
            //                    var invoiceItemGridPayload = {
            //                        query: {
            //                            InvoiceId: invoice.Entity.InvoiceId,
            //                        },
            //                        settings: subSection.Settings,
            //                        invoiceId: invoice.Entity.InvoiceId,
            //                    };
            //                    return invoice.invoiceItemGridAPI.load(invoiceItemGridPayload);
            //                };
            //                invoiceItemsTab.parentMenuActions = [];
            //                return invoiceItemsTab;
            //            }
            //        }
            //    }
            //}

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