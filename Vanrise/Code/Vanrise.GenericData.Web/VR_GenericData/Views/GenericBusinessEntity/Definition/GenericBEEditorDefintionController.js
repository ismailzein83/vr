(function (appControllers) {

    "use strict";

    GenericBEEditorDefintionController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VR_Sec_ViewTypeEnum', 'InsertOperationResultEnum'];

    function GenericBEEditorDefintionController($scope, VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VR_Sec_ViewTypeEnum, InsertOperationResultEnum) {

        var isEditMode;
        var viewTypeName = "VR_GenericData_GenericBusinessEntity";
        var businessEntityDefinitionEntity;
        var businessEntityDefinitionId;

        var settingDirectiveAPI;
        var settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var menuItems;
        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var viewEntity;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            isEditMode = (businessEntityDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.onSettingDirectiveReady = function (api) {
                settingDirectiveAPI = api;
                settingReadyPromiseDeferred.resolve();
            };

           
            $scope.scopeModal.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModal.hasSaveGenericBEEditor = function () {
                if (isEditMode) {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
                }
                else {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
                }
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.scopeModal.validateMenuLocation = function () {
                return ($scope.scopeModal.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getEntities().then(function () {
                        loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadSettingDirectiveSection, setTitle, loadTree]).then(function () {
                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && businessEntityDefinitionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(businessEntityDefinitionEntity.Name, 'Generic BE Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Generic BE Editor');
                }
                function loadStaticData()
                {
                    if(businessEntityDefinitionEntity != undefined )
                    {
                        $scope.scopeModal.businessEntityName = businessEntityDefinitionEntity.Name;
                        $scope.scopeModal.businessEntityTitle = businessEntityDefinitionEntity.Title;
                    }
                }
                function loadSettingDirectiveSection() {
                    var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    settingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined) ?
                            {
                                businessEntityDefinitionSettings: businessEntityDefinitionEntity.Settings,
                            } : undefined;
                            VRUIUtilsService.callDirectiveLoad(settingDirectiveAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                        });

                    return loadSettingDirectivePromiseDeferred.promise;
                   
                }

                function loadTree() {
                    var treeLoadDeferred = UtilsService.createPromiseDeferred();

                    loadMenuItems().then(function () {
                        treeReadyDeferred.promise.then(function () {
                            if (viewEntity != undefined) {
                               
                                $scope.scopeModal.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                            }
                            treeAPI.refreshTree(menuItems);
                            treeLoadDeferred.resolve();
                        });
                    }).catch(function (error) {
                        treeLoadDeferred.reject(error);
                    });

                    return treeLoadDeferred.promise;

                    function loadMenuItems() {
                        return VR_Sec_MenuAPIService.GetAllMenuItems(false,true).then(function (response) {
                            if (response) {
                                menuItems = [];
                                for (var i = 0; i < response.length; i++) {
                                    menuItems.push(response[i]);
                                }
                            }
                        });
                    }
                }
            }

            function getEntities() {
                return UtilsService.waitMultipleAsyncOperations([getBusinessEntityDefinition, getGenericBusinessEntityDefinitionView]);

                function getBusinessEntityDefinition() {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function (businessEntityDefinition) {
                        businessEntityDefinitionEntity = businessEntityDefinition;
                    });
                }
                function getGenericBusinessEntityDefinitionView() {
                    VR_GenericData_BusinessEntityDefinitionAPIService.GetGenericBEDefinitionView(businessEntityDefinitionId).then(function (response) {
                        viewEntity = response;
                    });
                }
            }
            
        }
        function buildGenericBEDefinitionFromScope() {
            var bEdefinition = {
                BusinessEntityDefinitionId :businessEntityDefinitionId,
                Name: $scope.scopeModal.businessEntityName,
                Title: $scope.scopeModal.businessEntityTitle,
                Settings: settingDirectiveAPI.getData()
            };
            return bEdefinition;
        }

        function buildViewObjectFromScope(businessEntityDefinitionId) {
            return {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModal.businessEntityName,
                Title: $scope.scopeModal.businessEntityTitle,
                ModuleId: $scope.scopeModal.selectedMenuItem.Id,
                Settings: {
                    $type: 'Vanrise.GenericData.Entities.GenericBEViewSettings, Vanrise.GenericData.Entities',
                    BusinessEntityDefinitionId: businessEntityDefinitionId
                },
                Type: VR_Sec_ViewTypeEnum.System.value
            };
        }


        function insert() {
            $scope.scopeModal.isLoading = true;
            var serverResponse;
            var genericBEDefinition = buildGenericBEDefinitionFromScope();
            var promises = [];
            var insertEntityDeferred = UtilsService.createPromiseDeferred();
            promises.push(insertEntityDeferred.promise);

            var insertViewDeferred = UtilsService.createPromiseDeferred();
            promises.push(insertViewDeferred.promise);


            insertGenericBEDefinition().then(function () {
                if (serverResponse.Result == InsertOperationResultEnum.Succeeded.value) {
                    insertEntityDeferred.resolve();
                    insertView().then(function () { insertViewDeferred.resolve(); }).catch(function (error) { insertViewDeferred.reject(error); });
                } else if (serverResponse.Result == InsertOperationResultEnum.SameExists.value) {
                    insertEntityDeferred.resolve();
                    insertViewDeferred.resolve();
                }
                else {
                    insertEntityDeferred.reject();
                }
            });


            return UtilsService.waitMultiplePromises(promises).then(function () {
                if (VRNotificationService.notifyOnItemAdded('Business Entity Definition', serverResponse, 'Name')) {
                    if ($scope.onGenericBEDefinitionAdded != undefined) {
                        $scope.onGenericBEDefinitionAdded(serverResponse.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
            function insertGenericBEDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.AddBusinessEntityDefinition(genericBEDefinition).then(function (response) {
                    serverResponse = response;
                });
            }
            function insertView() {
                return VR_Sec_ViewAPIService.AddView(buildViewObjectFromScope(serverResponse.InsertedObject.Entity.BusinessEntityDefinitionId));
            }

        }

        function update() {
            $scope.scopeModal.isLoading = true;
            var genericBEDefinitionResponse;
            var genericBEDefinition = buildGenericBEDefinitionFromScope();
            return UtilsService.waitMultipleAsyncOperations([updateGenericBEDefinition, updateView]).then(function () {
                if (VRNotificationService.notifyOnItemUpdated('Business Entity Definition', genericBEDefinitionResponse, 'Name')) {
                    if ($scope.onBusinessEntityDefinitionUpdated != undefined) {
                        $scope.onBusinessEntityDefinitionUpdated(genericBEDefinitionResponse.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function updateGenericBEDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.UpdateBusinessEntityDefinition(genericBEDefinition).then(function (response) {
                    genericBEDefinitionResponse = response;
                });
            }
            function updateView() {
                
                if (viewEntity != null)
                    return VR_Sec_ViewAPIService.UpdateView(buildViewObjectFromScope(businessEntityDefinitionId));
                else
                {
                    var view = buildViewObjectFromScope(businessEntityDefinitionId);
                    view.ViewTypeName = viewTypeName;
                    return VR_Sec_ViewAPIService.AddView(view);
                }
                 
            }
        }

    }

    appControllers.controller('VR_GenericData_GenericBEEditorDefintionController', GenericBEEditorDefintionController);
})(appControllers);
