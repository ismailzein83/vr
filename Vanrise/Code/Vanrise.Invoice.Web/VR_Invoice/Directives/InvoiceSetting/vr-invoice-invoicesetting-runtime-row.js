'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeRow', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/Templates/InvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;
            var invoiceTypeId;
            function initializeController() {
                ctrl.parts = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.parts != undefined) {
                        invoiceTypeId = payload.invoiceTypeId;
                        currentContext = payload.context;
                        var promises = [];
                        for (var i = 0; i < payload.parts.length; i++) {
                            var part = payload.parts[i];
                            part.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            part.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(part.loadPromiseDeferred.promise);
                            preparePartObject(part);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var parts = {};
                    for (var i = 0; i < ctrl.parts.length; i++) {
                        var part = ctrl.parts[i];
                        if (part.partAPI != undefined)
                            parts[part.PartConfigId] = part.partAPI.getData();
                    }
                    return parts;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function preparePartObject(part) {

                if (currentContext != undefined)
                    part.runTimeEditor = currentContext.getRuntimeEditor(part.PartConfigId);
                part.onPartDirectiveReady = function (api) {
                    part.partAPI = api;
                    part.readyPromiseDeferred.resolve();
                };
                var payload = {
                    fieldValue: currentContext != undefined ? currentContext.getPartsPathValue(part.PartConfigId) : undefined,
                    invoiceTypeId: invoiceTypeId
                };
                if (part.readyPromiseDeferred != undefined) {
                    part.readyPromiseDeferred.promise.then(function () {
                        part.readyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(part.partAPI, payload, part.loadPromiseDeferred);
                    });
                }
                ctrl.parts.push(part);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);