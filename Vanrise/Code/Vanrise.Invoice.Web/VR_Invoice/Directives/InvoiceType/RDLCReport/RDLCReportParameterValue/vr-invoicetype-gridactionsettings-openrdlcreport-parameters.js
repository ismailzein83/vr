"use strict";

app.directive("vrInvoicetypeGridactionsettingsOpenrdlcreportParameters", ["UtilsService", "VRNotificationService", "VR_Invoice_InvoiceTypeService",
    function (UtilsService, VRNotificationService, VR_Invoice_InvoiceTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Parameters($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/RDLCReportParameterValue/Templates/RDLCReportParameterManagement.html"

        };

        function Parameters($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one parameter.";
                };

                ctrl.addParameter = function () {
                    var onParameterAdded = function (parameter) {
                        ctrl.datasource.push({ Entity: parameter });
                    };
                    VR_Invoice_InvoiceTypeService.addParameter(onParameterAdded, getContext());
                };

                ctrl.removeParameter = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var parameters;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        parameters = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            parameters.push({
                                ParameterName: currentItem.Entity.ParameterName,
                                Value: currentItem.Entity.Value,
                                IsVisible: currentItem.Entity.IsVisible,
                            });
                        }
                    }
                    return parameters;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.parameters != undefined) {
                            for (var i = 0; i < payload.parameters.length; i++) {
                                var parameter = payload.parameters[i];
                                ctrl.datasource.push({ Entity: parameter });
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
                    clicked: editParameter,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editParameter(parameterObj) {
                var onParameterUpdated = function (parameter) {
                    var index = ctrl.datasource.indexOf(parameterObj);
                    ctrl.datasource[index] = { Entity: parameter };
                };
                VR_Invoice_InvoiceTypeService.editParameter(parameterObj.Entity, onParameterUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);