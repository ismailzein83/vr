"use strict";

app.directive("vrInvoicetypeGridactionsettingsDownloadfile", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DownloadFileAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceActions/MainExtensions/Templates/DownloadFileActionTemplate.html"

        };

        function DownloadFileAction($scope, ctrl, $attrs) {
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

                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                        context.showSecurityGridCallBack(false);

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.DownloadFileInvoiceAction ,Vanrise.Invoice.MainExtensions",
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