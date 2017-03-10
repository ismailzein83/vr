﻿'use strict';
app.directive('vrInvoicetypeInvoicesettingDefinitionRow', ['UtilsService', 'VRUIUtilsService','VR_Invoice_InvoiceTypeService','VR_Invoice_InvoiceTypeConfigsAPIService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeService, VR_Invoice_InvoiceTypeConfigsAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSettingDefinition/Templates/DefinitionRowTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var gridAPI;
            var context;
            var invoiceSettingPartsConfigs;
            function initializeController() {
                ctrl.parts = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };
                ctrl.removePart = function (dataItem) {
                    var index = ctrl.parts.indexOf(dataItem);
                    ctrl.parts.splice(index, 1);
                    if (context != undefined) {
                        context.removePart(dataItem.Entity);
                    }
                };
                defineAPI();
                defineGridRowsMenuAction();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   return getInvoiceSettingPartsConfigs().then(function () {
                        if (payload != undefined) {
                            context = payload.context;
                            if (payload.parts != undefined) {
                                ctrl.parts.length = 0;
                                for (var i = 0; i < payload.parts.length; i++) {
                                    var part = payload.parts[i];
                                    var partConfig = UtilsService.getItemByVal(invoiceSettingPartsConfigs,part.PartConfigId,"ExtensionConfigurationId");
                                    ctrl.parts.push({ Entity: part, partName: partConfig!=undefined?partConfig.Title:undefined });
                                }
                            }

                        }
                    })
                    
                };

                api.applyChanges = function (changes) {
                    if (changes != undefined) {
                        if (ctrl.parts == undefined)
                            ctrl.parts = [];
                        var partConfig = UtilsService.getItemByVal(invoiceSettingPartsConfigs, changes.PartConfigId, "ExtensionConfigurationId");
                        ctrl.parts.push({ Entity: changes, partName: partConfig != undefined ? partConfig.Title : undefined });
                    }
                };

                api.getData = function () {
                    var parts = [];
                    for (var i = 0; i < ctrl.parts.length; i++) {
                        var part = ctrl.parts[i];
                        parts.push(part.Entity);
                    }
                    return { Parts: parts };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineGridRowsMenuAction() {
                $scope.gridPartsMenuActions = [{
                    name: "Edit Part",
                    clicked: editPart,
                }];
            }

            function editPart(dataItem) {
                var onPartUpdated = function (part) {
                    var partIndex = ctrl.parts.indexOf(dataItem);
                    var partConfig = UtilsService.getItemByVal(invoiceSettingPartsConfigs, part.PartConfigId, "ExtensionConfigurationId");
                    ctrl.parts[partIndex] = { Entity: part, partName: partConfig != undefined ? partConfig.Title : undefined };

                };
                VR_Invoice_InvoiceTypeService.editInvoiceSettingPart(onPartUpdated, dataItem.Entity);
            }
            function getInvoiceSettingPartsConfigs()
            {
               return VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceSettingPartsConfigs().then(function (response) {
                   invoiceSettingPartsConfigs = response;
                });
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);