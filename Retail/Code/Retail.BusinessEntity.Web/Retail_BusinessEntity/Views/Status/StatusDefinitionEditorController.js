(function (appControllers) {

    "use strict";

    StatusDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'Retail_BE_StatusDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function StatusDefinitionEditorController($scope, UtilsService, Retail_BE_StatusDefinitionAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var statusDefinitionId;
        var statusDefinitionEntity;

        var settingsDirectiveAPI;
        //var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                statusDefinitionId = parameters.statusDefinitionId;
            }

            isEditMode = (statusDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
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
                GetStatusDefinition().then(function () {
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

        function GetStatusDefinition() {
            return Retail_BE_StatusDefinitionAPIService.GetStatusDefinition(statusDefinitionId).then(function (response) {
                statusDefinitionEntity = response;
            });
        }
        function loadAllControls() {
            //return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //}).finally(function () {
            //    $scope.scopeModel.isLoading = false;
            //});
        }
        //function setTitle() {
        //    if (isEditMode) {
        //        var switchName = (switchEntity != undefined) ? switchEntity.Name : null;
        //        $scope.title = UtilsService.buildTitleForUpdateEditor(switchName, 'Switch');
        //    }
        //    else {
        //        $scope.title = UtilsService.buildTitleForAddEditor('Switch');
        //    }
        //}
        //function loadStaticData() {
        //    if (switchEntity == undefined)
        //        return;
        //    $scope.scopeModel.name = switchEntity.Name;

        //    if (switchEntity.Settings == null)
        //        return;
        //    $scope.scopeModel.description = switchEntity.Settings.Description;
        //    $scope.scopeModel.location = switchEntity.Settings.Location;
        //}
        //function loadSettingsDirective() {
        //    var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        //    settingsDirectiveReadyDeferred.promise.then(function () {
        //        var settingsDirectivePayload;
        //        if (switchEntity != undefined) {
        //            settingsDirectivePayload = { settings: switchEntity.Settings };
        //        }
        //        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
        //    });

        //    return settingsDirectiveLoadDeferred.promise;
        //}

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_StatusDefinitionAPIService.AddStatusDefinition(buildSwitchObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('StatusDefinition', response, 'Name')) {
                    if ($scope.onStatusDefinitionAdded != undefined)
                        $scope.onStatusDefinitionAdded(response.InsertedObject);
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
            return Retail_BE_StatusDefinitionAPIService.UpdateStatusDefinition(buildSwitchObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('StatusDefinition', response, 'Name')) {
                    if ($scope.onStatusDefinitionUpdated != undefined) {
                        $scope.onStatusDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildSwitchObjFromScope() {
            //var settings = settingsDirectiveAPI.getData();
            //settings.Description = $scope.scopeModel.description;
            //settings.Location = $scope.scopeModel.location;

            return {
                StatusDefinitionId: statusDefinitionEntity != undefined ? statusDefinitionEntity.StatusDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                //Settings: settings
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionEditorController', StatusDefinitionEditorController);

})(appControllers);