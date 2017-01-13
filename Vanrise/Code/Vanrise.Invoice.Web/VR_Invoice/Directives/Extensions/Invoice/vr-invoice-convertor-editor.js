'use strict';

app.directive('vrInvoiceConvertorEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceConvertor = new InvoiceConvertor($scope, ctrl, $attrs);
                invoiceConvertor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/Invoice/Templates/InvoiceConvertorTemplate.html'
        };

        function InvoiceConvertor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {


                    if (payload != undefined) {

                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.Convertors.InvoiceToVRObjectConvertor, Vanrise.Invoice.MainExtensions"
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
