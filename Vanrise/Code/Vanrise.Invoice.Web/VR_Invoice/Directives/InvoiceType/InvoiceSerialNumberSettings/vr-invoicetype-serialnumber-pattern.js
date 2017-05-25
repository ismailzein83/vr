﻿"use strict";

app.directive("vrInvoicetypeSerialnumberPattern", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VRModalService","VR_Invoice_InvoiceTypeAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService, VR_Invoice_InvoiceTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@',
                isrequired:"="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OverallInvoiceCounterSerialNumberPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            if (attrs.hidelabel != undefined)
                withemptyline = '';
            var label = "Serial Number Pattern";
            if (attrs.label != undefined)
                label = attrs.label;
            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
                             + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                             + '<vr-textbox value="ctrl.value" isrequired="ctrl.isrequired"></vr-textbox>'
                         + '</vr-columns>'
                         + '<vr-columns width="normal" ' + withemptyline + '>'
                            + '<vr-button type="Help" data-onclick="scopeModel.openSerialNumberPatternHelper" standalone></vr-button>'
                         + '</vr-columns>';
            return template;

        }
        function OverallInvoiceCounterSerialNumberPart($scope, ctrl, $attrs) {
            var context;
            var invoiceTypeId;
            var parts;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.openSerialNumberPatternHelper = function () {
                    var modalSettings = {};

                    modalSettings.onScopeReady = function (modalScope) {
                        modalScope.onSetSerialNumberPattern = function (serialNumberPatternValue) {
                            if (ctrl.value == undefined)
                                ctrl.value = "";
                            ctrl.value += serialNumberPatternValue;
                        };
                    };
                    var parameter = {
                    context: getContext()
                    };
                    VRModalService.showModal('/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/Templates/SerialNumberPatternHelper.html', parameter, modalSettings);
                };
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    context = payload.context;
                    parts = payload.parts;
                    invoiceTypeId = payload.invoiceTypeId;
                    if (payload.serialNumberPattern != undefined)
                        ctrl.value = payload.serialNumberPattern;
                    var promises = [];
                    if (invoiceTypeId != undefined && parts == undefined) {
                        var promise = VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                            parts = response.Settings.InvoiceSerialNumberSettings.SerialNumberParts;
                      });
                      promises.push(promise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                }
            };

            api.getData = function () {
                return ctrl.value;
            };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext()
            {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                if (currentContext.getParts == undefined)
                {
                    currentContext.getParts = function () {
                        var returnedParts = [];
                        if (parts != undefined)
                        {
                            for (var i = 0, length = parts.length ; i < length; i++)
                            {
                                returnedParts.push(parts[i]);
                            }
                        }
                        return returnedParts;
                    };
                }
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);