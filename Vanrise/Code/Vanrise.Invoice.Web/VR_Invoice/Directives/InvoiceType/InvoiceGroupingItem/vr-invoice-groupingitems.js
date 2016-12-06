"use strict";

app.directive("vrInvoiceGroupingitems", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceGroupingItemService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceGroupingItemService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GroupingItem($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGroupingItem/Templates/InvoiceGroupingItemsTemplate.html"

        };

        function GroupingItem($scope, ctrl, $attrs) {

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

                ctrl.addGroupingItem = function () {
                    var onGroupingItemAdded = function (groupingItem) {
                        ctrl.datasource.push({ Entity: groupingItem });
                    };

                    VR_Invoice_InvoiceGroupingItemService.addGroupingItem(onGroupingItemAdded, getContext());
                };

                ctrl.removeGroupingItem = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var groupingItems;
                    if (ctrl.datasource != undefined) {
                        groupingItems = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            groupingItems.push(currentItem.Entity);
                        }
                    }
                    return groupingItems;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.groupingItems != undefined) {
                            for (var i = 0; i < payload.groupingItems.length; i++) {
                                var groupingItem = payload.groupingItems[i];
                                ctrl.datasource.push({ Entity: groupingItem });
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
                    clicked: editGroupingItem,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editGroupingItem(groupingItemObj) {
                var onGroupingItemUpdated = function (groupingItem) {
                    var index = ctrl.datasource.indexOf(groupingItemObj);
                    ctrl.datasource[index] = { Entity: groupingItem };
                };

                VR_Invoice_InvoiceGroupingItemService.editGroupingItem(onGroupingItemUpdated, groupingItemObj.Entity, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);