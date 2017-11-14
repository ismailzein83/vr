"use strict";

app.directive("vrInvoicetypeInvoicebulkactionRecreateinvoice", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RecreateInvoiceAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/RecreateInvoiceBulkActionSettingsTemplate.html"

        };

        function RecreateInvoiceAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var automaticInvoiceActionEntity;
                    if (payload != undefined) {
                        automaticInvoiceActionEntity = payload.automaticInvoiceActionEntity;
                        context = payload.context;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions.RecreateInvoiceBulkActionSettings ,Vanrise.Invoice.MainExtensions",
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