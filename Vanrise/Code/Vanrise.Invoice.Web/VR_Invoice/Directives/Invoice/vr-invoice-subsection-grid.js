"use strict";

app.directive("vrInvoiceSubsectionGrid", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemAPIService", "VRUIUtilsService","VRCommon_GridWidthFactorEnum","VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemAPIService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VR_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var subSectionGrid = new SubSectionGrid($scope, ctrl, $attrs);
                subSectionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/SubSectionGridTemplate.html"

        };

        function SubSectionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridWidthFactors = [];
            function initializeController() {
                gridWidthFactors = UtilsService.getArrayEnum(VRCommon_GridWidthFactorEnum);
                $scope.datastore = [];
                $scope.gridFields = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.load = function (payload) {
                            var query = {};
                            if (payload != undefined) {
                                var promiseDeferred = UtilsService.createPromiseDeferred();
                                if (payload.query != undefined)
                                    query = payload.query;
                                if (payload.settings != undefined) {
                                    query.ItemSetName = payload.settings.ItemSetName;
                                    var input = {
                                        GridColumns: payload.settings.GridColumns
                                    };
                                    VR_Invoice_InvoiceTypeAPIService.CovertToGridColumnAttribute(input).then(function (response) {
                                        buildGridFields(response, payload.settings.GridColumns);
                                        gridAPI.retrieveData(query).then(function () {
                                            promiseDeferred.resolve();
                                        }).catch(function (error) {
                                            promiseDeferred.reject(error);
                                        });;
                                    }).catch(function (error) {
                                        promiseDeferred.reject(error);
                                    });;

                                }
                                return promiseDeferred.promise;
                            }
                            
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
            function buildGridFields(gridAttributes, gridColumns) {

                $scope.gridFields.length = 0;
                if (gridAttributes != undefined && gridColumns != undefined) {

                    for (var i = 0; i < gridColumns.length ; i++) {
                        var gridColumn = gridColumns[i];
                        var gridAttribute = UtilsService.getItemByVal(gridAttributes, gridColumn.FieldName, "Field");
                        if (gridAttribute != undefined)
                        {
                            gridAttribute.Field = "Entity." + gridColumn.FieldName;
                            var gridWidthFactor = UtilsService.getItemByVal(gridWidthFactors, gridColumn.WidthFactor, "value");
                            if (gridWidthFactor != undefined)
                                gridAttribute.WidthFactor = gridWidthFactor.widthFactor;
                            $scope.gridFields.push(gridAttribute);
                        }
                    }
                }
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