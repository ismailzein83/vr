"use strict";

app.directive("vrInvoicetypeGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeAPIService", "VRUIUtilsService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeAPIService, VRUIUtilsService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var invoiceGrid = new InvoiceTypeGrid($scope, ctrl, $attrs);
                invoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/Templates/InvoiceTypeGridTemplate.html"

        };

        function InvoiceTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
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
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceTypeAPIService.GetFilteredInvoiceTypes(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
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
                $scope.gridMenuActions.push({
                    name: "Edit",
                    clicked: editInvoiceType
                });
            }
            function editInvoiceType(dataItem)
            {
                var onInvoiceTypeUpdated = function (invoiceType)
                {
                    gridAPI.itemUpdated(invoiceType);
                }
                VR_Invoice_InvoiceTypeService.editInvoiceType(onInvoiceTypeUpdated,dataItem.Entity.InvoiceTypeId)
            }
        }

        return directiveDefinitionObject;

    }
]);