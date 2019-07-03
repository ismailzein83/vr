(function (app) {

    'use strict';

    FilterDefinitionChildEntityFilterDirective.$inject = ['UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function FilterDefinitionChildEntityFilterDirective(UtilsService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChildEntityFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/ChildEntityFilterDefinitionTemplate.html'
        };

        function ChildEntityFilterCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var settings;
            var childDataRecordTypeId;

            var dataRecordFieldTypeConfigs = [];
            var isFirstLoad = true;
            var previousChildFieldTypeRuntimeDirective;
            var childFieldType;

            var dataRecordFieldTypeDirectiveAPI;
            var dataRecordFieldTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var childBeDefinitionSelectorAPI;
            var childBeDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var childInputFieldsSelectorAPI;
            var childInputFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var childInputFieldPromiseDeferredSelectionChanged;

            var childFieldTypeRuntimeDirectiveAPI;
            var childFieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var childOutputFieldsSelectorAPI;
            var childOutputFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var parentMappedFieldsSelectorAPI;
            var parentMappedFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var localizationTextResourceSelectorAPI;
            var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFieldTypeDirectiveReady = function (api) {
                    dataRecordFieldTypeDirectiveAPI = api;
                    dataRecordFieldTypeDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChildBusinessEntityDefinitionSelectorReady = function (api) {
                    childBeDefinitionSelectorAPI = api;
                    childBeDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChildInputFieldsSelectorDirectiveReady = function (api) {
                    childInputFieldsSelectorAPI = api;
                    childInputFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChildFieldTypeRumtimeDirectiveReady = function (api) {
                    childFieldTypeRuntimeDirectiveAPI = api;
                    childFieldTypeRuntimeReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChildOutputFieldsSelectorReady = function (api) {
                    childOutputFieldsSelectorAPI = api;
                    childOutputFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onParentMappedFieldsSelectorReady = function (api) {
                    parentMappedFieldsSelectorAPI = api;
                    parentMappedFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
                    localizationTextResourceSelectorAPI = api;
                    localizationTextResourceSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onChildBusinessEntityDefinitionSelectionChanged = function (item) {
                    if (isFirstLoad)
                        return;

                    if (item == undefined) {
                        childInputFieldsSelectorAPI.clearDataSource();
                        childOutputFieldsSelectorAPI.clearDataSource();
                        return;
                    }

                    childDataRecordTypeId = undefined;

                    VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDataRecordTypeIdIfGeneric(item.BusinessEntityDefinitionId).then(function (response) {
                        if (response) {
                            childDataRecordTypeId = response;

                            var ChildFieldsPayload = { dataRecordTypeId: childDataRecordTypeId };

                            var setChildInputFieldsLoader = function (value) {
                                $scope.scopeModel.isChildInputFieldsSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, childInputFieldsSelectorAPI, ChildFieldsPayload, setChildInputFieldsLoader, undefined);

                            var setChildOutputFieldsLoader = function (value) {
                                $scope.scopeModel.isChildOutputFieldsSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, childOutputFieldsSelectorAPI, ChildFieldsPayload, setChildOutputFieldsLoader, undefined);
                        }
                    });
                };

                $scope.scopeModel.onChildInputFieldsSelectionChanged = function (item) {
                    if (item != undefined) {
                        childFieldType = item.Type;

                        if (childInputFieldPromiseDeferredSelectionChanged != undefined) {
                            childInputFieldPromiseDeferredSelectionChanged.resolve();
                        } else {
                            var fieldType = item.Type;
                            var dataRecordFieldTypeConfig = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, fieldType.ConfigId, "ExtensionConfigurationId");

                            if (dataRecordFieldTypeConfig != undefined) {
                                $scope.scopeModel.childFieldTypeRuntimeDirective = dataRecordFieldTypeConfig.FilterEditor;

                                if (previousChildFieldTypeRuntimeDirective != $scope.scopeModel.childFieldTypeRuntimeDirective) {
                                    childFieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    previousChildFieldTypeRuntimeDirective = $scope.scopeModel.fieldTypeRuntimeDirective;
                                }

                                childFieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {

                                    var setLoader = function (value) {
                                        $scope.scopeModel.isChildFieldTypeRumtimeDirectiveLoading = value;
                                    };
                                    var directivePayload = {
                                        fieldTitle: "Default Value",
                                        fieldType: fieldType,
                                        fieldName: item.Name,
                                        fieldValue: undefined
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, childFieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
                                });
                            }
                        }
                    }
                    else {
                        $scope.scopeModel.childFieldTypeRuntimeDirective = undefined;
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;
                    }

                    loadStaticData();
                    initialPromises.push(getDataRecordFieldTypeConfigs());
                    initialPromises.push(loadDataRecordFieldTypeDirective());
                    initialPromises.push(loadChildBEDefinitionSelector());
                    initialPromises.push(loadParentMappedFieldsSelector());
                    initialPromises.push(loadLocalizationTextResourceSelector());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var getBEChildDataRecordTypeIdDeferred = UtilsService.createPromiseDeferred();

                            if (settings != undefined && settings.ChildBusinessEntityDefinitionId != undefined) {
                                VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDataRecordTypeIdIfGeneric(settings.ChildBusinessEntityDefinitionId).then(function (response) {
                                    if (response) {
                                        childDataRecordTypeId = response;
                                    }
                                    getBEChildDataRecordTypeIdDeferred.resolve();
                                });
                            }
                            else {
                                getBEChildDataRecordTypeIdDeferred.resolve();
                            }

                            return {
                                promises: [getBEChildDataRecordTypeIdDeferred.promise],
                                getChildNode: function () {
                                    var promises2 = [];

                                    if (childDataRecordTypeId != undefined) {
                                        promises2.push(loadChildInputFieldsSelector());
                                        promises2.push(loadChildOutputFieldsSelector());
                                    }

                                    if (settings != undefined && settings.ChildInputFieldName != undefined) {
                                        promises2.push(childInputFieldPromiseDeferredSelectionChanged.promise);
                                    }

                                    return {
                                        promises: promises2,
                                        getChildNode: function () {
                                            var promises3 = [];

                                            if (childFieldType != undefined) {
                                                var dataRecordFieldTypeConfig = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, childFieldType.ConfigId, "ExtensionConfigurationId");
                                                if (dataRecordFieldTypeConfig != undefined && dataRecordFieldTypeConfig.FilterEditor != undefined) {
                                                    $scope.scopeModel.childFieldTypeRuntimeDirective = dataRecordFieldTypeConfig.FilterEditor;
                                                    previousChildFieldTypeRuntimeDirective = $scope.scopeModel.fieldTypeRuntimeDirective;
                                                    promises3.push(loadChildFieldTypeRuntimeEditor());
                                                }
                                            }

                                            return {
                                                promises: promises3
                                            };
                                        }
                                    };
                                }
                            };
                        }
                    };


                    function loadStaticData() {
                        if (settings != undefined) {
                            $scope.scopeModel.fieldTitle = settings.FieldTitle;
                            $scope.scopeModel.isRequired = settings.IsRequired;
                        }
                    }

                    function getDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                var dataRecordFieldTypeConfig = response[i];
                                dataRecordFieldTypeConfigs.push(dataRecordFieldTypeConfig);
                            }
                        });
                    }

                    function loadDataRecordFieldTypeDirective() {
                        var dataRecordFieldTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordFieldTypeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var dataRecordFieldTypePayload;
                            if (settings != undefined && settings.FieldType != undefined) {
                                dataRecordFieldTypePayload = settings.FieldType;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordFieldTypeDirectiveAPI, dataRecordFieldTypePayload, dataRecordFieldTypeDirectiveLoadPromiseDeferred);
                        });

                        return dataRecordFieldTypeDirectiveLoadPromiseDeferred.promise;
                    }

                    function loadChildBEDefinitionSelector() {
                        var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        childBeDefinitionSelectorReadyPromiseDeferred.promise.then(function () {

                            var beDefinitionSelectorPayload;
                            if (settings != undefined && settings.ChildBusinessEntityDefinitionId) {
                                beDefinitionSelectorPayload = {
                                    selectedIds: settings.ChildBusinessEntityDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(childBeDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);
                        });

                        return beDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadChildInputFieldsSelector() {
                        var loadChildInputFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        childInputFieldPromiseDeferredSelectionChanged = UtilsService.createPromiseDeferred();

                        childInputFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var dataRecordChildFieldsPayload = {
                                dataRecordTypeId: childDataRecordTypeId,
                                selectedIds: settings != undefined ? settings.ChildInputFieldName : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(childInputFieldsSelectorAPI, dataRecordChildFieldsPayload, loadChildInputFieldsSelectorPromiseDeferred);
                        });

                        return loadChildInputFieldsSelectorPromiseDeferred.promise;
                    }

                    function loadChildFieldTypeRuntimeEditor() {
                        var loadRuntimeEditorPromiseDeferred = UtilsService.createPromiseDeferred();

                        childFieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                fieldTitle: "Default Value"
                            };

                            if (settings != undefined) {
                                directivePayload.fieldType = settings.FieldType;
                                directivePayload.fieldName = settings.ChildInputFieldName;
                                directivePayload.fieldValue = settings.ChildInputDefaultFieldValues;
                            }

                            VRUIUtilsService.callDirectiveLoad(childFieldTypeRuntimeDirectiveAPI, directivePayload, loadRuntimeEditorPromiseDeferred);
                        });

                        return loadRuntimeEditorPromiseDeferred.promise;
                    }

                    function loadChildOutputFieldsSelector() {
                        var loadChildOutputFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        childOutputFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var childOutputFieldsPayload = {
                                dataRecordTypeId: childDataRecordTypeId,
                                selectedIds: settings != undefined ? settings.ChildOutputFieldName : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(childOutputFieldsSelectorAPI, childOutputFieldsPayload, loadChildOutputFieldsSelectorPromiseDeferred);
                        });

                        return loadChildOutputFieldsSelectorPromiseDeferred.promise;
                    }

                    function loadParentMappedFieldsSelector() {
                        var loadParentMappedFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        parentMappedFieldsSelectorReadyPromiseDeferred.promise.then(function () {

                            var parentMappedFieldsPayload = {
                                dataRecordTypeId: context.getDataRecordTypeId(),
                                selectedIds: settings != undefined ? settings.ParentMappedFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(parentMappedFieldsSelectorAPI, parentMappedFieldsPayload, loadParentMappedFieldsSelectorPromiseDeferred);
                        });

                        return loadParentMappedFieldsSelectorPromiseDeferred.promise;
                    }

                    function loadLocalizationTextResourceSelector() {
                        var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {

                            var directivePayload;
                            if (payload.settings != undefined) {
                                directivePayload = {
                                    selectedValue: payload.settings.TextResourceKey
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, directivePayload, loadSelectorPromiseDeferred);
                        });

                        return loadSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        isFirstLoad = false;
                        childInputFieldPromiseDeferredSelectionChanged = undefined;
                    });
                };

                api.getData = function () {
                    var filterValues;
                    if (childFieldTypeRuntimeDirectiveAPI != undefined) {
                        filterValues = childFieldTypeRuntimeDirectiveAPI.getValuesAsArray();
                    }

                    var childInputDefaultFieldValues = [];
                    if (filterValues != undefined) {
                        for (var i = 0; i < filterValues.length; i++) {
                            var filterValue = filterValues[i];
                            if (filterValue != undefined) {
                                childInputDefaultFieldValues.push(filterValue);
                            }
                        }
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.ChildEntityFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        FieldTitle: $scope.scopeModel.fieldTitle,
                        FieldType: dataRecordFieldTypeDirectiveAPI.getData(),
                        IsRequired: $scope.scopeModel.isRequired,
                        ChildBusinessEntityDefinitionId: childBeDefinitionSelectorAPI.getSelectedIds(),
                        ChildInputFieldName: childInputFieldsSelectorAPI.getSelectedIds(),
                        ChildInputDefaultFieldValues: childInputDefaultFieldValues.length > 0 ? childInputDefaultFieldValues : undefined,
                        ChildOutputFieldName: childOutputFieldsSelectorAPI.getSelectedIds(),
                        ParentMappedFieldName: parentMappedFieldsSelectorAPI.getSelectedIds(),
                        TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionChildentityfilter', FilterDefinitionChildEntityFilterDirective);
})(app);