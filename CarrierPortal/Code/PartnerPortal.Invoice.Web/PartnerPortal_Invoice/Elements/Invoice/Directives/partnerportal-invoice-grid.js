"use strict";

app.directive("partnerportalInvoiceGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "PartnerPortal_Invoice_InvoiceAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, PartnerPortal_Invoice_InvoiceAPIService) {

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
            templateUrl: "/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/Templates/InvoiceGridTemplate.html"

        };

        function InvoiceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridColumns = [];
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
                            if (payload != undefined) {
                                gridColumns = payload.gridColumns;
                                query = payload.query;
                            }
                            gridAPI.retrieveData(query);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return PartnerPortal_Invoice_InvoiceAPIService.GetFilteredInvoices(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    var dataItem = response.Data[i];
                                    if ($scope.gridFields == undefined || $scope.gridFields.length == 0)
                                        buildGridFields(dataItem);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
            }

            function buildGridFields(dataItem) {
                $scope.gridFields = [];
                if (gridColumns != undefined) {
                    for (var i = 0; i < gridColumns.length ; i++) {
                        var gridColumn = gridColumns[i];
                        var field;
                        var type;
                        var numberprecision;
                        if (gridColumn.Field == 0) {
                            if (dataItem != undefined && dataItem.Items != undefined) {
                                for (var j = 0; j < dataItem.Items.length; j++) {
                                    var item = dataItem.Items[j];
                                    if (item.FieldName == gridColumn.CustomFieldName) {
                                        field = "Items[" + j + "].Description";
                                    }
                                }
                            }
                            type = gridColumn.Attribute.Type;
                            numberprecision = gridColumn.Attribute.NumberPrecision;
                        } else {
                            field = "Entity." + gridColumn.FieldName;
                            type = gridColumn.Attribute.Type;
                        }
                        $scope.gridFields.push({ HeaderText: gridColumn.Header, Field: field, Type: type, NumberPrecision: numberprecision });

                    }
                }
            }

        }

        return directiveDefinitionObject;

    }
]);