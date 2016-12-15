"use strict";

app.directive("retailInvoiceInvoicetypeRetailsubscriberinvoicesettings", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SubscriberInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Invoice/Directives/MainExtensions/SubscriberInvoiceSettings/Templates/SubscriberInvoiceSettingsTemplate.html"

        };

        function SubscriberInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                };


                api.getData = function () {
                    return {
                        $type: "Retail.Invoice.Business, Retail.Invoice.Business.RetailSubscriberInvoiceSettings"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);