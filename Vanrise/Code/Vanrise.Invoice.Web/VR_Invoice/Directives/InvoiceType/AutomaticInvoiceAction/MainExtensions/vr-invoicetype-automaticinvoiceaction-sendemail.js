"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactionSendemail", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceEmailActionService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceEmailActionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SendEmailAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/AutomaticSendEmailActionTemplate.html"

        };

        function SendEmailAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.addEmailAttachmentSet = function () {
                    var onEmailAttachmentSetAdded = function (emailAttachmentSet) {
                        ctrl.datasource.push({ Entity: emailAttachmentSet });
                    };

                    VR_Invoice_InvoiceEmailActionService.addEmailAttachmentSet(onEmailAttachmentSetAdded, getContext());
                };

                ctrl.removeEmailAttachmentSet = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var automaticInvoiceActionEntity;
                    if (payload != undefined) {
                        automaticInvoiceActionEntity = payload.automaticInvoiceActionEntity;
                        context = payload.context;
                        if (automaticInvoiceActionEntity != undefined && automaticInvoiceActionEntity.EmailActionAttachmentSets != undefined) {
                            for (var i = 0; i < automaticInvoiceActionEntity.EmailActionAttachmentSets.length; i++) {
                                var emailAttachmentSet = automaticInvoiceActionEntity.EmailActionAttachmentSets[i];
                                ctrl.datasource.push({ Entity: emailAttachmentSet });
                            }
                        }
                    }

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var emailAttachmentSets;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        emailAttachmentSets = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            emailAttachmentSets.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions.AutomaticSendEmailAction ,Vanrise.Invoice.MainExtensions",
                        EmailActionAttachmentSets: emailAttachmentSets
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

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editEmailAttachmentSet,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editEmailAttachmentSet(emailAttachmentSetObj) {
                var onEmailAttachmentSetUpdated = function (emailAttachmentSet) {
                    var index = ctrl.datasource.indexOf(emailAttachmentSetObj);
                    ctrl.datasource[index] = { Entity: emailAttachmentSet };
                };
                VR_Invoice_InvoiceEmailActionService.editEmailAttachmentSet(emailAttachmentSetObj.Entity, onEmailAttachmentSetUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);