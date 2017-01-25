'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeSerialnumberpatterninvoicesettingpart', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/SerialNumberPatternInvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;
            var serialNumberPatternAPI;
            var serialNumberPatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSerialNumberPatternReady = function (api) {
                    serialNumberPatternAPI = api;
                    serialNumberPatternReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.fieldValue != undefined)
                    {
                        $scope.scopeModel.enableAutomaticInvoice = payload.fieldValue.IsEnabled;
                    }

                    function loadSerialNumberPattern() {
                        var serialNumberPatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        serialNumberPatternReadyPromiseDeferred.promise.then(function () {
                            var serialNumberPatternDirectivePayload = { invoiceTypeId:payload.invoiceTypeId  };
                            if (payload != undefined && payload.fieldValue!=undefined)
                                serialNumberPatternDirectivePayload.serialNumberPattern = payload.fieldValue.SerialNumberPattern;
                            VRUIUtilsService.callDirectiveLoad(serialNumberPatternAPI, serialNumberPatternDirectivePayload, serialNumberPatternDeferredLoadPromiseDeferred);
                        });
                        return serialNumberPatternDeferredLoadPromiseDeferred.promise;
                    }
                    return loadSerialNumberPattern();
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.SerialNumberPatternInvoiceSettingPart,Vanrise.Invoice.Entities",
                        SerialNumberPattern: serialNumberPatternAPI.getData()
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