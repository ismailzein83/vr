(function (appControllers) {

    "use strict";

    GenericExtensibleBEEditorController.$inject = ['$scope', 'VR_GenericData_ExtensibleBEItemAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function GenericExtensibleBEEditorController($scope, VR_GenericData_ExtensibleBEItemAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

        var isEditMode;
        var extensibleBEItemEntity;
        var extensibleBEItemId;
        var recordTypeEntity;
        var businessEntityDefinitionId;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var extensibleBEItemDesignAPI;
        var extensibleBEItemDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var editorDirectiveAPI;


        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                extensibleBEItemId = parameters.extensibleBEItemId;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            isEditMode = (extensibleBEItemId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {}
            $scope.scopeModal.isRecordTypeDisbled = (isEditMode ==true);
            $scope.scopeModal.onRecordTypeSelectionChanged = function () {
                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedDataRecordTypeId != undefined) {
                    getDataRecordType(selectedDataRecordTypeId).then(function () {
                        var payload = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, extensibleBEItemDesignAPI, payload, setLoader, extensibleBEItemDesignReadyPromiseDeferred);
                    });
                }
            };

            $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModal.SaveExtensibleBEItem = function () {
                if (isEditMode) {
                    return updateExtensibleBEItem();
                }
                else {
                    return insertExtensibleBEItem();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModal.onExtensibleBEItemDirectiveReady = function (api) {
                extensibleBEItemDesignAPI = api;
                extensibleBEItemDesignReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getExtensibleBEItem().then(function () {
                    getDataRecordType(extensibleBEItemEntity.DataRecordTypeId).then(function () {
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
                    if (isEditMode && extensibleBEItemEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(extensibleBEItemEntity.Name, 'Extensible BE Item');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Extensible BE Item');
                }

                function loadEditorDesignSection() {
                    var loadExtensibleBEItemDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                    extensibleBEItemDesignReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (extensibleBEItemEntity != undefined && extensibleBEItemEntity != undefined && recordTypeEntity != undefined) ? { sections: extensibleBEItemEntity.Sections, recordTypeFields: recordTypeEntity.Fields } : undefined
                            extensibleBEItemDesignReadyPromiseDeferred = undefined;
                            VRUIUtilsService.callDirectiveLoad(extensibleBEItemDesignAPI, directivePayload, loadExtensibleBEItemDesignPromiseDeferred);
                        });

                    return loadExtensibleBEItemDesignPromiseDeferred.promise;
                    
                }


                function loadDataRecordTypeSelector() {
                    var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = (extensibleBEItemEntity != undefined) ? { selectedIds: extensibleBEItemEntity.DataRecordTypeId } : undefined

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                    return loadDataRecordTypeSelectorPromiseDeferred.promise;
                }


            }

            function getExtensibleBEItem() {
                return VR_GenericData_ExtensibleBEItemAPIService.GetExtensibleBEItem(extensibleBEItemId).then(function (extensibleBEItem) {
                    extensibleBEItemEntity = extensibleBEItem;
                });
            }
        }


        function getDataRecordType(dataRecordTypeId)
        {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
               recordTypeEntity = response;

            });
        }

        function buildExtensibleBEItemFromScope() {
            if (extensibleBEItemDesignAPI != undefined)
            {
                var extensibleBEItem = extensibleBEItemDesignAPI.getData();
                if (extensibleBEItem == undefined)
                    extensibleBEItem = {};
                extensibleBEItem.BusinessEntityDefinitionId = extensibleBEItemEntity != undefined ? extensibleBEItemEntity.BusinessEntityDefinitionId : businessEntityDefinitionId;
                extensibleBEItem.ExtensibleBEItemId = extensibleBEItemId;
                extensibleBEItem.DataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                return extensibleBEItem;
            }
        }

        function insertExtensibleBEItem() {

            var extensibleBEItem = buildExtensibleBEItemFromScope();
            return VR_GenericData_ExtensibleBEItemAPIService.AddExtensibleBEItem(extensibleBEItem)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Extensible BE Item", response)) {
                    if ($scope.onExtensibleBEItemAdded != undefined)
                        $scope.onExtensibleBEItemAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateExtensibleBEItem() {
            var extensibleBEItem = buildExtensibleBEItemFromScope();
            VR_GenericData_ExtensibleBEItemAPIService.UpdateExtensibleBEItem(extensibleBEItem)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Extensible BE Item", response)) {
                    if ($scope.onExtensibleBEItemUpdated != undefined)
                        $scope.onExtensibleBEItemUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('VR_GenericData_GenericExtensibleBEEditorController', GenericExtensibleBEEditorController);
})(appControllers);
