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
                                    //defineMenuActions(payload.invoiceGridActions);
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
                            VR_Invoice_InvoiceService.defineInvoiceTabsAndMenuActions(invoice, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions);
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
                                    buildGridFields(dataItem);
                                    VR_Invoice_InvoiceService.defineInvoiceTabsAndMenuActions(dataItem, gridAPI, subSections, subSectionConfigs, invoiceTypeId, invoiceActions);
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
                        switch (mainGridColumn.Field) {
                            case VR_Invoice_InvoiceFieldEnum.CustomField.value:
                                if (dataItem != undefined && dataItem.Items != undefined) {
                                    for (var j = 0; j < dataItem.Items.length; j++) {
                                        var item = dataItem.Items[j];
                                        if (item.FieldName == mainGridColumn.CustomFieldName)
                                        {
                                            field = "Items[" + j + "].Description";
                                        }
                                    }
                                }
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
                            case VR_Invoice_InvoiceFieldEnum.Paid.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.Paid.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.Paid.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.UserId.value:
                                field = VR_Invoice_InvoiceFieldEnum.UserId.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.UserId.type;
                                break;
                            case VR_Invoice_InvoiceFieldEnum.CreatedTime.value:
                                field = "Entity." + VR_Invoice_InvoiceFieldEnum.CreatedTime.fieldName;
                                attribute.type = VR_Invoice_InvoiceFieldEnum.CreatedTime.type;
                                break;
                        }
                        $scope.gridFields.push({ HeaderText: mainGridColumn.Header, Field: field, Type: attribute.type, NumberPrecision: attribute.numberprecision });
                    }
                   
                }
            }

            function defineMenuActions(invoiceGridActions) {
                console.log(invoiceGridActions);
                $scope.gridMenuActions.length = 0;
                if (invoiceGridActions != undefined)
                {
                    for(var i=0;i<invoiceGridActions.length;i++)
                    {
                        var invoiceGridAction = invoiceGridActions[i];
                        var actionType = VR_Invoice_InvoiceActionService.getActionTypeIfExist(invoiceGridAction.Settings.ActionTypeName);
                        if(actionType != undefined)
                        {
                            addgridMenuAction(invoiceGridAction,actionType);
                        }
                    }
                    function addgridMenuAction(invoiceGridAction, actionType)
                    {
                        $scope.gridMenuActions.push({
                            name: invoiceGridAction.Title,
                            clicked: function (dataItem) {
                                var payload = {
                                    invoice: dataItem,
                                    invoiceGridAction: invoiceGridAction
                                };
                                var promiseDeffered = UtilsService.createPromiseDeferred();

                                var promise = actionType.actionMethod(payload);
                                if (promise != undefined && promise.then != undefined)
                                {
                                    ctrl.isLodingGrid = true;

                                    promise.then(function (response) {
                                        if (invoiceGridAction.ReloadGridItem && response)
                                        {
                                            var invoiceId = dataItem.Entity.InvoiceId;
                                            return VR_Invoice_InvoiceAPIService.GetInvoiceDetail(invoiceId).then(function (response) {
                                                promiseDeffered.resolve();
                                                gridAPI.itemUpdated(response);
                                            }).catch(function (error) {
                                                promiseDeffered.reject(error);
                                            });
                                        }else
                                        {
                                          promiseDeffered.resolve();
                                        }
                                    }).catch(function (error) {
                                        promiseDeffered.reject(error);
                                    }).finally(function () {
                                        ctrl.isLodingGrid = false;
                                    });
                                } else
                                {
                                    promiseDeffered.resolve();
                                }
                                return promiseDeffered.promise;
                            }
                        });
                    }
                }
            }
        }

        return directiveDefinitionObject;

    }
]);