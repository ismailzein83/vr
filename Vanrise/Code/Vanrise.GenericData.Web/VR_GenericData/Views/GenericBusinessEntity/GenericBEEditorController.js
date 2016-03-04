(function (appControllers) {

    "use strict";

    GenericBEEditorController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function GenericBEEditorController($scope, VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

        var isEditMode;
        var genericEditorEntity;
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
                    return updateGenericBEDefinition();
                }
                else {
                    return insertGenericBEDefinition();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onGenericEditorDirectiveReady = function (api) {
                genericEditorDesignAPI = api;
                genericEditorDesignReadyPromiseDeferred.resolve();
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getGenericEditor().then(function () {
                    getDataRecordType(genericEditorEntity.Details.DataRecordTypeId).then(function () {
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
                return UtilsService.waitMultipleAsyncOperations([loadEditorDesignSection, setTitle, loadDataRecordTypeSelector, loadGridDesignSection, loadFilterDesignSection]).then(function () {

                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && genericEditorEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(genericEditorEntity.Name, 'Generic BE Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Generic BE Editor');
                }

                function loadEditorDesignSection() {
                    var loadGenericEditorDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    genericEditorDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (genericEditorEntity != undefined && genericEditorEntity.Details != undefined && recordTypeEntity != undefined) ? { sections: genericEditorEntity.Details.Sections, recordTypeFields: recordTypeEntity.Fields } : undefined
                            genericEditorDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(genericEditorDesignAPI, directivePayload, loadGenericEditorDesignPromiseDeferred);
                        });

                    return loadGenericEditorDesignPromiseDeferred.promise;

                }
                function loadGridDesignSection() {
                    var loadGridDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    gridFieldsDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (genericEditorEntity != undefined && genericEditorEntity.Details != undefined && recordTypeEntity != undefined) ? { sections: genericEditorEntity.Details.Sections, recordTypeFields: recordTypeEntity.Fields } : undefined
                            gridFieldsDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(gridFieldsDirectiveAPI, directivePayload, loadGridDesignPromiseDeferred);
                        });

                    return loadGridDesignPromiseDeferred.promise;

                }
                function loadFilterDesignSection() {
                    var loadFilterDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    filterFieldsDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (genericEditorEntity != undefined && genericEditorEntity.Details != undefined && recordTypeEntity != undefined) ? { sections: genericEditorEntity.Details.Sections, recordTypeFields: recordTypeEntity.Fields } : undefined
                            filterFieldsDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(filterFieldsDirectiveAPI, directivePayload, loadFilterDesignPromiseDeferred);
                        });

                    return loadFilterDesignPromiseDeferred.promise;

                }
                function loadDataRecordTypeSelector() {
                    var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (genericEditorEntity != undefined && genericEditorEntity.Details != undefined) ? { selectedIds: genericEditorEntity.Details.DataRecordTypeId } : undefined

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                    return loadDataRecordTypeSelectorPromiseDeferred.promise;
                }

            }

            function getGenericEditor() {
                return VR_GenericData_GenericEditorAPIService.GetGenericEditorDefinition(businessEntityDefinitionId).then(function (genericEditor) {
                    genericEditorEntity = genericEditor;
                });
            }
        }


        function getDataRecordType(dataRecordTypeId) {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                recordTypeEntity = response;

            });
        }

        function buildGenericBEDefinitionFromScope() {
            var genericBEDefinitionSettings = {
                $type:"Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                DataRecordTypeId : dataRecordTypeSelectorAPI.getSelectedIds(),
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
            console.log(bEdefinition)
            return bEdefinition;
        }

        function insertGenericBEDefinition() {

            var genericBEDefinition = buildGenericBEDefinitionFromScope();
            return VR_GenericData_BusinessEntityDefinitionAPIService.AddBusinessEntityDefinition(genericBEDefinition)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Business Entity Definition", response)) {
                    if ($scope.onGenericBEDefinitionAdded != undefined)
                        $scope.onGenericBEDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateGenericBEDefinition() {
            var genericBEDefinition = buildGenericBEDefinitionFromScope();
            VR_GenericData_BusinessEntityDefinitionAPIService.UpdateBusinessEntityDefinition(genericBEDefinition)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Business Entity Definition", response)) {
                    if ($scope.onGenericBEDefinitionUpdated != undefined)
                        $scope.onGenericBEDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('VR_GenericData_GenericBEEditorController', GenericBEEditorController);
})(appControllers);
