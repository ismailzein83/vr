﻿"use strict";

app.directive("vrInvoicetypeInvoicefilterconditionIssentinvoice", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new IsSentInvoiceFilterCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceGridSettings/MainExtensions/InvoiceFilterCondition/Templates/IsSentInvoiceFilterConditionTemplate.html"

        };

        function IsSentInvoiceFilterCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        context = payload.context;
                    }
                    if (invoiceFilterConditionEntity != undefined) {
                        $scope.scopeModel.isSent = invoiceFilterConditionEntity.IsSent;
                    }
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.IsSentInvoiceFilterCondition ,Vanrise.Invoice.MainExtensions",
                        IsSent: $scope.scopeModel.isSent
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