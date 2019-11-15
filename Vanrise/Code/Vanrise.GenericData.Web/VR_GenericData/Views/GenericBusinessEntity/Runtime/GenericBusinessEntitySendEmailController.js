(function (appControllers) {

    "use strict";

    GenericBusinessEntitySendEmailController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEEmailActionAPIService', 'VRCommon_VRMailAPIService','VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBusinessEntitySendEmailController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBEEmailActionAPIService, VRCommon_VRMailAPIService, VR_GenericData_GenericBusinessEntityAPIService) {
        var genericBusinessEntityId;
        var businessEntityDefinitionId;
        var genericBEActionId;
        var genericBusinessEntity;
        var genericBEEntity;
        var genericBETemplateEntity;
        var infoType;
        var genericBEMailTemplateReadyAPI;
        var genericBEMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var fileAPI;
        var selectedMailTemplateId;
        var mailTemplateSelectedPromiseDeferred;
        $scope.scopeModel = {};
        $scope.scopeModel.showMailMessageTemplateSelector = false;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {

                genericBusinessEntityId = parameters.genericBEDefinitionId;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
                genericBEEntity = parameters.genericBEAction;
                if (genericBEEntity != undefined) {
                    if (genericBEEntity.Settings != undefined) {
                        $scope.scopeModel.showMailMessageTemplateSelector = (genericBEEntity.Settings.MailMessageTypeId != undefined);
                        infoType = genericBEEntity.Settings.InfoType;
                    }
                    genericBEActionId = genericBEEntity.GenericBEActionId;
                }
            }
        }

        function defineScope() {
            $scope.scopeModel.uploadedAttachements = [];
            $scope.scopeModel.onUploadedAttachementFileReady = function (api) {
                fileAPI = api;
            };
            $scope.scopeModel.downloadAttachement = function (attachedfileId) {
                $scope.scopeModel.isLoading = true;
                return VRCommon_VRMailAPIService.DownloadAttachement(attachedfileId).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (response != undefined)
                        UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.scopeModel.addUploadedAttachement = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.uploadedAttachements.push({ value: { fileId: obj.fileId } });
                    fileAPI.clearFileUploader();
                }
            };
            $scope.scopeModel.onGenericBEMailTemplateSelectorReady = function (api) {
                genericBEMailTemplateReadyAPI = api;
                genericBEMailTemplateReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.sendEmail = function () {
                return sendEmail();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onGenericBEMailTemplateSelectionChanged = function (value) {
                $scope.scopeModel.isLoading = true;
                if (value != undefined) {
                    selectedMailTemplateId = value.VRMailMessageTemplateId;
                    if (mailTemplateSelectedPromiseDeferred != undefined) {
                        mailTemplateSelectedPromiseDeferred.resolve();
                    }
                    else {
                        getGenericBEEmail().then(function () {
                            if (genericBETemplateEntity != undefined && genericBETemplateEntity.VRMailEvaluatedTemplate != undefined) {
                                $scope.scopeModel.cc = genericBETemplateEntity.VRMailEvaluatedTemplate.CC != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.CC.split(';') : [];
                                $scope.scopeModel.to = genericBETemplateEntity.VRMailEvaluatedTemplate.To != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.To.split(';') : [];
                                $scope.scopeModel.subject = genericBETemplateEntity.VRMailEvaluatedTemplate.Subject;
                                $scope.scopeModel.body = genericBETemplateEntity.VRMailEvaluatedTemplate.Body;
                                $scope.scopeModel.from = genericBETemplateEntity.VRMailEvaluatedTemplate.From != "" ? genericBETemplateEntity.VRMailEvaluatedTemplate.From : null;

                            }
                            $scope.scopeModel.attachments = genericBETemplateEntity.EmailAttachments;

                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                            $scope.scopeModel.isLoading = false;
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
                }
                else {
                    $scope.scopeModel.cc = [];
                    $scope.scopeModel.to = [];
                    $scope.scopeModel.subject = undefined;
                    $scope.scopeModel.body = undefined;
                    $scope.scopeModel.from = null;
                    $scope.scopeModel.isLoading = false;
                }

            };

            function sendEmail() {
                $scope.scopeModel.isLoading = true;

                var emailObject = buildGenericBETemplateObjFromScope();
                return VR_GenericData_GenericBEEmailActionAPIService.SendEmail(emailObject)
                    .then(function (response) {
                        $scope.modalContext.closeModal();
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            tryLoadMailMsgTemplateSelector().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }


        function tryLoadMailMsgTemplateSelector() {
            var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            if ($scope.scopeModel.showMailMessageTemplateSelector) {
                genericBEMailTemplateReadyPromiseDeferred.promise.then(function () {
                    if (infoType != undefined) {
                        VR_GenericData_GenericBEEmailActionAPIService.GetMailTemplateIdByInfoType(genericBusinessEntityId, businessEntityDefinitionId, infoType).then(function (response) {
                            selectedMailTemplateId = response;
                            var selectorPayload = { filter: { VRMailMessageTypeId: genericBEEntity.Settings.MailMessageTypeId }, selectedIds: selectedMailTemplateId };
                            VRUIUtilsService.callDirectiveLoad(genericBEMailTemplateReadyAPI, selectorPayload, mailMsgTemplateSelectorLoadDeferred);
                        });
                    }
                    else {
                        var selectorPayload = { filter: { VRMailMessageTypeId: genericBEEntity.Settings.MailMessageTypeId }, selectFirstItem: true };
                        VRUIUtilsService.callDirectiveLoad(genericBEMailTemplateReadyAPI, selectorPayload, mailMsgTemplateSelectorLoadDeferred);
                    }
                });
                return mailMsgTemplateSelectorLoadDeferred.promise;
            }
            else {
                mailMsgTemplateSelectorLoadDeferred.resolve();
                return mailMsgTemplateSelectorLoadDeferred.promise;
            }
        }
        function getGenericBusinessEntity() {

            return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                genericBusinessEntity = response;
                if (genericBusinessEntity != undefined && genericBusinessEntity.FieldValues != undefined && genericBusinessEntity.FieldValues.Attachments != undefined && genericBusinessEntity.FieldValues.Attachments.length > 0) {
                    var attachments = genericBusinessEntity.FieldValues.Attachments;
                    for (var j = 0; j < attachments.length; j++) {
                        $scope.scopeModel.uploadedAttachements.push({ value: { fileId: attachments[j].FileId } });
                    }
                }
            });
        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = "Send Email";
            }
            function loadStaticData() {
                if (genericBETemplateEntity != undefined && genericBETemplateEntity.VRMailEvaluatedTemplate != undefined) {
                    $scope.scopeModel.cc = genericBETemplateEntity.VRMailEvaluatedTemplate.CC != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.CC.split(';') : [];
                    $scope.scopeModel.to = genericBETemplateEntity.VRMailEvaluatedTemplate.To != undefined ? genericBETemplateEntity.VRMailEvaluatedTemplate.To.split(';') : [];
                    $scope.scopeModel.subject = genericBETemplateEntity.VRMailEvaluatedTemplate.Subject;
                    $scope.scopeModel.body = genericBETemplateEntity.VRMailEvaluatedTemplate.Body;
                    $scope.scopeModel.from = genericBETemplateEntity.VRMailEvaluatedTemplate.From != "" ? genericBETemplateEntity.VRMailEvaluatedTemplate.From : null;

                }
            }
            return UtilsService.waitPromiseNode({
                promises: [getGenericBEEmail()],
                getChildNode: function () {
                    setTitle();
                    loadStaticData();
                    return { promises: [getGenericBusinessEntity()] };
                }
            }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
                
        }

        function getGenericBEEmail() {

            var selectedMailTemplatePromiseDeferred = UtilsService.createPromiseDeferred();

            if (selectedMailTemplateId == undefined) {
                if (infoType != undefined) {
                    VR_GenericData_GenericBEEmailActionAPIService.GetMailTemplateIdByInfoType(genericBusinessEntityId, businessEntityDefinitionId, infoType).then(function (response) {
                        selectedMailTemplateId = response;
                        selectedMailTemplatePromiseDeferred.resolve();
                    });
                }
                else {
                    selectedMailTemplateId = genericBEMailTemplateReadyAPI.getSelectedIds();
                    selectedMailTemplatePromiseDeferred.resolve();
                }
            }
            else {
                selectedMailTemplatePromiseDeferred.resolve();
            }

            return UtilsService.waitPromiseNode({
                promises: [selectedMailTemplatePromiseDeferred.promise],
                getChildNode: function () {
                    mailTemplateSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    var emailTempatePromiseDeferred = UtilsService.createPromiseDeferred();
                    VR_GenericData_GenericBEEmailActionAPIService.GetEmailTemplate(genericBusinessEntityId, businessEntityDefinitionId, selectedMailTemplateId, genericBEActionId).then(function (response) {
                        genericBETemplateEntity = response;
                        emailTempatePromiseDeferred.resolve();
                        mailTemplateSelectedPromiseDeferred = undefined;
                    });
                    return { promises: [emailTempatePromiseDeferred.promise] };
                }
            });
        }

        function buildGenericBETemplateObjFromScope() {
            var attachementFileIds = $scope.scopeModel.uploadedAttachements.map(function (a) { return a.value.fileId; });

            var obj = {
                BusinessEntityDefinitionId: businessEntityDefinitionId,
                GenericBusinessEntityId: genericBusinessEntityId,
                GenericBEActionId: genericBEActionId,
                EmailTemplate: {
                    From: $scope.scopeModel.from,
                    CC: $scope.scopeModel.cc.join(';'),
                    To: $scope.scopeModel.to.join(';'),
                    Subject: $scope.scopeModel.subject,
                    Body: $scope.scopeModel.body,
                },
                AttachementFileIds: attachementFileIds
            };
            return obj;
        }

    }

    appControllers.controller('VR_GenericData_GenericBusinessEntitySendEmailController', GenericBusinessEntitySendEmailController);
})(appControllers);
