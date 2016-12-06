"use strict";

app.directive("vrInvoiceGroupingitemDimensions", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceGroupingItemService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceGroupingItemService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DimensionGroupingItem($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGroupingItem/Templates/DimensionGroupingItemTemplate.html"

        };

        function DimensionGroupingItem($scope, ctrl, $attrs) {

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

                ctrl.addDimensionGroupingItem = function () {
                    var onDimensionGroupingItemAdded = function (dimensionGroupingItem) {
                        ctrl.datasource.push({ Entity: dimensionGroupingItem });
                    };

                    VR_Invoice_InvoiceGroupingItemService.addGroupingItemDimension(onDimensionGroupingItemAdded, getContext());
                };

                ctrl.removeDimensionGroupingItem = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var dimensionGroupingItems;
                    if (ctrl.datasource != undefined) {
                        dimensionGroupingItems = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            dimensionGroupingItems.push(currentItem.Entity);
                        }
                    }
                    return dimensionGroupingItems;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.dimensionGroupingItems != undefined) {
                            for (var i = 0; i < payload.dimensionGroupingItems.length; i++) {
                                var dimensionGroupingItem = payload.dimensionGroupingItems[i];
                                ctrl.datasource.push({ Entity: dimensionGroupingItem });
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
                    clicked: editDimensionGroupingItem,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDimensionGroupingItem(dimensionGroupingItemObj) {
                var onDimensionGroupingItemUpdated = function (dimensionGroupingItem) {
                    var index = ctrl.datasource.indexOf(dimensionGroupingItemObj);
                    ctrl.datasource[index] = { Entity: dimensionGroupingItem };
                };

                VR_Invoice_InvoiceGroupingItemService.editGroupingItemDimension(onDimensionGroupingItemUpdated, dimensionGroupingItemObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);