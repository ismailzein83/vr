"use strict";

app.directive("vrInvoicesettingGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceSettingAPIService", "VRUIUtilsService", "VR_Invoice_InvoiceSettingService","VR_Invoice_PartnerInvoiceSettingService","VR_Invoice_PartnerInvoiceSettingAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceSettingAPIService, VRUIUtilsService, VR_Invoice_InvoiceSettingService, VR_Invoice_PartnerInvoiceSettingService, VR_Invoice_PartnerInvoiceSettingAPIService) {

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

            function initializeController() {

                $scope.datastore = [];
                $scope.menuActions = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;


                    var drillDownDefinitions = [];
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Partner Invoice Setting";
                    drillDownDefinition.directive = "vr-partnerinvoicesetting-grid";

                    drillDownDefinition.loadDirective = function (directiveAPI, invoiceSettingItem) {
                        invoiceSettingItem.partnerInvoiceSettingGridAPI = directiveAPI;
                        var query = {
                            InvoiceSettingId: invoiceSettingItem.Entity.InvoiceSettingId
                        };
                        return invoiceSettingItem.partnerInvoiceSettingGridAPI.loadGrid(query);
                    };
                    drillDownDefinition.parentMenuActions = [{
                        name: "Add Partner Invoice Setting",
                        haspermission: function(dataItem){
                            return VR_Invoice_PartnerInvoiceSettingAPIService.HasAssignPartnerAccess(dataItem.Entity.InvoiceSettingId);
                        },
                        clicked: function (invoiceSettingItem) {
                            if (drillDownDefinition.setTabSelected != undefined)
                                drillDownDefinition.setTabSelected(invoiceSettingItem);
                            var onPartnerInvoiceSettingAdded = function (partnerInvoiceSettingObj) {
                                if (invoiceSettingItem.partnerInvoiceSettingGridAPI != undefined) {
                                    invoiceSettingItem.partnerInvoiceSettingGridAPI.onPartnerInvoiceSettingAdded(partnerInvoiceSettingObj);
                                }
                            };
                            VR_Invoice_PartnerInvoiceSettingService.addPartnerInvoiceSetting(onPartnerInvoiceSettingAdded, invoiceSettingItem.Entity.InvoiceSettingId);
                        }
                    }];
                    drillDownDefinitions.push(drillDownDefinition);
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions);


                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            gridQuery = query;
                            return gridAPI.retrieveData(gridQuery);
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
        }

        return directiveDefinitionObject;

    }
]);