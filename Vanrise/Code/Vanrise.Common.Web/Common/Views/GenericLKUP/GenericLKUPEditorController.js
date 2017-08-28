(function (appControllers) {

    "use strict";

    GenericLKUPEditorController.$inject = ['$scope', 'UtilsService', 'VR_Common_GnericLKUPAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericLKUPEditorController($scope, UtilsService, VR_Common_GnericLKUPAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var context;
        var isViewHistoryMode;
        var genericLKUPItemId;
        var genericLKUPItemEntity;

        var genericLKUPDefinitionSelectorApi;
        var genericLKUPDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        var selectedBeDefinitionSelectorPromiseDeferred;

        var genericLKUPDefinitionExtendedSetingsEntity;

        var extendedSettingDirectiveAPI;
        var extendedSettingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                genericLKUPItemId = parameters.genericLKUPItemId;
                context = parameters.context;
            }
            isEditMode = (genericLKUPItemId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);

        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericLKUPDefinitionSelectorReady = function (api) {
                genericLKUPDefinitionSelectorApi = api;
                genericLKUPDefinitionSelectorPromiseDeferred.resolve();
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
            $scope.scopeModel.onRuntimeEditorReady = function (api) {
                extendedSettingDirectiveAPI = api;
                extendedSettingReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.loadExtendedSettingsRuntimeEditor = function (value) {
               
                if (selectedBeDefinitionSelectorPromiseDeferred != undefined)
                    selectedBeDefinitionSelectorPromiseDeferred.resolve();
                else
                {
                    if (value != undefined)
                    {
                        var businessEntityDefinitionId = genericLKUPDefinitionSelectorApi.getSelectedIds();
                        if (businessEntityDefinitionId != undefined) {
                            getGenericLKUPDefinitionExtendedSetings(businessEntityDefinitionId).then(function () {
                                extendedSettingReadyPromiseDeferred.promise.then(function () {
                                    var setLoader = function (value) {
                                        $scope.scopeModel.isLoadingDirective = value;
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, extendedSettingDirectiveAPI, undefined, setLoader, selectedBeDefinitionSelectorPromiseDeferred);
                                });
                            });
                        }
                    }
                   
                }
               
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getGenericLKUPItem().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getGenericLKUPItemHistory().then(function () {
                    loadAllControls().finally(function () {
                        cityEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }

            else {
                loadAllControls();
            }
        }

        function getGenericLKUPItem() {
            return VR_Common_GnericLKUPAPIService.GetGenericLKUPItem(genericLKUPItemId).then(function (response) {
                genericLKUPItemEntity = response;
            });
        }
        function getGenericLKUPItemHistory() {            
            return VR_Common_GnericLKUPAPIService.GetGenericLKUPItemHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                genericLKUPItemEntity = response;
            });
        }
        function getGenericLKUPDefinitionExtendedSetings(businessEntityDefinitionId) {
            return VR_Common_GnericLKUPAPIService.GetGenericLKUPDefinitionExtendedSetings(businessEntityDefinitionId).then(function (response) {
                if (response)
                {
                    $scope.scopeModel.RuntimeEditor = response.RuntimeEditor;
                }

            });
        }
        function loadAllControls() {
            function setTitle() {
                if (isEditMode) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(genericLKUPItemEntity.Name, 'Generic LKUP');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Generic LKUP');
                }
            }
            function loadStaticData() {
                if (genericLKUPItemEntity == undefined)
                    return;
                $scope.scopeModel.name = genericLKUPItemEntity.Name;
            }
            function loadBusinessEntityDefinitionSelector() {
                if(genericLKUPItemEntity != undefined)
                    selectedBeDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                var genericLKUPDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                genericLKUPDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        selectedIds: genericLKUPItemEntity != undefined ? genericLKUPItemEntity.BusinessEntityDefinitionId : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(genericLKUPDefinitionSelectorApi, payloadSelector, genericLKUPDefinitionSelectorLoadDeferred);
                });
                return genericLKUPDefinitionSelectorLoadDeferred.promise;
            }

            function loadBusinessEntityExtendedSettings()
            {
                if (genericLKUPItemEntity != undefined)
                {
                    var promises = [];
                    promises.push(getGenericLKUPDefinitionExtendedSetings(genericLKUPItemEntity.BusinessEntityDefinitionId));

                    var loadExtendedSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([extendedSettingReadyPromiseDeferred.promise, selectedBeDefinitionSelectorPromiseDeferred.promise])
                            .then(function () {
                                selectedBeDefinitionSelectorPromiseDeferred = undefined;
                                var extendedSettingsPayload = genericLKUPItemEntity != undefined && genericLKUPItemEntity.Settings != undefined ? genericLKUPItemEntity.Settings.ExtendedSettings : undefined;
                                VRUIUtilsService.callDirectiveLoad(extendedSettingDirectiveAPI, extendedSettingsPayload, loadExtendedSettingDirectivePromiseDeferred);
                            });

                    promises.push(loadExtendedSettingDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }
               
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadBusinessEntityDefinitionSelector, loadBusinessEntityExtendedSettings]).then(function () { }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
       
        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Common_GnericLKUPAPIService.AddGenericLKUPItem(buildAlertLevelObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Genric LKUP', response, 'Name')) {
                    if ($scope.onGenericLKUPAdded != undefined)
                        $scope.onGenericLKUPAdded(response.InsertedObject);
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
            return VR_Common_GnericLKUPAPIService.UpdateGenericLKUPItem(buildAlertLevelObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Generic LKUP', response, 'Name')) {

                    if ($scope.onGenericLKUPUpdated != undefined) {
                        $scope.onGenericLKUPUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAlertLevelObjFromScope() {
            var settings = {
                ExtendedSettings: extendedSettingDirectiveAPI.getData()
            };
            return {
                GenericLKUPItemId: genericLKUPItemEntity != undefined ? genericLKUPItemEntity.GenericLKUPItemId : undefined,
                Name: $scope.scopeModel.name,
                BusinessEntityDefinitionId: genericLKUPDefinitionSelectorApi.getSelectedIds(),
                Settings: settings,
            };
        }
    }

    appControllers.controller('VR_Common_GenericLKUPEditorController', GenericLKUPEditorController);

})(appControllers);