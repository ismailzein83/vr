"use strict";

app.directive("vrInvoicetypeInvoicefilterconditionConditiongroup", ["UtilsService", "VRNotificationService", "VRUIUtilsService",'VR_Invoice_FilterConditionService','VR_Invoice_LogicalOperatorEnum',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_FilterConditionService, VR_Invoice_LogicalOperatorEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ConditionGroupFilterCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/MainExtensions/InvoiceFilterCondition/Templates/ConditionGroupTemplate.html"

        };

        function ConditionGroupFilterCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.logicalOperators = UtilsService.getArrayEnum(VR_Invoice_LogicalOperatorEnum);;

                ctrl.datasource = [];

                ctrl.addFilterCondition = function () {
                    var onFilterConditionItemAdded = function (filterCondition) {
                        ctrl.datasource.push({ Entity: filterCondition });
                    };
                    VR_Invoice_FilterConditionService.addFilterCondition(onFilterConditionItemAdded, getContext());
                };

                ctrl.removeFilterCondition = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        if (invoiceFilterConditionEntity != undefined) {

                            $scope.scopeModel.selectedLogicalOperator = UtilsService.getItemByVal($scope.scopeModel.logicalOperators, invoiceFilterConditionEntity.LogicalOperator, 'value');
                            if (invoiceFilterConditionEntity.FilterConditionItems != undefined) {
                                for (var i = 0; i < invoiceFilterConditionEntity.FilterConditionItems.length; i++) {
                                    var filterCondition = invoiceFilterConditionEntity.FilterConditionItems[i];
                                    ctrl.datasource.push({ Entity: filterCondition });
                                }
                            }
                        }

                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var filterConditions;
                    if (ctrl.datasource != undefined) {
                        filterConditions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            filterConditions.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ConditionGroupFilterCondition, Vanrise.Invoice.MainExtensions",
                        FilterConditionItems: filterConditions,
                        LogicalOperator: $scope.scopeModel.selectedLogicalOperator.value

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
            
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editFilterCondition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editFilterCondition(filterConditionObj) {
                var onFilterConditionItemUpdated = function (filterCondition) {
                    var index = ctrl.datasource.indexOf(filterConditionObj);
                    ctrl.datasource[index] = { Entity: filterCondition };
                };

                VR_Invoice_FilterConditionService.editFilterCondition(filterConditionObj.Entity, onFilterConditionItemUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);
