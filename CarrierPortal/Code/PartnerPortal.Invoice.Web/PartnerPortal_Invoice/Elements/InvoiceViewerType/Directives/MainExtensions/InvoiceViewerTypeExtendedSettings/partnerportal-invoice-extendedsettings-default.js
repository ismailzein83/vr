'use strict';

app.directive('partnerportalInvoiceExtendedsettingsDefault', ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new defaultInvoiceViewerExtendedSettingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/PartnerPortal_Invoice/Elements/InvoiceViewerType/Directives/MainExtensions/InvoiceViewerTypeExtendedSettings/Templates/DefaultInvoiceViewerExtendedSettings.html'
    };


    function defaultInvoiceViewerExtendedSettingsCtor(ctrl, $scope, $attrs) {
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
                    $type: "PartnerPortal.Invoice.MainExtensions.DefaultInvoiceViewerExtendedSettings, PartnerPortal.Invoice.MainExtensions",
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);