﻿'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeAutomaticinvoicesettingpart', ['UtilsService', 'VRUIUtilsService','VR_Invoice_InvoiceSettingAPIService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceSettingAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@',
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/AutomaticInvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onEnableAutomaticInvoiceChanged = function (value) {
                    if (currentContext != undefined)
                    {
                        if (currentContext.setRequiredBillingPeriod != undefined) {
                            currentContext.setRequiredBillingPeriod($scope.scopeModel.enableAutomaticInvoice);
                        }
                        if (currentContext.setAutomaticInvoiceActionsVisibility != undefined) {
                            currentContext.setAutomaticInvoiceActionsVisibility($scope.scopeModel.enableAutomaticInvoice);
                        }
                    }
                      
                };
                $scope.scopeModel.actions = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        currentContext = payload.context;
                        var invoiceTypeId = payload.invoiceTypeId;
                        var promises = [];
                        if (payload.fieldValue != undefined) {
                            $scope.scopeModel.enableAutomaticInvoice = payload.fieldValue.IsEnabled;
                            if (currentContext != undefined)
                            {
                                if (currentContext.setRequiredBillingPeriod != undefined)
                                    currentContext.setRequiredBillingPeriod($scope.scopeModel.enableAutomaticInvoice);
                                if (currentContext.setAutomaticInvoiceActionsVisibility != undefined)
                                    currentContext.setAutomaticInvoiceActionsVisibility($scope.scopeModel.enableAutomaticInvoice);
                            }
                               
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.AutomaticInvoiceSettingPart,Vanrise.Invoice.Entities",
                        IsEnabled: $scope.scopeModel.enableAutomaticInvoice,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);