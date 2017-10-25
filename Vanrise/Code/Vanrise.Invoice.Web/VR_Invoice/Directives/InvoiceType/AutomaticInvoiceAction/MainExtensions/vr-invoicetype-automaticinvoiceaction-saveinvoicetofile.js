"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactionSaveinvoicetofile", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceGenerationActionService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceGenerationActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SaveInvoiceToFileAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/AutomaticSaveInvoiceToFileActionTemplate.html"

        };

        function SaveInvoiceToFileAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.addInvoiceToFileActionSet = function () {
                    var onInvoiceToFileActionSetAdded = function (invoiceToFileActionSet) {
                        ctrl.datasource.push({ Entity: invoiceToFileActionSet });
                    };

                    VR_Invoice_InvoiceGenerationActionService.addInvoiceToFileActionSet(onInvoiceToFileActionSetAdded, getContext());
                };
              
                ctrl.removeInvoiceToFileActionSet = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var automaticInvoiceActionEntity;
                    if (payload != undefined) {
                        automaticInvoiceActionEntity = payload.automaticInvoiceActionEntity;
                        context = payload.context;
                        if (automaticInvoiceActionEntity != undefined && automaticInvoiceActionEntity.InvoiceToFileActionSets != undefined) {
                            for (var i = 0; i < automaticInvoiceActionEntity.InvoiceToFileActionSets.length; i++) {
                                var anvoiceToFileActionSet = automaticInvoiceActionEntity.InvoiceToFileActionSets[i];
                                ctrl.datasource.push({ Entity: anvoiceToFileActionSet });
                            }
                        }
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var anvoiceToFileActionSets;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        anvoiceToFileActionSets = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            anvoiceToFileActionSets.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions.AutomaticSaveInvoiceToFileAction ,Vanrise.Invoice.MainExtensions",
                        InvoiceToFileActionSets: anvoiceToFileActionSets,
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
                    clicked: editInvoiceToFileActionSet,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editInvoiceToFileActionSet(invoiceToFileActionSetObj) {
                var onInvoiceToFileActionSetUpdated = function (invoiceToFileActionSet) {
                    var index = ctrl.datasource.indexOf(invoiceToFileActionSetObj);
                    ctrl.datasource[index] = { Entity: invoiceToFileActionSet };
                };
                VR_Invoice_InvoiceGenerationActionService.editInvoiceToFileActionSet(invoiceToFileActionSetObj.Entity, onInvoiceToFileActionSetUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);