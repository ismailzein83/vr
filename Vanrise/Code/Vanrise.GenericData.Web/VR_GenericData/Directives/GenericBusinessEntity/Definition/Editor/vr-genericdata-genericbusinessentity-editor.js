"use strict";

app.directive("vrGenericdataGenericbusinessentityEditor", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordTypeAPIService", "VR_GenericData_DataRecordFieldAPIService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/GenericBusinessEntityDefinitionEditor.html"

        };

        function GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFields = [];

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeTitleFieldsSelectorAPI;
            var dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var columnDefinitionGridAPI;
            var columnDefinitionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var viewDefinitionGridAPI;
            var viewDefinitionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var editorDefinitionAPI;
            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var filterDefinitionAPI;
            var filterDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var modalWidthSelectorAPI;
            var modalWidthReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var actionDefinitionGridAPI;
            var actionDefinitionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var gridActionDefinitionGridAPI;
            var gridActionDefinitionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var extendedSettingsAPI;
            var extendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var afterSaveHandlerAPI;
            var afterSaveHandlerReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var beforeInsertHandlerAPI;
            var beforeInsertHandlerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var securityAPI;
            var securityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var recordTypeSelectedPromiseDeferred;
            var recordTypeEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordStorageSelectorDirectiveReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeTitleFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeTitleFieldsSelectorAPI = api;
                    dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEColumnDefinitionGridReady = function (api) {
                    columnDefinitionGridAPI = api;
                    columnDefinitionGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEViewDefinitionGridReady = function (api) {
                    viewDefinitionGridAPI = api;
                    viewDefinitionGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionAPI = api;
                    editorDefinitionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEFilterDefinitionDirectiveReady = function (api) {
                    filterDefinitionAPI = api;
                    filterDefinitionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onModalWidthSelectorReady = function (api) {
                    modalWidthSelectorAPI = api;
                    modalWidthReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEActionDefinitionDirectiveReady = function (api) {
                    actionDefinitionGridAPI = api;
                    actionDefinitionGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEGridActionDefinitionDirectiveReady = function (api) {
                    gridActionDefinitionGridAPI = api;
                    gridActionDefinitionGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEEditorExtendedSettingsReady = function (api) {
                    extendedSettingsAPI = api;
                    extendedSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEBeforeInsertHandlerSettingsReady = function (api) {
                    beforeInsertHandlerAPI = api;
                    beforeInsertHandlerReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEAfterSaveHandlerSettingsReady = function (api) {
                    afterSaveHandlerAPI = api;
                    afterSaveHandlerReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBESecurityReady = function (api) {
                    securityAPI = api;
                    securityReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRecordTypeSelectionChanged = function () {
                    var selectedRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                    dataRecordTypeFields.length = 0;
                    if (selectedRecordTypeId != undefined) {
                        reloadReleatedDirectives(selectedRecordTypeId);
                    }
                    else {
                        resetReleatedDirectives();
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Business.GenericBEDefinitionSettings, Vanrise.GenericData.Business",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds(),
                        TitleFieldName: dataRecordTypeTitleFieldsSelectorAPI.getSelectedIds(),
                        Security: securityAPI.getData(),
                        EditorSize: modalWidthSelectorAPI.getSelectedIds(),
                        GridDefinition: {
                            ColumnDefinitions: columnDefinitionGridAPI.getData(),
                            GenericBEGridActions: gridActionDefinitionGridAPI.getData(),
                            GenericBEGridViews: viewDefinitionGridAPI.getData()
                        },
                        EditorDefinition: {
                            Settings: editorDefinitionAPI.getData()
                        },
                        FilterDefinition: {
                            Settings: filterDefinitionAPI.getData()
                        },
                        GenericBEActions: actionDefinitionGridAPI.getData(),
                        ExtendedSettings: extendedSettingsAPI.getData(),
                        OnBeforeInsertHandler: beforeInsertHandlerAPI.getData(),
                        OnAfterSaveHandler: afterSaveHandlerAPI.getData()
                    };
                };

                api.load = function (payload) {
                    var businessEntityDefinitionSettings;
                    var promises = [];
                    if (payload != undefined) {
                        businessEntityDefinitionSettings = payload.businessEntityDefinitionSettings;

                        if (businessEntityDefinitionSettings != undefined)
                            recordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                    }
                    promises.push(loadDataRecordTypeSelector());
                    promises.push(loadDataRecordStorageSelector());
                    promises.push(loadActionDefinitionGrid());
                    promises.push(loadExtendedSettingsEditor());

                    promises.push(loadColumnDefinitionGrid());
                    promises.push(loadGridActionDefinitionGrid());

                    promises.push(loadViewDefinitionGrid());
                    promises.push(loadEditorDefinitionDirective());
                    promises.push(loadFilterDefinitionDirective());
                    promises.push(loadModalWidthSelector());


                    promises.push(loadAfterSaveHandlerSettings());
                    promises.push(loadBeforeInsertHandlerSettings());

                    promises.push(loadSecurityDirective());

                    if (businessEntityDefinitionSettings != undefined) {
                        promises.push(loadDataRecordTitleFieldsSelector());
                        promises.push(getDataRecordFieldsInfo(businessEntityDefinitionSettings.DataRecordTypeId));
                    }



                    function loadDataRecordTypeSelector() {

                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var directivePayload;

                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.DataRecordTypeId != undefined) {
                                directivePayload = {};
                                directivePayload.selectedIds = businessEntityDefinitionSettings.DataRecordTypeId;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    function loadDataRecordStorageSelector() {

                        var loadDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordStorageSelectorReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = (businessEntityDefinitionSettings != undefined) ? { selectedIds: businessEntityDefinitionSettings.DataRecordStorageId } : undefined;
                                VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, directivePayload, loadDataRecordStorageSelectorPromiseDeferred);
                            });

                        return loadDataRecordStorageSelectorPromiseDeferred.promise;
                    }

                    function loadDataRecordTitleFieldsSelector() {
                        var loadDataRecordTypeTitleFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.promise, recordTypeSelectedPromiseDeferred.promise]).then(function () {
                            var typeFieldsPayload = {
                                dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                                selectedIds: businessEntityDefinitionSettings.TitleFieldName
                            };

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeTitleFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeTitleFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeTitleFieldsSelectorPromiseDeferred.promise;
                    }


                    function loadColumnDefinitionGrid() {
                        var loadColumnDefinitionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        columnDefinitionGridReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext()
                            };
                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.GridDefinition != undefined && businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions != undefined)
                                payload.columnDefinitions = businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions;

                            VRUIUtilsService.callDirectiveLoad(columnDefinitionGridAPI, payload, loadColumnDefinitionGridPromiseDeferred);
                        });
                        return loadColumnDefinitionGridPromiseDeferred.promise;
                    }

                    function loadGridActionDefinitionGrid() {
                        var loadGridActionDefinitionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        gridActionDefinitionGridReadyPromiseDeferred.promise.then(function () {
                            var gActionpayload = {
                                context: getContext()
                            };
                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.GridDefinition != undefined && businessEntityDefinitionSettings.GridDefinition.GenericBEGridActions != undefined)
                                gActionpayload.genericBEGridActions = businessEntityDefinitionSettings.GridDefinition.GenericBEGridActions;

                            VRUIUtilsService.callDirectiveLoad(gridActionDefinitionGridAPI, gActionpayload, loadGridActionDefinitionGridPromiseDeferred);
                        });
                        return loadGridActionDefinitionGridPromiseDeferred.promise;
                    }

                    function loadViewDefinitionGrid() {
                        var loadViewDefinitionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        viewDefinitionGridReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                genericBEGridViews: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.GridDefinition != undefined && businessEntityDefinitionSettings.GridDefinition.GenericBEGridViews || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewDefinitionGridAPI, payload, loadViewDefinitionGridPromiseDeferred);
                        });
                        return loadViewDefinitionGridPromiseDeferred.promise;
                    }

                    function loadEditorDefinitionDirective() {
                        var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        editorDefinitionReadyPromiseDeferred.promise.then(function () {
                            var editorPayload = {
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.EditorDefinition && businessEntityDefinitionSettings.EditorDefinition.Settings || undefined,
                                context: getContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
                        });
                        return loadEditorDefinitionDirectivePromiseDeferred.promise;
                    }

                    function loadFilterDefinitionDirective() {
                        var loadFilterDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        filterDefinitionReadyPromiseDeferred.promise.then(function () {
                            var filterPayload = {
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.FilterDefinition && businessEntityDefinitionSettings.FilterDefinition.Settings || undefined,
                                context: getContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(filterDefinitionAPI, filterPayload, loadFilterDefinitionDirectivePromiseDeferred);
                        });
                        return loadFilterDefinitionDirectivePromiseDeferred.promise;
                    }


                    function loadModalWidthSelector() {
                        var loadModalWidthSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        modalWidthReadyPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                selectedIds: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.EditorSize != undefined ? businessEntityDefinitionSettings.EditorSize : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(modalWidthSelectorAPI, selectorPayload, loadModalWidthSelectorPromiseDeferred);
                        });

                        return loadModalWidthSelectorPromiseDeferred.promise;
                    }


                    function loadActionDefinitionGrid() {
                        var loadActionDefinitionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        actionDefinitionGridReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext(),
                                genericBEActions: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.GenericBEActions || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(actionDefinitionGridAPI, payload, loadActionDefinitionGridPromiseDeferred);
                        });
                        return loadActionDefinitionGridPromiseDeferred.promise;
                    }

                    function loadExtendedSettingsEditor() {
                        var loadExtendedSettingsEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                        extendedSettingsReadyPromiseDeferred.promise.then(function () {
                            var settingPayload = {
                                context: getContext(),
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.ExtendedSettings || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, settingPayload, loadExtendedSettingsEditorPromiseDeferred);
                        });
                        return loadExtendedSettingsEditorPromiseDeferred.promise;
                    }

                    function loadAfterSaveHandlerSettings() {
                        var loadAfterSaveHandlerSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                        afterSaveHandlerReadyPromiseDeferred.promise.then(function () {
                            var settingPayload = {
                                context: getContext(),
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.OnAfterSaveHandler || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(afterSaveHandlerAPI, settingPayload, loadAfterSaveHandlerSettingsPromiseDeferred);
                        });
                        return loadAfterSaveHandlerSettingsPromiseDeferred.promise;
                    }

                    function loadBeforeInsertHandlerSettings() {
                        var loadBeforeInsertHandlerSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                        beforeInsertHandlerReadyPromiseDeferred.promise.then(function () {
                            var settingPayload = {
                                context: getContext(),
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.OnBeforeInsertHandler || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(beforeInsertHandlerAPI, settingPayload, loadBeforeInsertHandlerSettingsPromiseDeferred);
                        });
                        return loadBeforeInsertHandlerSettingsPromiseDeferred.promise;
                    }

                    function loadSecurityDirective() {
                        var loadSecurityDPromiseDeferred = UtilsService.createPromiseDeferred();
                        securityReadyPromiseDeferred.promise.then(function () {
                            var securityPayload;
                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.Security != undefined)
                                securityPayload = businessEntityDefinitionSettings.Security;

                            VRUIUtilsService.callDirectiveLoad(securityAPI, securityPayload, loadSecurityDPromiseDeferred);
                        });
                        return loadSecurityDPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        recordTypeSelectedPromiseDeferred = undefined;
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }

            function getContext() {
                return {
                    getDataRecordTypeId: function () {
                        return dataRecordTypeSelectorAPI.getSelectedIds();
                    },
                    getRecordTypeFields: function () {
                        var data = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            data.push(dataRecordTypeFields[i]);
                        }
                        return data;
                    },
                    getFields: function () {
                        var dataFields = [];
                        for (var i = 0; i < dataRecordTypeFields.length; i++) {
                            dataFields.push({
                                FieldName: dataRecordTypeFields[i].Name,
                                FieldTitle: dataRecordTypeFields[i].Title,
                                Type: dataRecordTypeFields[i].Type
                            });
                        }
                        return dataFields;
                    },
                    getActionInfos: function () {
                        var data = [];
                        if (actionDefinitionGridAPI == undefined)
                            return data;
                        var actionDefinitions = actionDefinitionGridAPI.getData();
                        for (var i = 0; i < actionDefinitions.length; i++) {
                            data.push({
                                GenericBEActionId: actionDefinitions[i].GenericBEActionId,
                                Name: actionDefinitions[i].Name,
                            });
                        }
                        return data;
                    }
                };
            }

            function reloadReleatedDirectives(selectedRecordTypeId) {

                var setDataRecordTypeTitleLoader = function (value) {
                    $scope.scopeModel.isLoadingTitle = value
                };
                var recordTypeTitlePayload = {
                    dataRecordTypeId: selectedRecordTypeId
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeTitleFieldsSelectorAPI, recordTypeTitlePayload, setDataRecordTypeTitleLoader, recordTypeSelectedPromiseDeferred);

                var setGridColumnLoader = function (value) {
                    setTimeout(function () {
                        $scope.scopeModel.isLoadingColumns = value;
                        $scope.$apply();
                    }, 1);
                };
                var columnDefnitionPayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, columnDefinitionGridAPI, columnDefnitionPayload, setGridColumnLoader, recordTypeSelectedPromiseDeferred);

                var setFilterLoader = function (value) {
                    setTimeout(function () {
                        $scope.scopeModel.isLoadingFilter = value;
                        $scope.$apply();
                    }, 1);
                };
                var filterPayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDefinitionAPI, filterPayload, setFilterLoader, recordTypeSelectedPromiseDeferred);

                $scope.scopeModel.isLoadingEditor = true;
                getDataRecordFieldsInfo(selectedRecordTypeId).then(function () {
                    $scope.scopeModel.isLoadingEditor = false;
                    var editorPayload = {
                        context: getContext()
                    };
                    var setEditorLoader = function (value) {
                        $scope.scopeModel.isLoadingEditor = value
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorDefinitionAPI, editorPayload, setEditorLoader, recordTypeSelectedPromiseDeferred);
                });

                var setBeforeInsertLoader = function (value) {
                    setTimeout(function () {
                        $scope.scopeModel.isLoadingBeforeInsert = value;
                        $scope.$apply();
                    }, 1);
                };
                var beforeInsertPayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, beforeInsertHandlerAPI, beforeInsertPayload, setBeforeInsertLoader, recordTypeSelectedPromiseDeferred);


                var setAfterSaveLoader = function (value) {
                    setTimeout(function () {
                        $scope.scopeModel.isLoadingAfterSave = value;
                        $scope.$apply();
                    }, 1);
                };
                var afterSavePayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, afterSaveHandlerAPI, afterSavePayload, setAfterSaveLoader, recordTypeSelectedPromiseDeferred);
            }

            function resetReleatedDirectives() {
                if (dataRecordTypeTitleFieldsSelectorAPI != undefined)
                    dataRecordTypeTitleFieldsSelectorAPI.clearDataSource();
                if (columnDefinitionGridAPI != undefined)
                    columnDefinitionGridAPI.clearDataSource();
                if (editorDefinitionAPI != undefined)
                    editorDefinitionAPI.load();
                if (filterDefinitionAPI != undefined)
                    filterDefinitionAPI.load();
                if (afterSaveHandlerAPI != undefined)
                    afterSaveHandlerAPI.load();
                if (beforeInsertHandlerAPI != undefined)
                    beforeInsertHandlerAPI.load();
            }


            function getDataRecordFieldsInfo(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId, null).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined)
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                });
            }

        }

        return directiveDefinitionObject;

    }
]);