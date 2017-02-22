'use strict';

app.directive('partnerportalInvoicePartnerinvoicequeryinterceptor', ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new partnerInvoiceQueryInterceptorCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/PartnerPortal_Invoice/Elements/Invoice/Directives/MainExtensions/Templates/PartnerInvoiceQueryInterceptor.html'
    };


    function partnerInvoiceQueryInterceptorCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

            };

            api.getData = function () {
                var obj = {
                    $type: "PartnerPortal.Invoice.MainExtensions.Invoice.PartnerInvoiceQueryInterceptor, PartnerPortal.Invoice.MainExtensions",
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);