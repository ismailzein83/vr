"use strict";

app.directive("whsInvoiceInvoiceitemCustomer", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemAPIService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemAPIService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/CutomerInvoiceItem/Templates/CustomerInvoiceItemTemplates.html"

        };

        function InvoiceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            function initializeController() {

                $scope.datastore = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.load = function (query) {
                            if (query == undefined)
                                query = {};
                            query.ItemSetName = "Customer";
                            return gridAPI.retrieveData(query);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceItemAPIService.GetFilteredInvoiceItems(dataRetrievalInput)
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
                var defaultMenuActions = [];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }
        }

        return directiveDefinitionObject;

    }
]);