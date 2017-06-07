"use strict";

app.directive("vrInvoicetypeItemsetnamestoragerulesManagement", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ItemSetNameStorageRules($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/ItemSetNameStorageRule/Templates/ItemSetNameStorageRulesManagement.html"

        };

        function ItemSetNameStorageRules($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addItemSetNameStorageRule = function () {
                    var onItemSetNameStorageRuleAdded = function (itemSetNameStorageRule) {
                        ctrl.datasource.push({ Entity: itemSetNameStorageRule });
                    };

                    VR_Invoice_InvoiceTypeService.addItemSetNameStorageRule(onItemSetNameStorageRuleAdded, getContext());
                };

                ctrl.removeItemSetNameStorageRule = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var itemSetNameStorageRules;
                    if (ctrl.datasource != undefined) {
                        itemSetNameStorageRules = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            itemSetNameStorageRules.push(currentItem.Entity);
                        }
                    }
                    return itemSetNameStorageRules;
                };

                api.load = function (payload) {

                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.itemSetNameStorageRules != undefined) {
                            for (var i = 0; i < payload.itemSetNameStorageRules.length; i++) {
                                var itemSetNameStorageRule = payload.itemSetNameStorageRules[i];
                                ctrl.datasource.push({ Entity: itemSetNameStorageRule });
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
                    clicked: editItemSetNameStorageRule,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editItemSetNameStorageRule(itemSetNameStorageRuleObj) {
                var onItemSetNameStorageRuleUpdated = function (itemSetNameStorageRule) {
                    var index = ctrl.datasource.indexOf(itemSetNameStorageRuleObj);
                    ctrl.datasource[index] = { Entity: itemSetNameStorageRule };
                };
                VR_Invoice_InvoiceTypeService.editItemSetNameStorageRule(itemSetNameStorageRuleObj.Entity, onItemSetNameStorageRuleUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);