﻿'use strict';

app.directive('vrWhsBeInvoicesettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_InvoiceSettingService',
    function (UtilsService, VRUIUtilsService, WhS_BE_InvoiceSettingService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/InvoiceSettings/Templates/InvoiceSettingsTemplate.html"
        };

        function InvoiceSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        var count = 0;
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i];
                            if (item.Entity.IsDefault) {
                                count = count + 1;
                            }
                        }
                        if (count == 1)
                            return null;
                        if (count > 1)
                            return "You Should add at max one default setting.";
                        return "You Should add at least one default setting.";

                    }
                    return "You Should add at least one setting.";
                };
                ctrl.addInvoiceSetting = function () {
                    var onInvoiceSettingAdded = function (invoiceSetting) {
                        ctrl.datasource.push({ Entity: invoiceSetting });
                    };
                    WhS_BE_InvoiceSettingService.addInvoiceSetting(onInvoiceSettingAdded, ctrl.datasource);
                };
                ctrl.removeInvoiceSetting = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var invoiceSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        invoiceSettingsPayload = payload.data;
                    }
                    if (invoiceSettingsPayload != undefined && invoiceSettingsPayload.CustomerInvoiceSettings != undefined) {
                        for (var i = 0; i < invoiceSettingsPayload.CustomerInvoiceSettings.length; i++) {
                            var invoiceSetting = invoiceSettingsPayload.CustomerInvoiceSettings[i];
                            ctrl.datasource.push({ Entity: invoiceSetting });
                        }
                    }
                };

                api.getData = function () {
                    var invoiceSettings;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        invoiceSettings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            invoiceSettings.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.InvoiceSettings, TOne.WhS.BusinessEntity.Entities",
                        CustomerInvoiceSettings: invoiceSettings
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editInvoiceSetting
                }];

                $scope.gridMenuActions = function () {
                    return defaultMenuActions;
                };
            }

            function editInvoiceSetting(invoiceSettingObj) {
                var onInvoiceSettingUpdated = function (invoiceSetting) {
                    var index = ctrl.datasource.indexOf(invoiceSettingObj);
                    ctrl.datasource[index] = { Entity: invoiceSetting };
                };
                WhS_BE_InvoiceSettingService.editInvoiceSetting(invoiceSettingObj.Entity, onInvoiceSettingUpdated, ctrl.datasource);
            }
        }
    }]);