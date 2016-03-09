(function (appControllers) {

    'use strict';

    ModuleEditorController.$inject = ['$scope', 'VR_Sec_ModuleAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VR_Sec_MenuAPIService'];

    function ModuleEditorController($scope, VR_Sec_ModuleAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_MenuAPIService) {
        var isEditMode;
        var moduleId;
        var moduleEntity;
        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                moduleId = parameters.moduleId;

            isEditMode = (moduleId != undefined);
        }

        function defineScope() {
            $scope.save = function () {
                if (isEditMode)
                    return updateModule();
                else
                    return insertModule();
            };
            $scope.onModulesTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };
            $scope.nodes = [];
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getModule().then(function () {
                    loadAllControls().finally(function () {
                        moduleEntity = undefined;
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

        function getModule() {
            return VR_Sec_ModuleAPIService.GetModule(userId).then(function (response) {
                moduleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTree])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function loadTree() {
            var treeLoadDeferred = UtilsService.createPromiseDeferred();

            loadMenuItems().then(function () {
                treeReadyDeferred.promise.then(function () {
                    //if (viewEntity != undefined) {

                    //    $scope.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                    //}
                    treeAPI.refreshTree(menuItems);
                    treeLoadDeferred.resolve();
                });
            }).catch(function (error) {
                treeLoadDeferred.reject(error);
            });

            return treeLoadDeferred.promise;

            function loadMenuItems() {
                return VR_Sec_MenuAPIService.GetAllMenuItems(false).then(function (response) {
                    if (response) {
                        menuItems = [];
                        for (var i = 0; i < response.length; i++) {
                            menuItems.push(response[i]);
                        }
                    }
                });
            }
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

            $scope.name = moduleEntity.Name;
            $scope.titleValue = moduleEntity.Title;
            $scope.isDynamic = moduleEntity.AllowDynamic;
        }

        function buildModuleObjFromScope() {
            var moduleObject = {
                ModuleId: moduleId,
                Name: $scope.name,
                Title: $scope.titleValue,
                AllowDynamic: $scope.isDynamic,
                ParentId: $scope.selectedMenuItem.Id
            };
            return moduleObject;
        }

        function insertModule() {
            $scope.isLoading = true;

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
                $scope.isLoading = false;
            });

        }

        function updateModule() {
            $scope.isLoading = true;

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
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_ModuleEditorController', ModuleEditorController);

})(appControllers);
