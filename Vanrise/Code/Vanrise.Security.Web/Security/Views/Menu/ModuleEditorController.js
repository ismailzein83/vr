(function (appControllers) {

    'use strict';

    ModuleEditorController.$inject = ['$scope', 'VR_Sec_ModuleAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_MenuAPIService', 'VRUIUtilsService'];

    function ModuleEditorController($scope, VR_Sec_ModuleAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_MenuAPIService, VRUIUtilsService) {

        var viewSelectorAPI;
        var viewSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var isEditMode;
        var moduleId;
        var moduleEntity;
        var parentId;

        var nameResourceKeySelectorAPI;
        var nameResourceKeySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                parentId = parameters.parentId;
                moduleId = parameters.moduleId;
            }
            isEditMode = (moduleId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return updateModule();
                else
                    return insertModule();
            };

            if (parentId == null) {
                $scope.scopeModel.rootModule = true;
                $scope.scopeModel.renderedAsView = false;
            }

            $scope.hasSaveModulePermission = function () {
                if (isEditMode)
                    return VR_Sec_ModuleAPIService.HasUpdateModulePermission();
                else
                    return VR_Sec_ModuleAPIService.HasAddModulePermission();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onViewSelectorReady = function (api) {
                viewSelectorAPI = api;
                viewSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onNameResourceKeySelectorReady = function (api) {
                nameResourceKeySelectorAPI = api;
                nameResourceKeySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getModule().then(function () {
                    loadAllControls().finally(function () {
                        //  moduleEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getModule() {
            return VR_Sec_ModuleAPIService.GetModule(moduleId).then(function (response) {
                moduleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadViewSelector, loadNameResourceKeySelector, loadDevProjectSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (moduleEntity != undefined) {
                    payloadDirective = {
                        selectedIds: moduleEntity.DevProjectId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, payloadDirective, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }

        function loadNameResourceKeySelector() {
            var resourceKeySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            nameResourceKeySelectorReadyDeferred.promise.then(function () {
                var payload = {};
                if (moduleEntity != undefined && moduleEntity.Settings != undefined) {
                    payload.selectedResourceKey = moduleEntity.Settings.LocalizedName;
                }
                VRUIUtilsService.callDirectiveLoad(nameResourceKeySelectorAPI, payload, resourceKeySelectorLoadDeferred);
            });
            return resourceKeySelectorLoadDeferred.promise;
        }

        function setTitle() {
            if (isEditMode && moduleEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(moduleEntity.Name, 'Module');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Module');
        }

        function loadStaticData() {

            if (moduleEntity == undefined)
                return;

            $scope.scopeModel.name = moduleEntity.Name;
            if (moduleEntity.ParentId == null) {
                $scope.scopeModel.rootModule = true;
                $scope.scopeModel.renderedAsView = false;
            }
            else {
                $scope.scopeModel.rootModule = false;
                $scope.scopeModel.renderedAsView = moduleEntity.RenderedAsView;              
            }


        }
        function loadViewSelector() {
            var loadViewSelectorPromise = UtilsService.createPromiseDeferred();
            viewSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    selectedIds: moduleEntity != undefined ? moduleEntity.DefaultViewId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(viewSelectorAPI, payload, loadViewSelectorPromise);
            });
            return loadViewSelectorPromise.promise;

        }
        function buildModuleObjFromScope() {
            var parentId = moduleEntity != undefined ? moduleEntity.ParentId : parentId ;
            var moduleObject = {
                ModuleId: moduleId,
                Name: $scope.scopeModel.name,
                AllowDynamic: moduleEntity && moduleEntity.isDynamic || false,
                DefaultViewId: viewSelectorAPI.getSelectedIds(),
                DevProjectId: devProjectDirectiveApi.getSelectedIds(),
                ParentId: parentId,
                Settings: {
                    LocalizedName: nameResourceKeySelectorAPI.getResourceKey()
                },
                RenderedAsView: parentId != null ? $scope.scopeModel.renderedAsView : false
            };
            return moduleObject;
        }

        function insertModule() {
            $scope.scopeModel.isLoading = true;

            var moduleObject = buildModuleObjFromScope();

            return VR_Sec_ModuleAPIService.AddModule(moduleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Module', response, 'Name')) {
                    if ($scope.onModuleAdded != undefined)
                        $scope.onModuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateModule() {
            $scope.scopeModel.isLoading = true;

            var moduleObject = buildModuleObjFromScope();

            return VR_Sec_ModuleAPIService.UpdateModule(moduleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Module', response, 'Name')) {
                    if ($scope.onModuleUpdated != undefined)
                        $scope.onModuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


    }

    appControllers.controller('VR_Sec_ModuleEditorController', ModuleEditorController);

})(appControllers);
