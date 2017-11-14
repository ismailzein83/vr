"use strict";

app.directive("vrInvoicetypeInvoicebulkactionRecreateinvoiceRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RecreateInvoiceActionRuntime($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/RecreateInvoiceBulkActionRuntimeSettingsTemplate.html"

        };

        function RecreateInvoiceActionRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var emailActionSettings;
                    var promises = [];
                    var actionValueSettings;
                    if (payload != undefined) {
                        emailActionSettings = payload.emailActionSettings;
                        actionValueSettings = payload.actionValueSettings;
                        if(actionValueSettings != undefined)
                        {
                            $scope.scopeModel.includeSentInvoices = actionValueSettings.IncludeSentInvoices;
                            $scope.scopeModel.includePaidInvoices = actionValueSettings.IncludePaidInvoices;

                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions.RecreateInvoiceBulkActionRuntimeSettings ,Vanrise.Invoice.MainExtensions",
                        IncludeSentInvoices: $scope.scopeModel.includeSentInvoices,
                        IncludePaidInvoices: $scope.scopeModel.includePaidInvoices

                    };

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;

    }
]);