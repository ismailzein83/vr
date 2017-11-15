"use strict";

app.directive("vrInvoicetypeAutomaticinvoiceactionSendemailRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SendEmailActionRuntime($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/AutomaticInvoiceAction/MainExtensions/Templates/EmailAttachmentsRuntimeTemplate.html"

        };

        function SendEmailActionRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            var invoiceAttachments;
            var mailMessageTemplateSelectorAPI;
            var mailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var isAutomatic;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.scopeModel.onMailMessageTemplateDirectiveReady = function (api) {
                    mailMessageTemplateSelectorAPI = api;
                    mailMessageTemplateSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.showMaileTypeColumn = false;
                $scope.scopeModel.showIncludeSentInvocies = true;
                defineAPI();
            }

            function defineAPI() {
                var api = {};
               // 
                api.load = function (payload) {
                    var emailActionSettings;
                    var promises = [];
                    var actionValueSettings;
                    if (payload != undefined) {
                        invoiceAttachments = payload.invoiceAttachments;
                        $scope.scopeModel.showIncludeSentInvocies = !payload.isAutomatic;
                        emailActionSettings = payload.emailActionSettings;
                        actionValueSettings = payload.actionValueSettings;
                        context = payload.context;
                        if (emailActionSettings != undefined)
                        {
                            if (emailActionSettings.EmailActionAttachmentSets != undefined) {
                                for (var i = 0; i < emailActionSettings.EmailActionAttachmentSets.length; i++) {
                                    var emailAttachmentSet = emailActionSettings.EmailActionAttachmentSets[i];
                                    if (emailAttachmentSet.MailMessageTypeId != undefined)
                                        $scope.scopeModel.showMaileTypeColumn = true;
                                    var emailAttachmentSetPayload = {
                                        payload: emailAttachmentSet,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    promises.push(emailAttachmentSetPayload.loadPromiseDeferred.promise);
                                    if (emailAttachmentSet.MailMessageTypeId != undefined)
                                    {
                                        emailAttachmentSetPayload.readyMailTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                                        emailAttachmentSetPayload.loadMailTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                                        promises.push(emailAttachmentSetPayload.loadMailTemplatePromiseDeferred.promise);
                                    }
                                    var emailAttachmentSetValuePayload;
                                    if(actionValueSettings != undefined && actionValueSettings.EmailActionAttachmentSets != undefined)
                                    {
                                        emailAttachmentSetValuePayload = UtilsService.getItemByVal(actionValueSettings.EmailActionAttachmentSets, emailAttachmentSet.EmailActionAttachmentSetId, "EmailActionAttachmentSetId");
                                    }
                                    addDataItemAPI(emailAttachmentSetPayload, emailAttachmentSetValuePayload);
                                }
                            }
                            if(emailActionSettings.MailMessageTypeId != undefined)
                            {
                                $scope.scopeModel.showMailMessageTemplateSelector = true;
                                function loadMailMessageTypeSelector() {
                                    var mailMessageTemplateSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    mailMessageTemplateSelectorReadyDeferred.promise.then(function () {
                                        var mailMessageTemplateSelectorPayload = {
                                            context: getContext(),
                                            filter : {
                                                VRMailMessageTypeId: emailActionSettings.MailMessageTypeId
                                            },
                                            selectedIds: actionValueSettings != undefined ? actionValueSettings.MailMessageTemplateId : undefined
                                        };
                                        VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSelectorAPI, mailMessageTemplateSelectorPayload, mailMessageTemplateSelectorLoadPromiseDeferred);
                                    });
                                    return mailMessageTemplateSelectorLoadPromiseDeferred.promise;
                                }
                                promises.push(loadMailMessageTypeSelector());
                            }
                            if(actionValueSettings != undefined)
                            {
                                $scope.scopeModel.includeSentInvoices = actionValueSettings.IncludeSentInvoices;
                            }
                        }
                       
                    }
                    function addDataItemAPI(emailAttachmentSetPayload, emailAttachmentSetValuePayload)
                    {
                        var emailAttachmentSet = {
                            SetName: emailAttachmentSetPayload.payload.Name,
                            EmailActionAttachmentSetId: emailAttachmentSetPayload.payload.EmailActionAttachmentSetId,
                            IsEnabled:emailAttachmentSetValuePayload!=undefined? emailAttachmentSetValuePayload.IsEnabled:undefined,
                        };
                        emailAttachmentSet.onInvoiceAttachmentSelectorReady = function (api) {
                            emailAttachmentSet.invoiceAttachmentSelectorAPI = api;
                            emailAttachmentSetPayload.readyPromiseDeferred.resolve();
                        };
                        var attachmentIds;
                        if (emailAttachmentSetValuePayload != undefined && emailAttachmentSetValuePayload.Attachments != undefined)
                        {
                            attachmentIds = [];
                            for(var i=0;i<emailAttachmentSetValuePayload.Attachments.length;i++)
                            {
                                var attachment = emailAttachmentSetValuePayload.Attachments[i];
                                attachmentIds.push(attachment.AttachmentId);
                            }
                        }
                        emailAttachmentSetPayload.readyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                context: getContext(emailAttachmentSetPayload.payload.AttachmentsIds),
                                selectedIds: attachmentIds
                            };
                            VRUIUtilsService.callDirectiveLoad(emailAttachmentSet.invoiceAttachmentSelectorAPI, directivePayload, emailAttachmentSetPayload.loadPromiseDeferred);
                        });
                        if (emailAttachmentSetPayload.payload.MailMessageTypeId != undefined) {
                            emailAttachmentSet.showMailMessageTemplateSelector = true;
                            emailAttachmentSet.onMailMessageTemplateDirectiveReady = function (api) {
                                emailAttachmentSet.mailMessageTemplateSelectorAPI = api;
                                emailAttachmentSetPayload.readyMailTemplatePromiseDeferred.resolve();
                            };
                            emailAttachmentSetPayload.readyMailTemplatePromiseDeferred.promise.then(function () {
                                var directivePayload = {
                                    filter: {
                                        VRMailMessageTypeId: emailAttachmentSetPayload.payload.MailMessageTypeId 
                                    },
                                    selectedIds: emailAttachmentSetValuePayload != undefined ? emailAttachmentSetValuePayload.MailMessageTemplateId : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(emailAttachmentSet.mailMessageTemplateSelectorAPI, directivePayload, emailAttachmentSetPayload.loadMailTemplatePromiseDeferred);
                            });
                        }
                        ctrl.datasource.push({ Entity: emailAttachmentSet });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var emailActionAttachmentSets = [];
                    for (var i = 0, length = ctrl.datasource.length; i < length; i++) {
                        var emailActionAttachmentSet = ctrl.datasource[i];

                        var attachmentIds = emailActionAttachmentSet.Entity.invoiceAttachmentSelectorAPI.getSelectedIds();
                        var attachments = [];
                        if(attachmentIds != undefined)
                        {
                            for(var j=0;j<attachmentIds.length ;j++)
                            {
                                attachments.push({
                                    AttachmentId:attachmentIds[j]
                                });
                            }
                        }
                        emailActionAttachmentSets.push({
                            EmailActionAttachmentSetId: emailActionAttachmentSet.Entity.EmailActionAttachmentSetId,
                            Attachments: attachments,
                            MailMessageTemplateId: emailActionAttachmentSet.Entity.mailMessageTemplateSelectorAPI != undefined ? emailActionAttachmentSet.Entity.mailMessageTemplateSelectorAPI.getSelectedIds() : undefined,
                            IsEnabled: emailActionAttachmentSet.Entity.IsEnabled
                        });
                    }

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions.AutomaticSendEmailActionRuntimeSettings ,Vanrise.Invoice.MainExtensions",
                        EmailActionAttachmentSets:emailActionAttachmentSets,
                        MailMessageTemplateId: mailMessageTemplateSelectorAPI != undefined ? mailMessageTemplateSelectorAPI.getSelectedIds() : undefined,
                        IncludeSentInvoices: $scope.scopeModel.includeSentInvoices
                    };

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext(attachmentsIds) {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                currentContext.AttachmentsIds = attachmentsIds;
                currentContext.getInvoiceAttachmentsInfo = function () {
                    var invoiceAttachmentsInfo = [];
                    if (currentContext.AttachmentsIds != null) {
                        for (var i = 0, length = currentContext.AttachmentsIds.length; i < length; i++) {
                            var attachmentsId = currentContext.AttachmentsIds[i];
                            if (invoiceAttachments != null) {
                                var invoiceAttachment = UtilsService.getItemByVal(invoiceAttachments, attachmentsId, "InvoiceAttachmentId");
                                if (invoiceAttachment != null) {
                                    invoiceAttachmentsInfo.push({
                                        InvoiceAttachmentId: attachmentsId,
                                        Title: invoiceAttachment.Title
                                    });
                                }
                            }
                        }
                    }
                    return invoiceAttachmentsInfo;
                };
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);