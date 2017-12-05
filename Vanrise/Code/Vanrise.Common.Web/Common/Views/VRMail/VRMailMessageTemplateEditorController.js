(function (appControllers) {

    "use strict";

    MailMessageTemplateEditorController.$inject = ['$scope', 'VRCommon_VRMailMessageTemplateAPIService', 'VRCommon_VRMailMessageTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function MailMessageTemplateEditorController($scope, VRCommon_VRMailMessageTemplateAPIService, VRCommon_VRMailMessageTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var mailMessageTemplateId;
        var mailMessageTemplateEntity;
        var mailMessageTypeId;
        var fixedMailMessageTypeId;
        var mailMessageTypeSelectorAPI;
        var mailMessageTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var onMailMessageTypeSelectionChangedDeferred;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var drillDownManager;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                mailMessageTemplateId = parameters.mailMessageTemplateId;
                fixedMailMessageTypeId = parameters.mailMessageTypeId;

            }

            isEditMode = (mailMessageTemplateId != undefined);

        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.objects = [];
            $scope.scopeModel.menuActions = [];
            $scope.scopeModel.disabledMailMessageType = fixedMailMessageTypeId != undefined;

            $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                mailMessageTypeSelectorAPI = api;
                mailMessageTypeSelectoReadyDeferred.resolve();
            };
            $scope.scopeModel.onMailMessageTypeSelectionChanged = function () {
                mailMessageTypeId = mailMessageTypeSelectorAPI.getSelectedIds();
                if (mailMessageTypeId != undefined) {

                    if (onMailMessageTypeSelectionChangedDeferred != undefined) {
                        onMailMessageTypeSelectionChangedDeferred.resolve();
                    }
                    else {
                        $scope.scopeModel.isGridLoading = true;
                        getMailMessageType(undefined);
                    }
                }
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions, true);
                gridReadyDeferred.resolve();
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
                    $scope.scopeModel.from = mailMessageTemplateEntity.Settings.From != undefined ?mailMessageTemplateEntity.Settings.From.ExpressionString:undefined;
                    $scope.scopeModel.to = mailMessageTemplateEntity.Settings.To.ExpressionString;
                    $scope.scopeModel.cc = mailMessageTemplateEntity.Settings.CC.ExpressionString;
                    $scope.scopeModel.bcc = mailMessageTemplateEntity.Settings.BCC != undefined ? mailMessageTemplateEntity.Settings.BCC.ExpressionString : undefined;
                    $scope.scopeModel.subject = mailMessageTemplateEntity.Settings.Subject.ExpressionString;
                    $scope.scopeModel.body = mailMessageTemplateEntity.Settings.Body.ExpressionString;
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
                    if (fixedMailMessageTypeId != undefined) {
                        mailMessageTypeSelectorPayload = {
                            selectedIds: fixedMailMessageTypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, mailMessageTypeSelectorPayload, mailMessageTypeSelectorLoadDeferred);
                });
                return mailMessageTypeSelectorLoadDeferred.promise;
            }
            function loadGrid() {
                if (!isEditMode) return;

                onMailMessageTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                var gridLoadDeferred = UtilsService.createPromiseDeferred();

                onMailMessageTypeSelectionChangedDeferred.promise.then(function () {
                    gridReadyDeferred.promise.then(function () {
                        onMailMessageTypeSelectionChangedDeferred = undefined;
                        if (mailMessageTypeId != undefined)
                            getMailMessageType(gridLoadDeferred);
                    });
                });

                return gridLoadDeferred.promise;
            }
        }

        function getMailMessageTemplate() {
            return VRCommon_VRMailMessageTemplateAPIService.GetMailMessageTemplate(mailMessageTemplateId).then(function (response) {
                mailMessageTemplateEntity = response;
            });
        }
        function getMailMessageType(loadPromiseDeferred) {
            return VRCommon_VRMailMessageTypeAPIService.GetMailMessageType(mailMessageTypeId).then(function (response) {
                var mailMessageTypeEntity = response;

                if (mailMessageTypeEntity.Settings != undefined) {
                    $scope.scopeModel.objects = [];
                    var mailMessageTypeObjects = mailMessageTypeEntity.Settings.Objects;
                    var mailMessageTypeObject;
                    for (var key in mailMessageTypeObjects) {
                        if (key != "$type") {
                            mailMessageTypeObject = mailMessageTypeObjects[key];
                            $scope.scopeModel.objects.push(mailMessageTypeObject);

                            drillDownManager.setDrillDownExtensionObject(mailMessageTypeObject);
                        }
                    }
                }
                $scope.scopeModel.isGridLoading = false;
                if (loadPromiseDeferred != undefined)
                    loadPromiseDeferred.resolve();
            });
        }

        function buildDrillDownTabs() {
            var drillDownTabs = [];

            drillDownTabs.push(buildObjectTypePropertiesTab());

            function buildObjectTypePropertiesTab() {
                var objectTypePropertiesTab = {};

                objectTypePropertiesTab.title = 'Properties';
                objectTypePropertiesTab.directive = 'vr-common-objecttypeproperty-grid';

                objectTypePropertiesTab.loadDirective = function (objectTypePropertyGridAPI, mailMessageTypeObject) {
                    mailMessageTypeObject.objectTypePropertyGridAPI = objectTypePropertyGridAPI;
                    var objectTypePropertyGridPayload = {
                        objectVariable: mailMessageTypeObject
                    };
                    return mailMessageTypeObject.objectTypePropertyGridAPI.load(objectTypePropertyGridPayload);
                };

                return objectTypePropertiesTab;
            }

            return drillDownTabs;
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

        function buildMailMessageTemplateObjFromScope() {

            return {
                VRMailMessageTemplateId: mailMessageTemplateEntity != undefined ? mailMessageTemplateEntity.VRMailMessageTemplateId : undefined,
                Name: $scope.scopeModel.name,
                VRMailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds(),
                Settings: {
                    Variables: $scope.scopeModel.objects,
                    From: {ExpressionString: $scope.scopeModel.from},
                    To: { ExpressionString: $scope.scopeModel.to },
                    CC: { ExpressionString: $scope.scopeModel.cc },
                    BCC: { ExpressionString: $scope.scopeModel.bcc },
                    Subject: { ExpressionString: $scope.scopeModel.subject },
                    Body: { ExpressionString: $scope.scopeModel.body }
                }
            };
        }
    }

    appControllers.controller('VRCommon_VRMailMessageTemplateEditorController', MailMessageTemplateEditorController);

})(appControllers);