"use strict";

app.directive("vrInvoiceGroupingitemAggregates", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceGroupingItemService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceGroupingItemService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AggregateGroupingItem($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGroupingItem/Templates/AggregateGroupingItemTemplate.html"

        };

        function AggregateGroupingItem($scope, ctrl, $attrs) {

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

                ctrl.addAggregateGroupingItem = function () {
                    var onAggregateGroupingItemAdded = function (aggregateGroupingItem) {
                        ctrl.datasource.push({ Entity: aggregateGroupingItem });
                    };

                    VR_Invoice_InvoiceGroupingItemService.addGroupingItemAggregate(onAggregateGroupingItemAdded, getContext());
                };

                ctrl.removeAggregateGroupingItem = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var aggregateGroupingItems;
                    if (ctrl.datasource != undefined) {
                        aggregateGroupingItems = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            aggregateGroupingItems.push(currentItem.Entity);
                        }
                    }
                    return aggregateGroupingItems;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.aggregateGroupingItems != undefined) {
                            for (var i = 0; i < payload.aggregateGroupingItems.length; i++) {
                                var aggregateGroupingItem = payload.aggregateGroupingItems[i];
                                ctrl.datasource.push({ Entity: aggregateGroupingItem });
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
                    clicked: editAggregateGroupingItem,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAggregateGroupingItem(aggregateGroupingItemObj) {
                var onAggregateGroupingItemUpdated = function (aggregateGroupingItem) {
                    var index = ctrl.datasource.indexOf(aggregateGroupingItemObj);
                    ctrl.datasource[index] = { Entity: aggregateGroupingItem };
                };

                VR_Invoice_InvoiceGroupingItemService.editGroupingItemAggregate(onAggregateGroupingItemUpdated, aggregateGroupingItemObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);