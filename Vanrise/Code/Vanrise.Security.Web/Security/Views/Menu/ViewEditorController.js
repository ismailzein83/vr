(function (appControllers) {

    'use strict';

    ViewEditorController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

    function ViewEditorController($scope, VR_Sec_ViewAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        $scope.scopeModal = {};
        $scope.scopeModal.isEditMode;
        var viewId;
        var viewEntity;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();

        
        var viewCommonPropertiesAPI;
        var viewCommonPropertiesReadyDeferred = UtilsService.createPromiseDeferred();
        
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                viewId = parameters.viewId;

            $scope.scopeModal.isEditMode = (viewId != undefined);
        }

        function defineScope() {
            $scope.scopeModal.save = function () {
                if ($scope.scopeModal.isEditMode)
                    return updateView();
            };

            $scope.scopeModal.onViewCommonPropertiesReady = function (api) {
                viewCommonPropertiesAPI = api;
                viewCommonPropertiesReadyDeferred.resolve();
            };

            $scope.hasSaveViewPermission = function () {
                return VR_Sec_ViewAPIService.HasUpdateViewPermission();
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
            
            $scope.scopeModal.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if ($scope.scopeModal.isEditMode) {
                getView().then(function () {
                    loadAllControls().finally(function () {
                    //    viewEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                    viewEntity = response;
                });
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,  loadViewCommonProperties, loadDevProjectSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        

        function setTitle() {
            if ($scope.scopeModal.isEditMode && viewEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'View');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('View');
        }

        function loadViewCommonProperties() {
            var viewCommmonPropertiesLoadDeferred = UtilsService.createPromiseDeferred();
            viewCommonPropertiesReadyDeferred.promise.then(function () {
                var payload = {};
                if (viewEntity != undefined) {
                    payload.viewEntity = viewEntity;
                }
                VRUIUtilsService.callDirectiveLoad(viewCommonPropertiesAPI, payload, viewCommmonPropertiesLoadDeferred);
            });
            return viewCommmonPropertiesLoadDeferred.promise;
        }

        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (viewEntity != undefined) {
                    payloadDirective = {
                        selectedIds: viewEntity.DevProjectId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, payloadDirective, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function loadStaticData() {

            if (viewEntity == undefined)
                return;

            $scope.scopeModal.name = viewEntity.Name;
            $scope.scopeModal.titleValue = viewEntity.Title;
        }

        function buildViewObjFromScope() {
            var viewSettings = {};
            if (viewEntity != undefined && viewEntity.Settings != undefined)
                viewSettings = viewEntity.Settings;
            
             viewCommonPropertiesAPI.setCommonProperties(viewSettings);
            var viewObject = {
                ViewId: viewId,
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.titleValue,
                Url: viewEntity.Url,
                ModuleId: viewEntity.ModuleId,
                ActionNames: viewEntity.ActionNames,
                Type: viewEntity.Type,
                ViewContent: viewEntity.ViewContent,
                Rank: viewEntity.Rank,
                DevProjectId: devProjectDirectiveApi.getSelectedIds(),
                Settings: viewSettings,
            };
            return viewObject;
        }

        function updateView() {
            $scope.scopeModal.isLoading = true;

            var viewObject = buildViewObjFromScope();

            return VR_Sec_ViewAPIService.UpdateView(viewObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('View', response, 'Name')) {
                    if ($scope.onViewUpdated != undefined)
                        $scope.onViewUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_ViewEditorController', ViewEditorController);

})(appControllers);
