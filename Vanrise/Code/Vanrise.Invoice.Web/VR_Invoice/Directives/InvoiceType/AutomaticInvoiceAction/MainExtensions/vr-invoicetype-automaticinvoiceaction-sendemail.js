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
            var mailMessageTypeAPI;
            var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();
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
                $scope.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeAPI = api;
                    mailMessageTypeReadyDeferred.resolve();
                };
                $scope.isMailMessageTypeRequired = function () {
                    if(ctrl.datasource.length > 0)
                    {
                        for(var i=0;i<ctrl.datasource.length;i++)
                        {
                            var item = ctrl.datasource[i];
                            if (item.Entity.MailMessageTypeId != undefined)
                                return false;
                        }
                    }
                    return true;

                };
                ctrl.isValid = function () {
                    if (mailMessageTypeAPI.getSelectedIds() != undefined)
                        return null;
                    if (ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i];
                            if (item.Entity.MailMessageTypeId != undefined)
                                return null;
                        }
                    }
                    return "Mail message type should be selected.";
                };
                ctrl.removeEmailAttachmentSet = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                UtilsService.waitMultiplePromises([mailMessageTypeReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
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
                    function loadMailMessageTypeSelector() {
                        var mailMessageTypePayload;
                        if (automaticInvoiceActionEntity != undefined) {
                            mailMessageTypePayload = {
                                selectedIds: automaticInvoiceActionEntity.MailMessageTypeId
                            };
                        }
                        return mailMessageTypeAPI.load(mailMessageTypePayload);
                    }
                    promises.push(loadMailMessageTypeSelector());
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
                        EmailActionAttachmentSets: emailAttachmentSets,
                        MailMessageTypeId: mailMessageTypeAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.isMailMessageTypeSelected = function () {
                    if (mailMessageTypeAPI.getSelectedIds() != undefined)
                        return true;
                    return false;
                };
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