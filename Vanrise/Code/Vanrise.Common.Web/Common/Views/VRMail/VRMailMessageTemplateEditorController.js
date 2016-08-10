(function (appControllers) {

    "use strict";

    MailMessageTemplateEditorController.$inject = ['$scope', 'VRCommon_VRMailMessageTemplateAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function MailMessageTemplateEditorController($scope, VRCommon_VRMailMessageTemplateAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var mailMessageTemplateId;
        var mailMessageTemplateEntity;

        var mailMessageTypeSelectorAPI;
        var mailMessageTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                mailMessageTemplateId = parameters.mailMessageTemplateId;
            }

            isEditMode = (mailMessageTemplateId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.variables = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                mailMessageTypeSelectorAPI = api;
                mailMessageTypeSelectoReadyDeferred.resolve();
            }

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_StyleDefinitionAPIService.GetFilteredStyleDefinitions(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getMailMessageTemplate().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }


        function getMailMessageTemplate() {
            return VRCommon_VRMailMessageTemplateAPIService.GetMailMessageTemplate(mailMessageTemplateId).then(function (response) {
                mailMessageTemplateEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadMailMessageTypeSelector, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var mailMessageTemplateName = (mailMessageTemplateEntity != undefined) ? mailMessageTemplateEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(mailMessageTemplateName, 'Mail Message Template');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Mail Message Template');
                }
            }
            function loadStaticData() {
                if (mailMessageTemplateEntity == undefined)
                    return;

                $scope.scopeModel.name = mailMessageTemplateEntity.Name;

                if (mailMessageTemplateEntity.Settings != undefined) {
                    $scope.scopeModel.to = mailMessageTemplateEntity.Settings.To;
                    $scope.scopeModel.cc = mailMessageTemplateEntity.Settings.CC;
                    $scope.scopeModel.subject = mailMessageTemplateEntity.Settings.Subject;
                }
            }
            function loadMailMessageTypeSelector() {
                var mailMessageTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                mailMessageTypeSelectoReadyDeferred.promise.then(function () {
                    var mailMessageTypeSelectorPayload = null;
                    if (isEditMode) {
                        mailMessageTypeSelectorPayload = {
                            selectedIds: mailMessageTemplateEntity.VRMailMessageTypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, mailMessageTypeSelectorPayload, mailMessageTypeSelectorLoadDeferred);
                });
                return mailMessageTypeSelectorLoadDeferred.promise;
            }
            function loadGrid() {
                var gridLoadDeferred = UtilsService.createPromiseDeferred();
                gridReadyDeferred.promise.then(function () {
                    var mailMessageTypeSelectorPayload = null;
                    if (isEditMode) {
                        mailMessageTypeSelectorPayload = {
                            selectedIds: mailMessageTemplateEntity.VRMailMessageTypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, mailMessageTypeSelectorPayload, gridLoadDeferred);
                });
                return gridLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRMailMessageTemplateAPIService.AddMailMessageTemplate(buildMailMessageTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('MailMessageTemplate', response, 'Name')) {
                    if ($scope.onMailMessageTemplateAdded != undefined)
                        $scope.onMailMessageTemplateAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRMailMessageTemplateAPIService.UpdateMailMessageTemplate(buildMailMessageTemplateObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('MailMessageTemplate', response, 'Name')) {
                    if ($scope.onMailMessageTemplateUpdated != undefined) {
                        $scope.onMailMessageTemplateUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildContext() {

            var context = {
                getObjectVariables: function () { return objectDirectiveAPI.getData(); }
            }
            return context;
        }
        function buildMailMessageTemplateObjFromScope() {

            //var objects = objectDirectiveAPI.getData();
            //var variables = variableDirectiveAPI.getData();

            return {
                VRMailMessageTemplateId: mailMessageTemplateEntity != undefined ? mailMessageTemplateEntity.VRMailMessageTemplateId : undefined,
                Name: $scope.scopeModel.name,
                VRMailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds(),
                Settings: {
                    To: $scope.scopeModel.to,
                    CC: $scope.scopeModel.cc,
                    Subject: $scope.scopeModel.subject
                }       
            };          
        }
    }

    appControllers.controller('VRCommon_MailMessageTemplateEditorController', MailMessageTemplateEditorController);

})(appControllers);