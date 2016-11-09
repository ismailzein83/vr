"use strict";

app.directive("vrInvoicetypeGridactionsettingsSetinvoicepaid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SetInvoicePaidAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GridSettings/InvoiceGridActionSettings/MainExtensions/GridActionSettings/Templates/SetInvoicePaidActionTemplate.html"

        };

        function SetInvoicePaidAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {
                $scope.scopeModel = {};
             
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceGridActionEntity;
                    if (payload != undefined) {
                        invoiceGridActionEntity = payload.invoiceGridActionEntity;
                        context = payload.context;
                    }
                    if (invoiceGridActionEntity != undefined) {
                        $scope.scopeModel.isInvoicePaid = invoiceGridActionEntity.IsInvoicePaid;
                    }
                    var promises = [];
                   
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.SetInvoicePaidAction ,Vanrise.Invoice.MainExtensions",
                        IsInvoicePaid: $scope.scopeModel.isInvoicePaid,
                    };
                }

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