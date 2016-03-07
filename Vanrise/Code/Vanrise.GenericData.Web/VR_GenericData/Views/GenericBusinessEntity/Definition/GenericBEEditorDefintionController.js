﻿(function (appControllers) {

    "use strict";

    GenericBEEditorDefintionController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VR_Sec_ViewTypeEnum', 'InsertOperationResultEnum'];

    function GenericBEEditorDefintionController($scope, VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VR_Sec_ViewTypeEnum, InsertOperationResultEnum) {

        var isEditMode;
        var businessEntityDefinitionEntity;
        var recordTypeEntity;
        var businessEntityDefinitionId;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var genericEditorDesignAPI;
        var genericEditorDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var gridFieldsDirectiveAPI;
        var gridFieldsDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filterFieldsDirectiveAPI;
        var filterFieldsDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();


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
            $scope.scopeModal = {}

            $scope.scopeModal.onGridFieldDesignReady = function(api)
            {
                gridFieldsDirectiveAPI = api;
                gridFieldsDesignReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onFilterFieldDesignReady = function (api) {
                filterFieldsDirectiveAPI = api;
                filterFieldsDesignReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.isRecordTypeDisbled = (isEditMode == true);

            $scope.scopeModal.onRecordTypeSelectionChanged = function () {

                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();

                if (selectedDataRecordTypeId != undefined) {
                    getDataRecordType(selectedDataRecordTypeId).then(function () {
                        var payload = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericEditorDesignAPI, payload, setLoader, genericEditorDesignReadyPromiseDeferred);

                        var payloadGrid = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoaderGrid = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridFieldsDirectiveAPI, payloadGrid, setLoaderGrid, gridFieldsDesignReadyPromiseDeferred);

                        var payloadFilter = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoaderFilter = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterFieldsDirectiveAPI, payloadFilter, setLoaderFilter, filterFieldsDesignReadyPromiseDeferred);
                    });
                }
            }

            $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onGenericEditorDirectiveReady = function (api) {
                genericEditorDesignAPI = api;
                genericEditorDesignReadyPromiseDeferred.resolve();
            }

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
                    getDataRecordType(businessEntityDefinitionEntity.Settings.DataRecordTypeId).then(function () {
                        loadAllControls();
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModal.isLoading = false;
                    });;


                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadEditorDesignSection, setTitle, loadDataRecordTypeSelector, loadGridDesignSection, loadFilterDesignSection, loadTree]).then(function () {

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
                        console.log(businessEntityDefinitionEntity);
                        $scope.scopeModal.businessEntityName = businessEntityDefinitionEntity.Name;
                        $scope.scopeModal.businessEntityTitle = businessEntityDefinitionEntity.Title;
                        if(businessEntityDefinitionEntity.Settings != undefined)
                            $scope.scopeModal.selectedTitleFieldPath = UtilsService.getItemByVal($scope.scopeModal.fields, businessEntityDefinitionEntity.Settings.FieldPath, "Name");
                    }
                }
                function loadEditorDesignSection() {
                    var loadGenericEditorDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    genericEditorDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined && recordTypeEntity != undefined) ? { sections: businessEntityDefinitionEntity.Settings.EditorDesign.Sections, recordTypeFields: recordTypeEntity.Fields } : undefined
                            genericEditorDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(genericEditorDesignAPI, directivePayload, loadGenericEditorDesignPromiseDeferred);
                        });

                    return loadGenericEditorDesignPromiseDeferred.promise;

                }
                function loadGridDesignSection() {
                    var loadGridDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    gridFieldsDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings.ManagementDesign != undefined && recordTypeEntity != undefined) ? { selectedColumns: businessEntityDefinitionEntity.Settings.ManagementDesign.GridDesign.Columns, recordTypeFields: recordTypeEntity.Fields } : undefined
                            gridFieldsDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(gridFieldsDirectiveAPI, directivePayload, loadGridDesignPromiseDeferred);
                        });

                    return loadGridDesignPromiseDeferred.promise;

                }
                function loadFilterDesignSection() {
                    var loadFilterDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    filterFieldsDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined && recordTypeEntity != undefined) ? { selectedFields: businessEntityDefinitionEntity.Settings.ManagementDesign.FilterDesign.Fields, recordTypeFields: recordTypeEntity.Fields } : undefined
                            filterFieldsDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(filterFieldsDirectiveAPI, directivePayload, loadFilterDesignPromiseDeferred);
                        });

                    return loadFilterDesignPromiseDeferred.promise;

                }
                function loadDataRecordTypeSelector() {
                    var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined) ? { selectedIds: businessEntityDefinitionEntity.Settings.DataRecordTypeId } : undefined

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                    return loadDataRecordTypeSelectorPromiseDeferred.promise;
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


        function getDataRecordType(dataRecordTypeId) {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                recordTypeEntity = response;
                $scope.scopeModal.fields = response.Fields;
            });
        }

        function buildGenericBEDefinitionFromScope() {
            var genericBEDefinitionSettings = {
                $type:"Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                FieldPath: $scope.scopeModal.selectedTitleFieldPath.Name,
                EditorDesign: genericEditorDesignAPI.getData(),
                ManagementDesign: {
                    GridDesign:gridFieldsDirectiveAPI.getData(),
                    FilterDesign:filterFieldsDirectiveAPI.getData()
                }
            };
            var bEdefinition = {
                BusinessEntityDefinitionId :businessEntityDefinitionId,
                Name: $scope.scopeModal.businessEntityName,
                Title: $scope.scopeModal.businessEntityTitle,
                Settings:genericBEDefinitionSettings
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
                    return VR_Sec_ViewAPIService.AddView(buildViewObjectFromScope(businessEntityDefinitionId));
            }
        }

    }

    appControllers.controller('VR_GenericData_GenericBEEditorDefintionController', GenericBEEditorDefintionController);
})(appControllers);
