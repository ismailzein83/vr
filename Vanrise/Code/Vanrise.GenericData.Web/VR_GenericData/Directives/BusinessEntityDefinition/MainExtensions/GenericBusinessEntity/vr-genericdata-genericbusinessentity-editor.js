"use strict";

app.directive("vrGenericdataGenericbusinessentityEditor", ["UtilsService", "VRNotificationService","VR_GenericData_DataRecordTypeAPIService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var genericBusinessEntityDefinitionEditor = new GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs);
                genericBusinessEntityDefinitionEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Templates/GenericBusinessEntityDefinitionEditor.html"

        };

        function GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var genericEditorDesignAPI;
            var genericEditorDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridFieldsDirectiveAPI;
            var gridFieldsDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var filterFieldsDirectiveAPI;
            var filterFieldsDesignReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var editPermissionAPI;
            var editPermissionReadyDeferred = UtilsService.createPromiseDeferred();


            var recordTypeSelectedPromiseDeferred;
            var recordTypeEntity;
            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.onGridFieldDesignReady = function (api) {
                    gridFieldsDirectiveAPI = api;
                    gridFieldsDesignReadyPromiseDeferred.resolve();
                };

                $scope.scopeModal.onFilterFieldDesignReady = function (api) {
                    filterFieldsDirectiveAPI = api;
                    filterFieldsDesignReadyPromiseDeferred.resolve();
                };
                $scope.scopeModal.onGenericEditorDirectiveReady = function (api) {
                    genericEditorDesignAPI = api;
                    genericEditorDesignReadyPromiseDeferred.resolve();
                };
                $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModal.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };
                $scope.scopeModal.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    addPermissionReadyDeferred.resolve();
                };
                $scope.scopeModal.onEditRequiredPermissionReady = function (api) {
                    editPermissionAPI = api;
                    editPermissionReadyDeferred.resolve();
                };
                $scope.scopeModal.onRecordTypeSelectionChanged = function () {

                    var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();

                    if (selectedDataRecordTypeId != undefined) {
                        if (recordTypeSelectedPromiseDeferred != undefined)
                            recordTypeSelectedPromiseDeferred.resolve();
                        else {
                            $scope.scopeModal.selectedTitleFieldPath = undefined;
                            getDataRecordType(selectedDataRecordTypeId).then(function () {

                                var payload = { recordTypeFields: recordTypeEntity.Fields };
                                var setLoader = function (value) { $scope.isLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericEditorDesignAPI, payload, setLoader);

                                var payloadGrid = { recordTypeFields: recordTypeEntity.Fields };
                                var setLoaderGrid = function (value) { $scope.isLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridFieldsDirectiveAPI, payloadGrid, setLoaderGrid);

                                var payloadFilter = { recordTypeFields: recordTypeEntity.Fields };
                                var setLoaderFilter = function (value) { $scope.isLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterFieldsDirectiveAPI, payloadFilter, setLoaderFilter);
                                recordTypeSelectedPromiseDeferred = undefined;
                            });
                        }
                    }
                };

                $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        FieldPath: $scope.scopeModal.selectedTitleFieldPath.Name,
                        EditorDesign: genericEditorDesignAPI.getData(),
                        ManagementDesign: {
                            GridDesign: gridFieldsDirectiveAPI.getData(),
                            FilterDesign: filterFieldsDirectiveAPI.getData()
                        },
                        Security: {
                            ViewRequiredPermission: viewPermissionAPI.getData(),
                            AddRequiredPermission: addPermissionAPI.getData(),
                            EditRequiredPermission: editPermissionAPI.getData()
                        }
                    };
                };

                api.load = function (payload) {
                    var businessEntityDefinitionSettings;
                    var promises = [];
                    if (payload != undefined)
                    {
                        businessEntityDefinitionSettings = payload.businessEntityDefinitionSettings;
                        if (businessEntityDefinitionSettings != undefined) {
                            $scope.scopeModal.isRecordTypeDisbled =true;

                            var promise = UtilsService.createPromiseDeferred();
                            promises.push(promise.promise);
                            getDataRecordType(businessEntityDefinitionSettings.DataRecordTypeId).then(function () {
                                $scope.scopeModal.selectedTitleFieldPath = UtilsService.getItemByVal($scope.scopeModal.fields, businessEntityDefinitionSettings.FieldPath, "Name");
                                recordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                UtilsService.waitMultipleAsyncOperations([loadEditorDesignSection, loadGridDesignSection, loadFilterDesignSection, loadViewRequiredPermission, loadAddRequiredPermission, loadEditRequiredPermission]).then(function () {
                                   
                                    promise.resolve();
                                }).catch(function (error) {
                                    promise.reject(error);
                                });
                            }).catch(function (error) {
                                promise.reject(error);
                            });
                        }
                      
                    }
                    promises.push(loadDataRecordTypeSelector());
                    function loadEditorDesignSection() {
                        if (businessEntityDefinitionSettings != undefined) {
                            var loadGenericEditorDesignPromiseDeferred = UtilsService.createPromiseDeferred();

                            UtilsService.waitMultiplePromises([genericEditorDesignReadyPromiseDeferred.promise, recordTypeSelectedPromiseDeferred.promise])

                                .then(function () {
                                    recordTypeSelectedPromiseDeferred = undefined;

                                    var directivePayload = (businessEntityDefinitionSettings != undefined && recordTypeEntity != undefined) ?
                                {
                                    sections: businessEntityDefinitionSettings.EditorDesign.Sections,
                                    recordTypeFields: recordTypeEntity.Fields
                                } : undefined;
                                    genericEditorDesignReadyPromiseDeferred = undefined;
                                    VRUIUtilsService.callDirectiveLoad(genericEditorDesignAPI, directivePayload, loadGenericEditorDesignPromiseDeferred);
                                });

                            return loadGenericEditorDesignPromiseDeferred.promise;
                        }
                    }
                    function loadGridDesignSection() {
                        if (businessEntityDefinitionSettings != undefined) {
                            var loadGridDesignPromiseDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([gridFieldsDesignReadyPromiseDeferred.promise, recordTypeSelectedPromiseDeferred.promise])

                                .then(function () {
                                    recordTypeSelectedPromiseDeferred = undefined;

                                    var directivePayload = (businessEntityDefinitionSettings.ManagementDesign != undefined && recordTypeEntity != undefined) ? { selectedColumns: businessEntityDefinitionSettings.ManagementDesign.GridDesign.Columns, recordTypeFields: recordTypeEntity.Fields } : undefined;
                                    gridFieldsDesignReadyPromiseDeferred = undefined;
                                    VRUIUtilsService.callDirectiveLoad(gridFieldsDirectiveAPI, directivePayload, loadGridDesignPromiseDeferred);
                                });

                            return loadGridDesignPromiseDeferred.promise;
                        }
                    }
                    function loadFilterDesignSection() {
                        if (businessEntityDefinitionSettings != undefined) {
                            var loadFilterDesignPromiseDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([filterFieldsDesignReadyPromiseDeferred.promise, recordTypeSelectedPromiseDeferred.promise])

                                .then(function () {
                                    recordTypeSelectedPromiseDeferred = undefined;
                                    var directivePayload = (businessEntityDefinitionSettings != undefined && recordTypeEntity != undefined) ? { selectedFields: businessEntityDefinitionSettings.ManagementDesign.FilterDesign.Fields, recordTypeFields: recordTypeEntity.Fields } : undefined;
                                    filterFieldsDesignReadyPromiseDeferred = undefined;
                                    VRUIUtilsService.callDirectiveLoad(filterFieldsDirectiveAPI, directivePayload, loadFilterDesignPromiseDeferred);
                                });

                            return loadFilterDesignPromiseDeferred.promise;
                        }
                    }
                    function loadDataRecordTypeSelector() {
                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = (businessEntityDefinitionSettings != undefined) ? { selectedIds: businessEntityDefinitionSettings.DataRecordTypeId } : undefined;
                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                            });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }
                    function loadViewRequiredPermission() {
                        var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        viewPermissionReadyDeferred.promise.then(function () {
                            var payload;

                            if (businessEntityDefinitionSettings && businessEntityDefinitionSettings.Security != undefined && businessEntityDefinitionSettings.Security.ViewRequiredPermission != null) {
                                payload = {
                                    data: businessEntityDefinitionSettings.Security.ViewRequiredPermission
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                        });

                        return viewPermissionLoadDeferred.promise;
                    }
                    function loadAddRequiredPermission() {
                        var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        addPermissionReadyDeferred.promise.then(function () {
                            var payload;

                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.Security != undefined && businessEntityDefinitionSettings.Security.AddRequiredPermission != null) {
                                payload = {
                                    data: businessEntityDefinitionSettings.Security.AddRequiredPermission
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(addPermissionAPI, payload, addPermissionLoadDeferred);
                        });

                        return addPermissionLoadDeferred.promise;
                    }
                    function loadEditRequiredPermission() {
                        var editPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                        editPermissionReadyDeferred.promise.then(function () {
                            var payload;

                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.Security != undefined && businessEntityDefinitionSettings.Security.EditRequiredPermission != null) {
                                payload = {
                                    data: businessEntityDefinitionSettings.Security.EditRequiredPermission
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(editPermissionAPI, payload, editPermissionLoadDeferred);
                        });

                        return editPermissionLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getDataRecordType(dataRecordTypeId) {
                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                    recordTypeEntity = response;
                    $scope.scopeModal.fields = response.Fields;
                });
            }
        }

        return directiveDefinitionObject;

    }
]);