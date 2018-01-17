'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeMinamountinvoicesettingpart', ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceSettingAPIService',
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

                var ctor = new minAmountCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/MinAmountPartTemplate.html';
            }

        };

        function minAmountCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};
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
                            $scope.scopeModel.minAmount = payload.fieldValue.MinAmount;
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.MinAmountInvoiceSettingPart,Vanrise.Invoice.Entities",
                        MinAmount: $scope.scopeModel.minAmount,
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