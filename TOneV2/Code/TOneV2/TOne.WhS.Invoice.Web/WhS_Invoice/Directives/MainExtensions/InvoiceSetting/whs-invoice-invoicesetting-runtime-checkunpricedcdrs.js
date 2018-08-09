'use strict';
app.directive('whsInvoiceInvoicesettingRuntimeCheckunpricedcdrs', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/WhS_Invoice/Directives/MainExtensions/InvoiceSetting/Templates/CheckUnpricedCDRsTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;

            function initializeController() {
                $scope.scopeModel = {};                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];
                        if (payload.fieldValue != undefined) {
                            $scope.scopeModel.enableCheckUnpricedCDRs = payload.fieldValue.IsEnabled;
                                                    }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CheckUnpricedCDRsInvoiceSettingPart,TOne.WhS.Invoice.Business",
                        IsEnabled: $scope.scopeModel.enableCheckUnpricedCDRs,
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