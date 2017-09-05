﻿"use strict";

app.directive("vrInvoicetypeFilenameInvoicefield", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceFieldEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceFieldEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceFieldFileNamePart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNamePart/MainExtensions/Templates/InvoiceFieldFileNamePartTemplate.html"

        };

        function InvoiceFieldFileNamePart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.invoiceFields = UtilsService.getArrayEnum(VR_Invoice_InvoiceFieldEnum);
                $scope.scopeModel.recordFields = [];

                $scope.scopeModel.isCustomFieldRequired = function () {
                    if ($scope.scopeModel.selectedInvoiceField != undefined) {
                        if ($scope.scopeModel.selectedInvoiceField.value == VR_Invoice_InvoiceFieldEnum.CustomField.value)
                            return true;
                    }
                    return false;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (context != undefined) {
                            var fields = context.getFields();
                            if (fields != undefined)
                                $scope.scopeModel.recordFields = fields;
                        }
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.selectedInvoiceField = UtilsService.getItemByVal($scope.scopeModel.invoiceFields, payload.concatenatedPartSettings.Field, "value");
                            $scope.scopeModel.selectedRecordField = UtilsService.getItemByVal($scope.scopeModel.recordFields, payload.concatenatedPartSettings.FieldName, "FieldName");
                        }

                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceFileNamePart.InvoiceFieldFileNamePart ,Vanrise.Invoice.MainExtensions",
                        Field: $scope.scopeModel.selectedInvoiceField.value,
                        FieldName: $scope.scopeModel.isCustomFieldRequired() ? $scope.scopeModel.selectedRecordField.FieldName : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);