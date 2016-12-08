"use strict";

app.directive("vrInvoiceItemgroupingAggregates", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemGroupingService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemGroupingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AggregateItemGrouping($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/AggregateItemGroupingTemplate.html"

        };

        function AggregateItemGrouping($scope, ctrl, $attrs) {

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

                ctrl.addAggregateItemGrouping = function () {
                    var onAggregateItemGroupingAdded = function (aggregateItemGrouping) {
                        ctrl.datasource.push({ Entity: aggregateItemGrouping });
                    };

                    VR_Invoice_InvoiceItemGroupingService.addItemGroupingAggregate(onAggregateItemGroupingAdded, getContext());
                };

                ctrl.removeAggregateItemGrouping = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var aggregateItemGroupings;
                    if (ctrl.datasource != undefined) {
                        aggregateItemGroupings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            aggregateItemGroupings.push(currentItem.Entity);
                        }
                    }
                    return aggregateItemGroupings;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.aggregateItemGroupings != undefined) {
                            for (var i = 0; i < payload.aggregateItemGroupings.length; i++) {
                                var aggregateItemGrouping = payload.aggregateItemGroupings[i];
                                ctrl.datasource.push({ Entity: aggregateItemGrouping });
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
                    clicked: editAggregateItemGrouping,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAggregateItemGrouping(aggregateItemGroupingObj) {
                var onAggregateItemGroupingUpdated = function (aggregateItemGrouping) {
                    var index = ctrl.datasource.indexOf(aggregateItemGroupingObj);
                    ctrl.datasource[index] = { Entity: aggregateItemGrouping };
                };

                VR_Invoice_InvoiceItemGroupingService.editItemGroupingAggregate(onAggregateItemGroupingUpdated, aggregateItemGroupingObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);