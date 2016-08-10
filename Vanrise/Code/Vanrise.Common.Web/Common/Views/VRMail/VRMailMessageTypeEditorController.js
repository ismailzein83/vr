(function (appControllers) {

    "use strict";

    MailMessageTypeEditorController.$inject = ['$scope', 'VRCommon_VRMailMessageTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function MailMessageTypeEditorController($scope, VRCommon_VRMailMessageTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var mailMessageTypeId;
        var mailMessageTypeEntity;

        var objectDirectiveAPI;
        var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var variableDirectiveAPI;
        var variableDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                mailMessageTypeId = parameters.mailMessageTypeId;
            }

            isEditMode = (mailMessageTypeId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onObjectDirectiveReady = function (api) {
                objectDirectiveAPI = api;
                objectDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onVariableDirectiveReady = function (api) {
                variableDirectiveAPI = api;
                variableDirectiveReadyDeferred.resolve();
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
                getMailMessageType().then(function () {
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


        function getMailMessageType() {
            return VRCommon_VRMailMessageTypeAPIService.GetMailMessageType(mailMessageTypeId).then(function (response) {
                mailMessageTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadObjectDirective, loadVariableDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var mailMessageTypeName = (mailMessageTypeEntity != undefined) ? mailMessageTypeEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(mailMessageTypeName, 'Mail Message Type');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Mail Message Type');
            }
        }
        function loadStaticData() {
            if (mailMessageTypeEntity == undefined)
                return;
            $scope.scopeModel.name = mailMessageTypeEntity.Name;
        }
        function loadObjectDirective() {
            var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            objectDirectiveReadyDeferred.promise.then(function () {
                var objectDirectivePayload;

                if (mailMessageTypeEntity != undefined && mailMessageTypeEntity.Settings != undefined && mailMessageTypeEntity.Settings.Objects != undefined) {

                    var settings = mailMessageTypeEntity.Settings;
                    var objects = [];
                    for (var key in settings.Objects) {
                        if (key != "$type")
                            objects.push(settings.Objects[key]);
                    }

                    objectDirectivePayload = {
                        objects: objects
                    };
                }

                VRUIUtilsService.callDirectiveLoad(objectDirectiveAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
            });

            return objectDirectiveLoadDeferred.promise;
        }
        function loadVariableDirective() {
            var variableDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            variableDirectiveReadyDeferred.promise.then(function () {
                var variableDirectivePayload = {};

                variableDirectivePayload.context = buildContext();

                if (mailMessageTypeEntity != undefined && mailMessageTypeEntity.Settings != undefined && mailMessageTypeEntity.Settings.Variables != undefined) {

                    var settings = mailMessageTypeEntity.Settings;
                    var variables = [];
                    for (var index = 0 ; index < settings.Variables.length; index++)
                            variables.push(settings.Variables[index]);

                    variableDirectivePayload.variables = variables
                }

                VRUIUtilsService.callDirectiveLoad(variableDirectiveAPI, variableDirectivePayload, variableDirectiveLoadDeferred);
            });

            return variableDirectiveLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_VRMailMessageTypeAPIService.AddMailMessageType(buildMailMessageTypeObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('MailMessageType', response, 'Name')) {
                    if ($scope.onMailMessageTypeAdded != undefined)
                        $scope.onMailMessageTypeAdded(response.InsertedObject);
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
            return VRCommon_VRMailMessageTypeAPIService.UpdateMailMessageType(buildMailMessageTypeObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('MailMessageType', response, 'Name')) {
                    if ($scope.onMailMessageTypeUpdated != undefined) {
                        $scope.onMailMessageTypeUpdated(response.UpdatedObject);
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
        function buildMailMessageTypeObjFromScope() {

            var objects = objectDirectiveAPI.getData();
            var variables = variableDirectiveAPI.getData();

            return {
                VRMailMessageTypeId: mailMessageTypeEntity != undefined ? mailMessageTypeEntity.VRMailMessageTypeId : undefined,
                Name: $scope.scopeModel.name,
                Settings: {
                    Objects: objects,
                    Variables: variables
                }
            };
        }
    }

    appControllers.controller('VRCommon_VRMailMessageTypeEditorController', MailMessageTypeEditorController);

})(appControllers);