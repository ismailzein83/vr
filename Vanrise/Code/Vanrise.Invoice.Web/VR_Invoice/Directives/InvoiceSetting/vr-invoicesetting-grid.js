"use strict";

app.directive("vrInvoicesettingGrid", [ "VRCommon_ObjectTrackingService", "UtilsService", "VRNotificationService", "VR_Invoice_InvoiceSettingAPIService", "VRUIUtilsService", "VR_Invoice_InvoiceSettingService", "VR_Invoice_PartnerInvoiceSettingService",
    function (VRCommon_ObjectTrackingService, UtilsService, VRNotificationService, VR_Invoice_InvoiceSettingAPIService, VRUIUtilsService, VR_Invoice_InvoiceSettingService, VR_Invoice_PartnerInvoiceSettingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new InvoiceSettingGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceSetting/Templates/InvoiceSettingGridTemplate.html"

        };

        function InvoiceSettingGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridQuery;
            var gridDrillDownTabsObj;
            var showAccountSelector;
            var partnerIds;
            function initializeController() {

                $scope.datastore = [];
                $scope.menuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;


                    var drillDownDefinitions = [];
                    var drillDownDefinition = {};
                    var drillDownDefinitionHistory = {};

                    drillDownDefinition.title = "Linked Partners";
                    drillDownDefinition.directive = "vr-invoice-partnerinvoiecsetting-search";

                    drillDownDefinition.loadDirective = function (directiveAPI, invoiceSettingItem) {
                        invoiceSettingItem.partnerInvoiceSettingGridAPI = directiveAPI;
                        var query = {
                            invoiceSettingId: invoiceSettingItem.Entity.InvoiceSettingId,
                            invoiceTypeId: invoiceSettingItem.Entity.InvoiceTypeId,
                            showAccountSelector: showAccountSelector,
                        partnerIds: partnerIds
                        };
                        return invoiceSettingItem.partnerInvoiceSettingGridAPI.load(query);
                    };
                    drillDownDefinitions.push(drillDownDefinition);

                    drillDownDefinitionHistory.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                    drillDownDefinitionHistory.directive = "vr-common-objecttracking-grid";
                    drillDownDefinitionHistory.loadDirective = function (directiveAPI, invoiceSettingItem) {
                        invoiceSettingItem.objectTrackingGridAPI = directiveAPI; 
                        var query = {
                            ObjectId: invoiceSettingItem.Entity.InvoiceSettingId,
                            EntityUniqueName: VR_Invoice_InvoiceSettingService.getEntityUniqueName(invoiceSettingItem.Entity.InvoiceTypeId)
                        };
                        return invoiceSettingItem.objectTrackingGridAPI.load(query);
                    };

                    drillDownDefinitions.push(drillDownDefinitionHistory);
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions);


                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (payload) {
                            if (payload != undefined) {
                                showAccountSelector = payload.showAccountSelector;
                                partnerIds = payload.partnerIds;
                                gridQuery = payload.query;
                                return gridAPI.retrieveData(payload.query);
                            }
                         
                        };
                        directiveAPI.onInvoiceSettingAdded = function (invoiceSetting) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(invoiceSetting);
                            gridAPI.itemAdded(invoiceSetting);
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceSettingAPIService.GetFilteredInvoiceSettings(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {

                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
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

            function defineMenuActions() {
                var defaultMenuAction = [{
                    name: "Edit",
                    clicked: editInvoiceSetting,
                    haspermission: hasUpdateInvoiceSettingPermission
                }];
                var mainMenuAction = [{
                    name: "Edit",
                    clicked: editInvoiceSetting,
                    haspermission: hasUpdateInvoiceSettingPermission
                }, {
                    name: "Set Default",
                    clicked: setInvoiceSettingDefault,
                    haspermission: hasUpdateInvoiceSettingPermission
                }];
               
                $scope.gridMenuActions = function (dataItem) {
                    if (dataItem.menuActionObj.menuActions != undefined)
                        dataItem.menuActionObj.menuActions.length = 0;
                    var menuActions = [];
                    if (dataItem.Entity.IsDefault) {
                        menuActions = UtilsService.cloneObject(defaultMenuAction);
                    } else {
                        menuActions = UtilsService.cloneObject(mainMenuAction);
                    }
                    for (var i = 0; i < $scope.menuActions.length; i++)
                    {
                        var menuAction = $scope.menuActions[i];
                        menuActions.push(menuAction);
                    }
                    if (dataItem.CanDeleteInvoiceSetting)
                    {
                        menuActions.push({
                            name: "Delete",
                            clicked: deleteInvoiceSetting,
                            haspermission: hasUpdateInvoiceSettingPermission
                        });
                    }
                    return menuActions;
                };
            }
            function hasUpdateInvoiceSettingPermission(dataItem) {
                return VR_Invoice_InvoiceSettingAPIService.HasUpdateInvoiceSettingPermission(dataItem.Entity.InvoiceTypeId);
            }
            function editInvoiceSetting(dataItem) {
                var onInvoiceSettingUpdated = function (invoiceSetting) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(invoiceSetting);
                    gridAPI.itemUpdated(invoiceSetting);
                };
                VR_Invoice_InvoiceSettingService.editInvoiceSetting(onInvoiceSettingUpdated, dataItem.Entity.InvoiceSettingId, dataItem.Entity.InvoiceTypeId)
            }
            function setInvoiceSettingDefault(dataItem) {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        return VR_Invoice_InvoiceSettingAPIService.SetInvoiceSettingDefault(dataItem.Entity.InvoiceSettingId,dataItem.Entity.InvoiceTypeId).then(function (response) {
                            return gridAPI.retrieveData(gridQuery);
                        });
                    }
                });
            }
            function deleteInvoiceSetting(dataItem) {
                var promiseDeffered = UtilsService.createPromiseDeferred();
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        VR_Invoice_InvoiceSettingAPIService.DeleteInvoiceSetting(dataItem.Entity.InvoiceTypeId, dataItem.Entity.InvoiceSettingId).then(function (response) {
                            if (VRNotificationService.notifyOnItemDeleted("Invoice Setting", response)) {
                                gridAPI.itemDeleted(dataItem);
                            }
                            promiseDeffered.resolve();
                        }).catch(function(error){
                            promiseDeffered.reject(error);
                        });
                    } else {
                        promiseDeffered.resolve();
                    }
                });
                return promiseDeffered.promise;
            }
        }

        return directiveDefinitionObject;

    }
]);