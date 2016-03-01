(function (appControllers) {

    "use strict";

    GenericEditorController.$inject = ['$scope', 'VR_GenericData_GenericEditorAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function GenericEditorController($scope, VR_GenericData_GenericEditorAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

        var isEditMode;
        var genericEditorEntity;
        var genericEditorDefinitionId;
        var recordTypeEntity;
        var businessEntityDefinitionId;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var genericEditorDesignAPI;
        var genericEditorDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var editorDirectiveAPI;


        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                genericEditorDefinitionId = parameters.genericEditorDefinitionId;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            isEditMode = (genericEditorDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {}
            $scope.scopeModal.isRecordTypeDisbled = (isEditMode ==true);
            $scope.scopeModal.onRecordTypeSelectionChanged = function () {
                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedDataRecordTypeId != undefined)
                {
                    getDataRecordType(selectedDataRecordTypeId).then(function () {
                        var payload = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericEditorDesignAPI, payload, setLoader, genericEditorDesignReadyPromiseDeferred);
                    });
                }
            }

            $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.SaveGenericEditor = function () {
                if (isEditMode) {
                    return updateGenericEditor();
                }
                else {
                    return insertGenericEditor();
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
                return UtilsService.waitMultipleAsyncOperations([loadEditorDesignSection, setTitle, loadDataRecordTypeSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && genericEditorEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(genericEditorEntity.Name, 'Generic Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Generic Editor');
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
                return VR_GenericData_GenericEditorAPIService.GetGenericEditorDefinition(genericEditorDefinitionId).then(function (genericEditor) {
                    genericEditorEntity = genericEditor;
                });
            }
        }


        function getDataRecordType(dataRecordTypeId)
        {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
               recordTypeEntity = response;

            });
        }

        function buildGenericEditorObjFromScope() {
            if (genericEditorDesignAPI != undefined)
            {
                var genericEditor={
                    BusinessEntityId: genericEditorEntity != undefined ? genericEditorEntity.BusinessEntityId : businessEntityDefinitionId,
                    Details: genericEditorDesignAPI.getData(),
                    GenericEditorDefinitionId : genericEditorDefinitionId
                }
                if (genericEditor.Details != undefined)
                {
                    genericEditor.Details.DataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                }
                return genericEditor;
            }
            
           
        }

        function insertGenericEditor() {

            var genericEditorObject = buildGenericEditorObjFromScope();
            return VR_GenericData_GenericEditorAPIService.AddGenericEditor(genericEditorObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Generic Editor", response)) {
                    if ($scope.onGenericEditorAdded != undefined)
                        $scope.onGenericEditorAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateGenericEditor() {
            var genericEditorObject = buildGenericEditorObjFromScope();
            VR_GenericData_GenericEditorAPIService.UpdateGenericEditor(genericEditorObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Generic Editor", response)) {
                    if ($scope.onGenericEditorUpdated != undefined)
                        $scope.onGenericEditorUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('VR_GenericData_GenericEditorController', GenericEditorController);
})(appControllers);
