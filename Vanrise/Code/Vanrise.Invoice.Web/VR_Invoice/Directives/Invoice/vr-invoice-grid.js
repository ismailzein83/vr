"use strict";

app.directive("vrInvoiceGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceAPIService) {

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
            function initializeController() {

                $scope.datastore = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            if (query.mainGridColumns != undefined) {
                                $scope.gridFields = [];
                                for(var i=0;i<query.mainGridColumns.length ; i++)
                                {
                                    var mainGridColumn = query.mainGridColumns[i];
                                    var field;
                                    if(mainGridColumn.CustomFieldName != null)
                                    {
                                        field = mainGridColumn.CustomFieldName;
                                    }
                                    $scope.gridFields.push({ header: mainGridColumn.Header, field: field });
                                }
                            }
                            return gridAPI.retrieveData(query);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Invoice_InvoiceAPIService.GetFilteredInvoices(dataRetrievalInput)
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