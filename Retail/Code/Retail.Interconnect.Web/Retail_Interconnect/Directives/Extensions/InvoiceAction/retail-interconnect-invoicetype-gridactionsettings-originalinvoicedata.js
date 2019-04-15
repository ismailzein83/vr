"use strict";

app.directive("retailInterconnectInvoicetypeGridactionsettingsOriginalinvoicedata", ["UtilsService", "VRNotificationService", "VRUIUtilsService", 
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OriginalInvoiceDataAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Interconnect/Directives/Extensions/InvoiceAction/Templates/OriginalInvoiceDataActionTemplate.html"

        };

        function OriginalInvoiceDataAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceActionEntity;
                    if (payload != undefined) {
                        invoiceActionEntity = payload.invoiceActionEntity;
                        context = payload.context;
                    }
                    var promises = [];

                    if (invoiceActionEntity != undefined) {

                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Interconnect.Business.Extensions.OriginalInvoiceDataAction,Retail.Interconnect.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);