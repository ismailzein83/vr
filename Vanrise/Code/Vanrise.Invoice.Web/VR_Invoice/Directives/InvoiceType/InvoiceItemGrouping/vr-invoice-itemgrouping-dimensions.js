"use strict";

app.directive("vrInvoiceItemgroupingDimensions", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemGroupingService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemGroupingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DimensionItemGrouping($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/DimensionItemGroupingTemplate.html"

        };

        function DimensionItemGrouping($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one part.";
                };

                ctrl.addDimensionItemGrouping = function () {
                    var onDimensionItemGroupingAdded = function (dimensionItemGrouping) {
                        ctrl.datasource.push({ Entity: dimensionItemGrouping });
                    };

                    VR_Invoice_InvoiceItemGroupingService.addItemGroupingDimension(onDimensionItemGroupingAdded, getContext());
                };

                ctrl.removeDimensionItemGrouping = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var dimensionItemGroupings;
                    if (ctrl.datasource != undefined) {
                        dimensionItemGroupings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            dimensionItemGroupings.push(currentItem.Entity);
                        }
                    }
                    return dimensionItemGroupings;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.dimensionItemGroupings != undefined) {
                            for (var i = 0; i < payload.dimensionItemGroupings.length; i++) {
                                var dimensionItemGrouping = payload.dimensionItemGroupings[i];
                                ctrl.datasource.push({ Entity: dimensionItemGrouping });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editDimensionItemGrouping,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDimensionItemGrouping(dimensionItemGroupingObj) {
                var onDimensionItemGroupingUpdated = function (dimensionItemGrouping) {
                    var index = ctrl.datasource.indexOf(dimensionItemGroupingObj);
                    ctrl.datasource[index] = { Entity: dimensionItemGrouping };
                };

                VR_Invoice_InvoiceItemGroupingService.editItemGroupingDimension(onDimensionItemGroupingUpdated, dimensionItemGroupingObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);