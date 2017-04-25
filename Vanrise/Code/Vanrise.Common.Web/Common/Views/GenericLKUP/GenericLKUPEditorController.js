(function (appControllers) {

    "use strict";

    GenericLKUPEditorController.$inject = ['$scope', 'UtilsService', 'VR_Common_GnericLKUPAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericLKUPEditorController($scope, UtilsService, VR_Common_GnericLKUPAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var genericLKUPItemId;
        var genericLKUPItemEntity;

        var beDefinitionSelectorApi;
        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
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
            }
            isEditMode = (genericLKUPItemId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
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
            $scope.scopeModel.loadExtendedSettingsRuntimeEditor = function (v) {
               
                if (selectedBeDefinitionSelectorPromiseDeferred != undefined)
                    selectedBeDefinitionSelectorPromiseDeferred.resolve();
                else
                {
                    var businessEntityDefinitionId = beDefinitionSelectorApi.getSelectedIds();
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
            else {
                loadAllControls();
            }
        }

        function getGenericLKUPItem() {
            return VR_Common_GnericLKUPAPIService.GetGenericLKUPItem(genericLKUPItemId).then(function (response) {
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
                    var GenericLKUPItemName = (genericLKUPItemEntity != undefined) ? genericLKUPItemEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(GenericLKUPItemName, 'Generic LKUP');
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
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        selectedIds: genericLKUPItemEntity != undefined ? genericLKUPItemEntity.BusinessEntityDefinitionId : undefined,
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Common.Business.GenericLKUPBEDefinitionFilter, Vanrise.Common.Business"
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
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
                                var extendedSettingsPayload = genericLKUPItemEntity != undefined && genericLKUPItemEntity.Setting != undefined ? genericLKUPItemEntity.Settings.ExtendedSettings : undefined;
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
                BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                Settings: settings,
            };
        }
    }

    appControllers.controller('VR_Common_GenericLKUPEditorController', GenericLKUPEditorController);

})(appControllers);