﻿(function (app) {

    'use strict';

    InvoicePropertyEvaluator.$inject = ['VR_Invoice_InvoiceFieldEnum', 'UtilsService', 'VRUIUtilsService'];

    function InvoicePropertyEvaluator(VR_Invoice_InvoiceFieldEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new InvoiceSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/VRObjectTypes/Templates/InvoicePropertyEvaluatorTemplate.html'
        };

        function InvoiceSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var dataRecordFieldSelectorAPI;
            var dataRecordFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordFieldSelectorAPI = api;
                    dataRecordFieldSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.invoiceFields = UtilsService.getArrayEnum(VR_Invoice_InvoiceFieldEnum);
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
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.objectPropertyEvaluator != undefined) {
                            $scope.scopeModel.selectedInvoiceField = UtilsService.getItemByVal($scope.scopeModel.invoiceFields, payload.objectPropertyEvaluator.InvoiceField, "value");
                        }
                        var objectType = payload.objectType;
                        if(objectType != undefined)
                        {
                            VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(objectType.InvoiceTypeId).then(function (response) {

                                var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                                dataRecordFieldSelectorReadyDeferred.promise.then(function () {
                                    var dataRecordFieldSelectorPayload = { dataRecordTypeId: response.Settings.InvoiceDetailsRecordTypeId };
                                    if (invoiceEntity != undefined) {
                                        dataRecordFieldSelectorPayload.selectedIds = invoiceEntity.FieldName;
                                    }
                                    VRUIUtilsService.callDirectiveLoad(dataRecordFieldSelectorAPI, dataRecordFieldSelectorPayload, partnerSelectorPayloadLoadDeferred);
                                });
                                promises.push(partnerSelectorPayloadLoadDeferred.promise);

                            });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceFieldPropertyEvaluator, Vanrise.Invoice.MainExtensions",
                        InvoiceField: $scope.scopeModel.selectedInvoiceField.value,
                        FieldName: dataRecordFieldSelectorAPI != undefined ? dataRecordFieldSelectorAPI.getSelectedIds() : undefined
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrInvoiceInvoicefieldpropertyevaluator', InvoicePropertyEvaluator);

})(app);
