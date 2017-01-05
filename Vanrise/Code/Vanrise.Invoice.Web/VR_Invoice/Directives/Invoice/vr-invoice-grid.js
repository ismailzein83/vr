"use strict";

app.directive("vrInvoiceGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService", "VR_Invoice_InvoiceFieldEnum", "VRUIUtilsService", "VR_Invoice_InvoiceService", "VR_Invoice_InvoiceTypeConfigsAPIService","VR_Invoice_InvoiceActionService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService, VR_Invoice_InvoiceFieldEnum, VRUIUtilsService, VR_Invoice_InvoiceService, VR_Invoice_InvoiceTypeConfigsAPIService, VR_Invoice_InvoiceActionService) {

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
            var invoiceTypeId;
            var invoiceActions=[];
            var invoiceGridActions = [];
            var mainGridColumns = [];
            var invoiceItemGroupings;
            function initializeController() {

                $scope.datastore = [];
                $scope.gridMenuActions = [];

                $scope.menuActions = function (dataItem) {
                    var menuActions = [];
                    if (dataItem.menuActions != null) {
                        for (var i = 0; i < dataItem.menuActions.length; i++)
                            menuActions.push(dataItem.menuActions[i]);
                    }
                    return menuActions;
                };


                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            var query;
                            var promiseDeferred = UtilsService.createPromiseDeferred();
                            VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceUISubSectionSettingsConfigs().then(function (response) {
                                if (response != undefined) {
                                    subSectionConfigs = response;
                                }
                                if (payload != undefined) {
                                    invoiceActions = payload.invoiceActions;
                                    mainGridColumns = payload.mainGridColumns;
                                    subSections = payload.subSections;
                                    query = payload.query;
                                    invoiceGridActions = payload.invoiceGridActions;
                                    invoiceTypeId = payload.InvoiceTypeId;
                                    invoiceItemGroupings = payload.invoiceItemGroupings;
                                  
                                }
                                gridAPI.retrieveData(query).then(function () {
                                    promiseDeferred.resolve();
                                }).catch(function (error) {
                                    promiseDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                promiseDeferred.reject(error);
                            });
                            return promiseDeferred.promise;
                        };

                        directiveAPI.onGenerateInvoice = function (invoice) {
                            buildGridFields(invoice);
                            VR_Invoice_InvoiceService.defineInvoiceTabsAndMenuActions(invoice, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions, invoiceItemGroupings);
                            gridAPI.itemAdded(invoice);
                        };

                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoices(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var dataItem = response.Data[i];
                                    if ($scope.gridFields == undefined || $scope.gridFields.length == 0)
                                        buildGridFields(dataItem);
                                    VR_Invoice_InvoiceService.defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions, invoiceItemGroupings);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }

            function buildGridFields(dataItem)
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
                        var invoiceField = UtilsService.getEnum(VR_Invoice_InvoiceFieldEnum, "value", mainGridColumn.Field);
                        if (invoiceField != undefined)
                        {
                            if (invoiceField.value == VR_Invoice_InvoiceFieldEnum.CustomField.value)
                            {
                                if (dataItem != undefined && dataItem.Items != undefined) {
                                    for (var j = 0; j < dataItem.Items.length; j++) {
                                        var item = dataItem.Items[j];
                                        if (item.FieldName == mainGridColumn.CustomFieldName) {
                                            field = "Items[" + j + "].Description";
                                        }
                                    }
                                }
                                attribute = {
                                    type: mainGridColumn.Attribute.Type,
                                    numberprecision: mainGridColumn.Attribute.NumberPrecision
                                };
                            } else
                            {
                                field = invoiceField.fieldName;
                                attribute.type = invoiceField.type;
                            }
                            $scope.gridFields.push({ HeaderText: mainGridColumn.Header, Field: field, Type: attribute.type, NumberPrecision: attribute.numberprecision });
                        }
                    }
                }
            }
      
        }

        return directiveDefinitionObject;

    }
]);