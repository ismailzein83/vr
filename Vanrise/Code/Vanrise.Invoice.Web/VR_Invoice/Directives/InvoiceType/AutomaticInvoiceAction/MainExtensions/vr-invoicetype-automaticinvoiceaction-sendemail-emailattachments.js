"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactionSendemailEmailattachments", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceEmailActionService",
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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/EmailAttachmentsTemplate.html"

        };

        function SendEmailAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.addEmailAttachment = function () {
                    var onEmailAttachmentAdded = function (emailAttachment) {
                        ctrl.datasource.push({ Entity: emailAttachment });
                    };

                    VR_Invoice_InvoiceEmailActionService.addEmailAttachment(onEmailAttachmentAdded, getContext());
                };

                ctrl.removeEmailAttachment = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var emailAttachmentsEntity;
                    if (payload != undefined) {
                        emailAttachmentsEntity = payload.emailAttachmentsEntity;
                        context = payload.context;
                        if (emailAttachmentsEntity != undefined) {
                            for (var i = 0; i < emailAttachmentsEntity.length; i++) {
                                var emailAttachment = emailAttachmentsEntity[i];
                                ctrl.datasource.push({ Entity: emailAttachment });
                            }
                        }
                    }

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var emailAttachments;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        emailAttachments = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            emailAttachments.push(currentItem.Entity);
                        }
                    }
                    return emailAttachments;
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
                    clicked: editEmailAttachment,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editEmailAttachment(emailAttachmentObj) {
                var onEmailAttachmentUpdated = function (emailAttachment) {
                    var index = ctrl.datasource.indexOf(emailAttachmentObj);
                    ctrl.datasource[index] = { Entity: emailAttachment };
                };
                VR_Invoice_InvoiceEmailActionService.editEmailAttachment(emailAttachmentObj.Entity, onEmailAttachmentUpdated, getContext());
            }
        }

        return directiveDefinitionObject;

    }
]);