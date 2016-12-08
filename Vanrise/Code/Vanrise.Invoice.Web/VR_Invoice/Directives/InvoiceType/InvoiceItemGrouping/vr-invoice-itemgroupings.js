"use strict";

app.directive("vrInvoiceItemgroupings", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceItemGroupingService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceItemGroupingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ItemGrouping($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceItemGrouping/Templates/InvoiceItemGroupingsTemplate.html"

        };

        function ItemGrouping($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                //ctrl.isValid = function () {
                //    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                //        return null;
                //    return "You Should add at least one part.";
                //};

                ctrl.addItemGrouping = function () {
                    var onItemGroupingAdded = function (itemGrouping) {
                        ctrl.datasource.push({ Entity: itemGrouping });
                    };

                    VR_Invoice_InvoiceItemGroupingService.addItemGrouping(onItemGroupingAdded, getContext());
                };

                ctrl.removeItemGrouping = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var itemGroupings;
                    if (ctrl.datasource != undefined) {
                        itemGroupings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            itemGroupings.push(currentItem.Entity);
                        }
                    }
                    return itemGroupings;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.itemGroupings != undefined) {
                            for (var i = 0; i < payload.itemGroupings.length; i++) {
                                var itemGrouping = payload.itemGroupings[i];
                                ctrl.datasource.push({ Entity: itemGrouping });
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
                    clicked: editItemGrouping,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editItemGrouping(itemGroupingObj) {
                var onItemGroupingUpdated = function (itemGrouping) {
                    var index = ctrl.datasource.indexOf(itemGroupingObj);
                    ctrl.datasource[index] = { Entity: itemGrouping };
                };

                VR_Invoice_InvoiceItemGroupingService.editItemGrouping(onItemGroupingUpdated, itemGroupingObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);