'use strict';


app.directive('reprocessReprocessfilterdefinitionSettings', ['UtilsService', 'VRUIUtilsService', 'Reprocess_ReprocessFilterDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, Reprocess_ReprocessFilterDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Ctor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Reprocess/Directives/ReprocessFilterDefinition/Templates/ReprocessFilterDefinitionSettings.html'
        };

        function Ctor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var isFirstTimeLoading = false;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;
                $scope.scopeModel.mappingFields = [];

                $scope.scopeModel.fields = [];

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = { context: buildFilterDefinitionContext() };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                $scope.scopeModel.onSelectorSelectionChanged = function (selectedValue) {
                    if (!isFirstTimeLoading) {
                        $scope.scopeModel.mappingFields = [];
                        $scope.scopeModel.fields = [];
                        $scope.scopeModel.selectedDataRecordTypes = [];
                    }
                };

                $scope.scopeModel.isMappingFieldsGridValid = function () {
                    if ($scope.scopeModel.mappingFields != undefined && $scope.scopeModel.mappingFields.length > 0 && ($scope.scopeModel.fields == undefined || $scope.scopeModel.fields.length == 0))
                        return 'At least one field should be added';
                    return null;
                };

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onselectDataRecordType = function (dataRecordType) {
                    var dataItem = {
                        Id: dataRecordType.DataRecordTypeId,
                        Name: dataRecordType.Name,
                        Mappings: {},
                    };

                    var promises = [];
                    $scope.scopeModel.loadingMappingFields = true;
                    for (var x = 0; x < $scope.scopeModel.fields.length; x++) {
                        var fieldName = $scope.scopeModel.fields[x].columnName;
                        var filterItem = {
                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            readyPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(filterItem.loadPromiseDeferred.promise);
                        addMappingFieldAPI(filterItem, dataItem, fieldName);
                    }

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.loadingMappingFields = false;
                    });

                    $scope.scopeModel.mappingFields.push(dataItem);
                };

                $scope.scopeModel.ondeselectRecordType = function (dataRecordType) {
                    var itemIndex = UtilsService.getItemIndexByVal($scope.scopeModel.mappingFields, dataRecordType.Id, 'Id');
                    $scope.scopeModel.mappingFields.splice(itemIndex, 1);
                };

                $scope.onSelectionChanged = function (dataItem, columnName) {
                    if (columnName != undefined) {
                        var currentMappingField = dataItem.Mappings[columnName];
                        if (currentMappingField != undefined && currentMappingField.directiveAPI != undefined)
                            currentMappingField.selectedDataRecordField = currentMappingField.directiveAPI.getSelectedIds();
                        else
                            currentMappingField.selectedDataRecordField = undefined;
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    isFirstTimeLoading = true;
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var filterDefinition;

                    if (payload != undefined) {
                        filterDefinition = payload.filterDefinition;
                    }

                    var loadReprocessFilterDefinitionConfigs = getReprocessFilterDefinitionConfigs();
                    promises.push(loadReprocessFilterDefinitionConfigs);

                    var selectedRecordTypes = undefined;

                    if (filterDefinition != undefined) {

                        $scope.scopeModel.applyFilterToSourceData = filterDefinition.ApplyFilterToSourceData;

                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);

                        if (filterDefinition.MappingFields != undefined) {
                            loadMappingFields();
                        }
                    }

                    var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector(selectedRecordTypes);
                    promises.push(loadDataRecordTypeSelectorPromise);

                    function loadMappingFields() {
                        selectedRecordTypes = [];

                        var isFirstItem = true;
                        for (var prop in filterDefinition.MappingFields) {
                            if (prop == '$type')
                                continue;

                            var itemToBeAdded = {
                                Id: prop,
                                Mappings: {}
                            };

                            selectedRecordTypes.push(prop);
                            var mappings = filterDefinition.MappingFields[prop];

                            for (var propMapping in mappings) {
                                if (propMapping == '$type')
                                    continue;

                                var fieldObject = {
                                    columnName: propMapping,
                                    onDataRecordFieldSelectorReadyPath: "dataItem.Mappings." + propMapping + ".onDataRecordFieldSelectorReady"
                                };

                                if (isFirstItem) {
                                    $scope.scopeModel.fields.push(fieldObject);
                                }

                                var mappingFieldName = mappings[propMapping];
                                var filterItem = {
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                addMappingFieldAPI(filterItem, itemToBeAdded, propMapping, mappingFieldName);
                                promises.push(filterItem.loadPromiseDeferred.promise);
                            }
                            $scope.scopeModel.mappingFields.push(itemToBeAdded);
                            isFirstItem = false;
                        }
                    };

                    function getReprocessFilterDefinitionConfigs() {
                        return Reprocess_ReprocessFilterDefinitionAPIService.GetReprocessFilterDefinitionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (filterDefinition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, filterDefinition.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    };

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                filterDefinition: filterDefinition,
                                context: buildFilterDefinitionContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });
                        return directiveLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstTimeLoading = false;
                    });

                    function loadDataRecordTypeSelector(selectedRecordTypes) {

                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload = { selectedIds: selectedRecordTypes };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                        });
                        return dataRecordTypeSelectorLoadDeferred.promise.then(function () {
                            if ($scope.scopeModel.mappingFields != undefined && $scope.scopeModel.mappingFields.length > 0) {
                                for (var x = 0; x < $scope.scopeModel.mappingFields.length; x++) {
                                    var currentMappingFieldItem = $scope.scopeModel.mappingFields[x];
                                    var dataRecordTypeObj = UtilsService.getItemByVal($scope.scopeModel.selectedDataRecordTypes, currentMappingFieldItem.Id, 'DataRecordTypeId');
                                    currentMappingFieldItem.Name = dataRecordTypeObj.Name;
                                }
                            }
                        });
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = $scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined ? directiveAPI.getData() : undefined;
                    if (obj != undefined && $scope.scopeModel.mappingFields != undefined && $scope.scopeModel.mappingFields.length > 0 && $scope.scopeModel.fields != undefined && $scope.scopeModel.fields.length > 0) {
                        obj.ApplyFilterToSourceData = $scope.scopeModel.applyFilterToSourceData;
                        obj.MappingFields = {};

                        for (var x = 0; x < $scope.scopeModel.mappingFields.length; x++) {
                            var currentRow = $scope.scopeModel.mappingFields[x];
                            var dataRecordTypeMappings = obj.MappingFields[currentRow.Id] = {};
                            for (var y = 0; y < $scope.scopeModel.fields.length; y++) {
                                var currentColumn = $scope.scopeModel.fields[y];

                                var selectedId = currentRow.Mappings[currentColumn.columnName].directiveAPI.getSelectedIds();
                                dataRecordTypeMappings[currentColumn.columnName] = selectedId != undefined ? selectedId : null;
                            }
                        }
                    }
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function buildFilterDefinitionContext() {
                return {
                    onFieldAdded: function (fieldName) {
                        var fieldObject = {
                            columnName: fieldName,
                            onDataRecordFieldSelectorReadyPath: "dataItem.Mappings." + fieldName + ".onDataRecordFieldSelectorReady"
                        };
                        $scope.scopeModel.loadingMappingFields = true;
                        var promises = [];
                        for (var x = 0; x < $scope.scopeModel.mappingFields.length; x++) {
                            var currentItem = $scope.scopeModel.mappingFields[x];

                            var filterItem = {
                                loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                readyPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            addMappingFieldAPI(filterItem, currentItem, fieldName);
                        }

                        UtilsService.waitMultiplePromises(promises).then(function () {
                            $scope.scopeModel.loadingMappingFields = false;
                        });
                        $scope.scopeModel.fields.push(fieldObject);
                    },
                    onFieldDeleted: function (fieldName) {
                        var itemIndex = UtilsService.getItemIndexByVal($scope.scopeModel.fields, fieldName, 'columnName');
                        $scope.scopeModel.fields.splice(itemIndex, 1);
                    },
                };
            };

            function addMappingFieldAPI(filterItem, currentItem, fieldName, selectedvalue) {
                var currentFieldItem = currentItem.Mappings[fieldName] = {};
                var payloadSelector = { dataRecordTypeId: currentItem.Id };

                currentFieldItem.onDataRecordFieldSelectorReady = function (api) {
                    currentFieldItem.directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    if (selectedvalue != undefined)
                        payloadSelector.selectedIds = selectedvalue;
                    else
                        payloadSelector.selectedIds = currentFieldItem.selectedDataRecordField;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currentFieldItem.directiveAPI, payloadSelector, setLoader, filterItem.readyPromiseDeferred);
                };

                filterItem.readyPromiseDeferred.promise.then(function () {
                    filterItem.readyPromiseDeferred = undefined;

                    VRUIUtilsService.callDirectiveLoad(currentFieldItem.directiveAPI, payloadSelector, filterItem.loadPromiseDeferred);
                });
            };
        }
    }]);